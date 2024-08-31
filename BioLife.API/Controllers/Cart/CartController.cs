using Models.MongoDb;
using HuloToys_Service.MongoDb;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;
using HuloToys_Front_End.Models.Products;
using HuloToys_Service.Models.Cart;
using HuloToys_Service.Models.APIRequest;

namespace HuloToys_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WorkQueueClient workQueueClient;
        private readonly CartMongodbService _cartMongodbService;
        private readonly ProductDetailMongoAccess _productDetailMongoAccess;
        
        public CartController(IConfiguration configuration)
        {
            _configuration  = configuration;

            workQueueClient = new WorkQueueClient(configuration);
            _cartMongodbService = new CartMongodbService(configuration);
            _productDetailMongoAccess = new ProductDetailMongoAccess(configuration);
           
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductAddToCartRequestModel>(objParr[0].ToString());
                    if (request == null || request.product_id == null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = await _cartMongodbService.FindByProductId(request.product_id, request.account_client_id);
                    int id = 0;
                    if (data == null || data.product == null)
                    {
                        var product = await _productDetailMongoAccess.GetByID(request.product_id);

                        await _cartMongodbService.Insert(new CartItemMongoDbModel()
                        {
                            account_client_id = request.account_client_id,
                            product = product,
                            quanity = request.quanity,
                            total_amount=product.amount * request.quanity,
                            created_date=DateTime.Now,
                        });
                        id =1;
                    }
                    else
                    {
                        data.quanity += request.quanity;
                        data.created_date = DateTime.Now;
                         await _cartMongodbService.UpdateCartQuanity(data);
                        id = 2;
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = ResponseMessages.Success,
                        data = id
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
        [HttpPost("count")]
        public async Task<IActionResult> CountCart([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductCartCountRequestModel>(objParr[0].ToString());
                    if (request == null || request.account_client_id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = await _cartMongodbService.Count(request.account_client_id);

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
        [HttpPost("delete")]
        public async Task<ActionResult> Delete([FromBody] APIRequestGenericModel input)
        {
            try
            {


                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<CartDeleteRequestModel>(objParr[0].ToString());
                    if (request == null || request.id == null || request.id.Trim() == "")
                    {

                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var result = await _cartMongodbService.Delete(request.id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Success",
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = ResponseMessages.FunctionExcutionFailed
                });

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.FunctionExcutionFailed
            });
        }
        [HttpPost("list")]
        public async Task<IActionResult> Listing([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, _configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<ProductCartCountRequestModel>(objParr[0].ToString());
                    if (request == null || request.account_client_id <= 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = ResponseMessages.DataInvalid
                        });
                    }
                    var data = await _cartMongodbService.GetList(request.account_client_id);

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
       
    }
}
