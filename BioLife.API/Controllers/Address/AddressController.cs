using HuloToys_Service.Models;
using HuloToys_Service.Models.Address;
using HuloToys_Service.Models.Queue;
using HuloToys_Service.RabitMQ;
using HuloToys_Service.RedisWorker;
using HuloToys_Service.Utilities.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utilities;
using Utilities.Contants;

namespace HuloToys_Service.Controllers.Address
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly RedisConn redisService;
        public AddressController(IConfiguration _configuration, RedisConn _redisService)
        {
            configuration = _configuration;
            redisService = _redisService;
        }
        [HttpGet("insert-address")]

        public async Task<IActionResult> insertAddress(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<AddressViewModel>(objParr[0].ToString());
                    bool response_queue = false;
                    var work_queue = new WorkQueueClient(configuration);
                    
                    var address_model = JsonConvert.SerializeObject(request);
                    if (address_model != null)
                    {

                        var j_param = new Dictionary<string, object>
                    {
                        {"data_push", JsonConvert.SerializeObject(address_model)}, // có thể là json
                        {"type",QueueType.ADD_ADDRESS}
                    };
                        var _data_push = JsonConvert.SerializeObject(j_param);

                        // Execute Push Queue

                        response_queue = work_queue.InsertQueueSimple( _data_push, QueueName.queue_app_push);
                        if (response_queue)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Success"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "FAILED"
                            });
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });
        }
        [HttpGet("update-address")]
        public async Task<IActionResult> updateAddress(string token)
        {
            try
            {
                JArray objParr = null;
                if (CommonHelper.GetParamWithKey(token, out objParr, configuration["KEY:private_key"]))
                {
                    var request = JsonConvert.DeserializeObject<AddressViewModel>(objParr[0].ToString());
                    bool response_queue = false;
                    var work_queue = new WorkQueueClient(configuration);
                    var queue_setting = new QueueSettingViewModel
                    {
                        host = configuration["Queue:Host"],
                        v_host = configuration["Queue:V_Host"],
                        port = Convert.ToInt32(configuration["Queue:Port"]),
                        username = configuration["Queue:Username"],
                        password = configuration["Queue:Password"]
                    };
                    var address_model = JsonConvert.SerializeObject(request);
                    if (address_model != null)
                    {

                        var j_param = new Dictionary<string, object>
                    {
                        {"data_push", JsonConvert.SerializeObject(address_model)}, // có thể là json
                        {"type",QueueType.UPDATE_ADDRESS}
                    };
                        var _data_push = JsonConvert.SerializeObject(j_param);

                        // Execute Push Queue

                        response_queue = work_queue.InsertQueueSimple(_data_push, QueueName.queue_app_push);
                        if (response_queue)
                        {

                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Success"
                            });

                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "FAILED"
                            });

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = ResponseMessages.DataInvalid
            });
        }
    }
}
