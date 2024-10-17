using HuloToys_Front_End.Models.Products;
using HuloToys_Service.ElasticSearch;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.Products;
using HuloToys_Service.MongoDb;
using HuloToys_Service.RedisWorker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;
using Utilities.Contants;

namespace WEB.CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductSpecificationMongoAccess _productSpecificationMongoAccess;
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        private readonly CartMongodbService _cartMongodbService;
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;
        private readonly GroupProductESService groupProductESService;

        public ProductController(IConfiguration configuration, RedisConn redisService)
        {
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
            _productSpecificationMongoAccess = new ProductSpecificationMongoAccess(configuration);
            _cartMongodbService = new CartMongodbService(configuration);
            groupProductESService = new GroupProductESService(configuration["DataBaseConfig:Elastic:Host"], configuration);

            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
        }
              
        [HttpPost("get-list.json")]
        public async Task<IActionResult> ProductListing([FromBody] APIRequestGenericModel input)
        {
            try
            {              
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductListRequestModel>(objParr[0].ToString());
                    int node_redis = Convert.ToInt32(_configuration["Redis:Database:db_search_result"]);
                    var obj_product = new ProductListResponseModel();
                    int total_max_cache = 100; // số bản ghi tối đa để cache    
                    int group_product_id = request.group_id;
                    int skip = request.page_index;
                    int top = request.page_size;

                    string cache_name = CacheType.PRODUCT_LISTING + group_product_id;
                    var j_data = await _redisService.GetAsync(cache_name, node_redis);

                    // Kiểm tra có trong cache không ?
                    if (!string.IsNullOrEmpty(j_data))
                    {
                        obj_product = JsonConvert.DeserializeObject<ProductListResponseModel>(j_data);
                        // Nếu tổng số bản ghi muốn lấy vượt quá số bản ghi trong Redis thì vào ES lấy                        
                        if (top > obj_product.items.Count())
                        {
                            // Lấy ra trong Mongo
                            var data = await _productDetailMongoAccess.ResponseListing(request.keyword, request.group_id, 0, top);
                        }
                    }
                    else // Không có trong cache
                    {
                        // Lấy ra số bản ghi tối đa để cache                        
                        obj_product = await _productDetailMongoAccess.ResponseListing(request.keyword, request.group_id, 0, Math.Max(total_max_cache, top));

                        if (obj_product.count > 0)
                        {

                            _redisService.Set(cache_name, JsonConvert.SerializeObject(obj_product), node_redis);
                        }
                    }

                    if (obj_product != null && obj_product.items.Count() > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = obj_product.items.ToList().Skip(skip).Take(top),
                            total = obj_product.count
                        });
                    }

                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "data empty !!! with group_product_id = " + group_product_id
                        });
                    }
                }
                else
                {     
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = ResponseMessages.DataInvalid ,
                    });
                }

            }
            catch
            {
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = ResponseMessages.DataInvalid,
                });
            }

        }

        [HttpPost("detail")]
        public async Task<IActionResult> ProductDetail([FromBody] APIRequestGenericModel input)
        {
            try
            {
                int node_redis = Convert.ToInt32(_configuration["Redis:Database:db_search_result"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    string product_id = objParr[0]["product_id"].ToString();

                    var cache_name = CacheType.PRODUCT_DETAIL + product_id;
                    var j_data = await _redisService.GetAsync(cache_name, node_redis);
                    if (j_data != null && j_data.Trim() != "")
                    {
                        var result = JsonConvert.DeserializeObject<ProductDetailResponseModel>(j_data);
                        if (result != null)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = ResponseMessages.Success,
                                data = result
                            });
                        }
                    }
                    var data = await _productDetailMongoAccess.GetFullProductById(product_id);
                    if (data != null)
                    {
                        _redisService.Set(cache_name, JsonConvert.SerializeObject(data), node_redis);
                    }
                    var request = JsonConvert.DeserializeObject<ProductDetailRequestModel>(objParr[0].ToString());

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = ResponseMessages.DataInvalid
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Loi xac thuc authent",
                    });
                }
            }
            catch
            {
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Failed",
                });
            }
        }

        [HttpPost("group-product")]
        public async Task<IActionResult> GroupProduct([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductListRequestModel>(objParr[0].ToString());
                    if (request == null || request.group_id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = groupProductESService.GetListGroupProductByParentId(request.group_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = data
                    });
                }


            }
            catch
            {

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid,
            });
        }

        [HttpPost("brand.json")]
        public async Task<IActionResult> ProductBrand([FromBody] APIRequestGenericModel input)
        {
            try
            {

                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductBrandRequestModel>(objParr[0].ToString());
                

                    var data = await _productSpecificationMongoAccess.GetByType(1);

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY                       
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ex.ToString(),
                });
            }

        }
        [HttpPost("product-by-brand.json")]
        public async Task<IActionResult> ProductByBrand([FromBody] APIRequestGenericModel input)
        {
            try
            {
                string brand_name = "";
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    string brand_id = objParr[0]["brand_id"].ToString();
                    var brand = await _productSpecificationMongoAccess.GetByID(brand_id);
                    brand_name = brand.attribute_name;
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = brand,
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = ResponseMessages.DataInvalid,
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ex.ToString(),
                });
            }

        }
        [HttpPost("product-by-pricerange.json")]
        public async Task<IActionResult> ProductByPriceRange([FromBody] APIRequestGenericModel input)
        {
            try
            {
                //input.token = CommonHelper.Encode(

                //    JsonConvert.SerializeObject(new ProductBrandRequestModel()
                //    {
                //        amount_min= 140000,
                //        amount_max=170000,
                //        group_product_id = 54,
                //        page_index = 1,
                //        page_size = 2
                //    })
                //, _configuration["KEY:private_key"]);

                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductBrandRequestModel>(objParr[0].ToString());                 
                                        
                    var data = await _productDetailMongoAccess.ListingByPriceRange(request.amount_min, request.amount_max, "", request.group_product_id, request.page_index, request.page_size);

                    return Ok(new
                    {
                        status = data.count > 0 ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
                        data = data,
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = ResponseMessages.DataInvalid,
                    });
                }

            }
            catch
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ResponseMessages.DataInvalid,
                });
            }
           
        }

        [HttpPost("search.json")]
        public async Task<IActionResult> searchByProductName([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    string keyword =  objParr[0]["keyword"].ToString();

                    var data = await _productDetailMongoAccess.searchByProductName(keyword);

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = ex.ToString(),
                });
            }

        }
    }

}