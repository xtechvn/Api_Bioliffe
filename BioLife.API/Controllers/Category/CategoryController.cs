using HuloToys_Service.Controllers.News.Business;
using HuloToys_Service.Models.APIRequest;
using HuloToys_Service.Models.Article;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;
using Utilities.Contants;


namespace HuloToys_Service.Controllers.Category
{
    [Route("api/{controller}")]
    [ApiController]
    [Authorize]

    public class CategoryController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly RedisConn _redisService;
        private readonly NewsBusiness _newsBusiness;

        public CategoryController(IConfiguration config, RedisConn redisService)
        {
            configuration = config;

            _redisService = redisService;
            _redisService = new RedisConn(config);
            _redisService.Connect();
            _newsBusiness = new NewsBusiness(configuration);

        }

        /// <summary>
        /// Lấy ra tất cả các chuyên mục thuộc B2C theo Id cha
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("get-list.json")]
        public async Task<ActionResult> GetAllCategory([FromBody] APIRequestGenericModel input)
        {
            try
            {
                JArray objParr = null;
                if (input != null && input.token != null && CommonHelper.GetParamWithKey(input.token, out objParr, configuration["KEY:private_key"]))
                {
                    int parent_id = Convert.ToInt32(objParr[0]["parent_id"]);

                    var group_product = await _newsBusiness.GetArticleCategoryByParentID(parent_id);
                    if(group_product!=null)
                    {
                        return Ok(new
                        {
                            status = group_product.Count > 0 ? (int)ResponseType.SUCCESS : (int)ResponseType.EMPTY,
                            data = group_product
                        });
                    }
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY,
                        msg = "category child empty"
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
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "NewsController - GetAllCategory: " + ex + "\n Token: " + input.token);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Error: " + ex.ToString(),
                });
            }
        }

    }
}
