using HuloToys_Service.RedisWorker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HuloToys_Service.Controllers.Files
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RedisConn _redisService;

        public FileController(IConfiguration configuration, RedisConn redisService)
        {

            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
