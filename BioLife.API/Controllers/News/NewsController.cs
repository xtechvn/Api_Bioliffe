using HuloToys_Service.Controllers.News.Business;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers
{
    [Route("api/news")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {

        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly NewsBusiness _newsBusiness;

        public NewsController(IConfiguration config, RedisConn redisService)
        {
            configuration = config;

            _redisService = redisService;
            _redisService = new RedisConn(config);
            _redisService.Connect();
            _newsBusiness = new NewsBusiness(configuration);

        }

        /// <summary>
        /// Lấy ra bài viết theo 1 chuyên mục
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //[HttpPost("get-list-by-categoryid.json")]
        //public async Task<ActionResult> getListArticleByCategoryId([FromBody] APIRequestGenericModel input)
        //{
        //    try
        //    {
        //        JArray objParr = null;
        //        if (CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
        //        {
        //            var _category_detail = new GroupProductModel();
        //            var list_article = new List<CategoryArticleModel>();
        //            int total_max_cache = 100; // số bản ghi tối đa để cache

        //            int category_id = Convert.ToInt32(objParr[0]["category_id"]);

        //            int skip =  Convert.ToInt32(objParr[0]["skip"]);
        //            int take =  Convert.ToInt32(objParr[0]["take"]);
                    
        //            string cache_name = CacheType.ARTICLE_CATEGORY_ID + category_id;
        //            var j_data = await _redisService.GetAsync(cache_name, Convert.ToInt32(configuration["Redis:Database:db_common"]));

        //            // Kiểm tra có trong cache không ?
        //            if (!string.IsNullOrEmpty(j_data))
        //            {
        //                list_article = JsonConvert.DeserializeObject<List<CategoryArticleModel>>(j_data);
        //                // Nếu tổng số bản ghi muốn lấy vượt quá số bản ghi trong Redis thì vào ES lấy                        
        //                if (take > list_article.Count())
        //                {
        //                    // Lấy ra trong es
        //                    list_article = await _newsBusiness.getArticleListByCategoryId(category_id, take);
        //                }
        //            }
        //            else // Không có trong cache
        //            {
        //                // lấy ra 100 bản ghi để đi cache. Trường hợp cần hơn sẽ vào ES lấy
        //                list_article = await _newsBusiness.getArticleListByCategoryId(category_id, total_max_cache);

        //                if (list_article.Count() > 0)
        //                {
        //                    _redisService.Set(cache_name, JsonConvert.SerializeObject(list_article), Convert.ToInt32(configuration["Redis:Database:db_common"]));
        //                }
        //            }

        //            if (list_article != null && list_article.Count() > 0)
        //            {
        //                // lay ra chi tiet chuyen muc
        //                _category_detail = _newsBusiness.GetById(category_id);

        //                return Ok(new
        //                {
        //                    status = (int)ResponseType.SUCCESS,
        //                    data = list_article.ToList(),
        //                    category_detail = _category_detail
        //                }); ;
        //            }
        //            else
        //            {
        //                return Ok(new
        //                {
        //                    status = (int)ResponseType.EMPTY,
        //                    msg = "data empty !!!"
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                status = (int)ResponseType.ERROR,
        //                msg = "Key ko hop le"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.FAILED,
        //            msg = "Error: " + ex.ToString(),
        //        });
        //    }
        //}

        /// <summary>
        /// Lấy ra các bài viết từ store sync sang  es_biolife_sp_get_article
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("get-list-news.json")]
        public async Task<ActionResult> getListNews([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    int node_redis = Convert.ToInt32(configuration["Redis:Database:db_article"]);
                    var _category_detail = new GroupProductModel();
                    var list_article = new List<CategoryArticleModel>();
                    int total_max_cache = 100 ; // số bản ghi tối đa để cache    
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);

                    int skip = Convert.ToInt32(objParr[0]["skip"]);
                    int top = Convert.ToInt32(objParr[0]["top"]);

                    string cache_name = CacheType.ARTICLE_CATEGORY_ID + category_id;
                    var j_data = await _redisService.GetAsync(cache_name, node_redis);

                    // Kiểm tra có trong cache không ?
                    if (!string.IsNullOrEmpty(j_data))
                    {
                        list_article = JsonConvert.DeserializeObject<List<CategoryArticleModel>>(j_data);
                        // Nếu tổng số bản ghi muốn lấy vượt quá số bản ghi trong Redis thì vào ES lấy                        
                        if (top > list_article.Count())
                        {
                            // Lấy ra trong es
                            list_article = await _newsBusiness.getListNews(category_id, top);
                        }
                    }
                    else // Không có trong cache
                    {
                        // Lấy ra số bản ghi tối đa để cache
                        list_article = await _newsBusiness.getListNews(category_id, Math.Max(total_max_cache,top));

                        if (list_article.Count() > 0)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(list_article), node_redis);
                        }
                    }

                    if (list_article != null && list_article.Count() > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = list_article.ToList().Skip(skip).Take(top)
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "data empty !!!"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        /// <summary>
        /// Lấy ra chi tiết bài viết
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-article-detail.json")]
        public async Task<ActionResult> getArticleDetail([FromBody] APIRequestGenericModel input)
        {
            try
            {
                int node_redis = Convert.ToInt32(configuration["Redis:Database:db_article"]);
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {                    
                    var article_detail = new ArticleModel();                                        

                    long article_id = Convert.ToInt64(objParr[0]["article_id"]);                    

                    string cache_name = CacheType.ARTICLE_ID + article_id;
                    var j_data = await _redisService.GetAsync(cache_name, node_redis);

                    // Kiểm tra có trong cache không ?
                    if (!string.IsNullOrEmpty(j_data))
                    {
                        article_detail = JsonConvert.DeserializeObject<ArticleModel>(j_data);                        
                    }
                    else // Không có trong cache
                    {                        
                        article_detail = await _newsBusiness.getArticleDetail(article_id);

                        if (article_detail != null)
                        {
                            _redisService.Set(cache_name, JsonConvert.SerializeObject(article_detail), node_redis);
                        }
                    }

                    if (article_detail != null)
                    {                        

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = article_detail
                        }); ;
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.EMPTY,
                            msg = "data empty !!!"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

        [HttpPost("get-total-news.json")]
        public async Task<ActionResult> getTotalItemNewsByCategoryId([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    int category_id = Convert.ToInt32(objParr[0]["category_id"]);
                    // Lấy ra trong es
                    var total = await _newsBusiness.getTotalItemNewsByCategoryId(category_id);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = total
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Key ko hop le"
                    });
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }
    }
}
