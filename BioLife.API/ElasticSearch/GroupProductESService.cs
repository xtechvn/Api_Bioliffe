using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Models.Products;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Drawing;
using System.Reflection;
using Utilities.Contants;

namespace HuloToys_Service.ElasticSearch
{
    public class GroupProductESService : ESRepository<GroupProduct>
    {
        public string index = "";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;
        private static ElasticClient _elasticClient;

        public GroupProductESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:GroupProduct"];

        }
        /// <summary>
        /// Lấy ra các chuyên mục con theo chuyên mục cha
        /// </summary>
        /// <param name="parent_id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<GroupProductModel> GetListGroupProductByParentId(long parent_id, int size = 10)
        {
            try
            {
                if (_elasticClient == null)
                {
                    var nodes = new Uri[] { new Uri(_ElasticHost) };
                    var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
                    var connectionSettings = new ConnectionSettings(connectionPool)
                        .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
                        .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
                        .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
                        .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn

                    _elasticClient = new ElasticClient(connectionSettings);

                }

                // Thực hiện truy vấn với ElasticClient
                var query = _elasticClient.Search<GroupProductModel>(sd => sd
                    .Index(index)
                    .Size(size)
                    .Query(q => q
                        .Bool(b => b
                            .Must(
                                m => m.Term(t => t.Field("status").Value(ArticleStatus.PUBLISH.ToString())),
                                m => m.Term(t => t.Field("parentid").Value(parent_id))
                            )
                        )
                    )
                );

                // Kiểm tra nếu query hợp lệ
                if (query.IsValid)
                {
                    var data = query.Documents.ToList();
                    return data;
                }
                else
                {
                    // Ghi log nếu truy vấn không thành công
                    LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "Query Invalid: " + query.DebugInformation);
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }

            return null;
        }
        public GroupProductModel GetDetailGroupProductById(long id)
        {
            try
            {
                if (_elasticClient == null)
                {
                    var nodes = new Uri[] { new Uri(_ElasticHost) };
                    var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
                    var connectionSettings = new ConnectionSettings(connectionPool)
                        .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
                        .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
                        .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
                        .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn

                    _elasticClient = new ElasticClient(connectionSettings);

                }

                var query = _elasticClient.Search<GroupProductModel>(sd => sd
                               .Index(index)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("status").Query(ArticleStatus.PUBLISH.ToString())),
                                   sh => sh.Match(m => m.Field("id").Query(id.ToString())
                                   )
                                   )
                               )
                          ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<GroupProductModel>;
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }

        
    }
}
