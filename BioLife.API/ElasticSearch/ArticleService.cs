using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Utilities.Lib;
using Nest;
using Newtonsoft.Json;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class ArticleService : ESRepository<ArticleModel>
    {
       
        private readonly IConfiguration configuration;
        private static string _ElasticHost;
        private static ElasticClient elasticClient;
        private ISearchResponse<CategoryArticleModel> search_response;
        public ArticleService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;           
        }

        /// <summary>
        /// Lấy ra chi tiết bài viết
        /// </summary>
        /// <param name="id">articleID</param>
        /// <returns></returns>
        public ArticleModel GetArticleDetailById(long id)
        {
            try
            {
                if (elasticClient == null)
                {
                    var nodes = new Uri[] { new Uri(_ElasticHost) };
                    var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
                    var connectionSettings = new ConnectionSettings(connectionPool)
                        .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
                        .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
                        .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
                        .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn

                    elasticClient = new ElasticClient(connectionSettings);
                }

                var query = elasticClient.Search<ArticleModel>(sd => sd
                 .Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"])  // Chỉ mục bạn muốn tìm kiếm
               .Query(q => q
                   .Term(t => t.Field(f => f.id).Value(id))  // Tìm kiếm chính xác theo giá trị id (dạng int)
               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<ArticleModel>;
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

        /// <summary>
        /// Lấy ra các bài viết cho các Box theo chuyên mục.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public CategoryArticleModel GetArticleDetailForCategoryById(long id)
        //{
        //    try
        //    {
        //        if (elasticClient == null)
        //        {
        //            var nodes = new Uri[] { new Uri(_ElasticHost) };
        //            var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
        //            var connectionSettings = new ConnectionSettings(connectionPool)
        //                .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
        //                .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
        //                .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
        //                .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn

        //            elasticClient = new ElasticClient(connectionSettings);
        //        }

        //        var query = elasticClient.Search<CategoryArticleModel>(sd => sd
        //        .Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"])  // Chỉ mục bạn muốn tìm kiếm
        //       .Query(q => q
        //           .Term(t => t.Field(f => f.id).Value(id))  // Tìm kiếm chính xác theo giá trị id (dạng int)
        //       ));

        //        if (query.IsValid)
        //        {
        //            var data = query.Documents as List<CategoryArticleModel>;
        //            return data.FirstOrDefault();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }
        //    return null;
        //}

        /// <summary>

        /// ES sẽ gọi tới node được đồng bộ từ JOB push store. Store SP_GetAllArticle này sẽ Join sẵn các bảng với nhau bao gồm. category và news    
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        public List<CategoryArticleModel> getListNews(int category_id, int top)
        {
            var data = new List<CategoryArticleModel>();
            try
            {
                if (elasticClient == null)
                {
                    var nodes = new Uri[] { new Uri(_ElasticHost) };
                    var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
                    var connectionSettings = new ConnectionSettings(connectionPool)
                        .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
                        .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
                        .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
                        .EnableHttpCompression();                // Bật nén HTTP để truyền tải nhanh hơn
                        //.DisableDirectStreaming()  // Kích hoạt ghi lại luồng request/response
                        //.PrettyJson();              // Định dạng kết quả JSON cho dễ đọc

                    elasticClient = new ElasticClient(connectionSettings);
                }
                if (category_id <= 0)
                {
                    // Lấy ra toàn bộ các bài viết của các chuyên mục theo thời gian bài nào mới nhất lên đầu
                    search_response = elasticClient.Search<CategoryArticleModel>(s => s
                   .Size(top) // Lấy ra số lượng bản ghi (ví dụ 100)
                   .Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"])  // Chỉ mục bạn muốn tìm kiếm
                       .Sort(sort => sort
                           .Descending(f => f.publish_date) // Sắp xếp giảm dần theo publishdate
                       )
                   );
                }
                else
                { 
                    search_response = elasticClient.Search<CategoryArticleModel>(s => s
                        .Size(top)
                        .Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"])  // Chỉ mục muốn tìm kiếm
                        .Sort(sort => sort
                            .Descending(f => f.publish_date)
                        )
                       .Query(q => q
                            .Bool(b => b
                                .Must(m => m
                                    .Wildcard(w => w
                                        .Field(f => f.list_category_id)
                                        .Value("*"+ category_id.ToString() +"*") // Tìm các chuỗi chứa ký tự liên quan
                                    )
                                )
                            )
                        )
                    );
                }                

                if (search_response.IsValid)
                {
                    data = search_response.Documents.ToList();
                }

                return data;
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
                return data;
            }
        }

        public int getTotalItemNewsByCategoryId(int category_id)
        {
            try
            {
                int totalCount = 0;
                if (elasticClient == null)
                {
                    var nodes = new Uri[] { new Uri(_ElasticHost) };
                    var connectionPool = new SniffingConnectionPool(nodes); // Sử dụng Sniffing để khám phá nút khác trong cụm
                    var connectionSettings = new ConnectionSettings(connectionPool)
                        .RequestTimeout(TimeSpan.FromMinutes(2))  // Tăng thời gian chờ nếu cần
                        .SniffOnStartup(true)                     // Khám phá các nút khi khởi động
                        .SniffOnConnectionFault(true)             // Khám phá lại các nút khi có lỗi kết nối
                        .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn


                    elasticClient = new ElasticClient(connectionSettings);

                }
                if (category_id > 0)
                {
                    var countResponse = elasticClient.Count<CategoryArticleModel>(c => c
                    .Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"])  // Chỉ mục bạn muốn tìm kiếm
                    .Query(q => q
                        .Term(t => t.Field("categoryid").Value(category_id))  // Tìm theo category_id
                    ));
                    totalCount = Convert.ToInt32(countResponse.Count);
                }
                else
                {
                    var countResponse = elasticClient.Count<CategoryArticleModel>(c => c.Index(configuration["DataBaseConfig:Elastic:Index:SpGetArticle"]));
                    totalCount = Convert.ToInt32(countResponse.Count);
                }

                return Convert.ToInt32(totalCount);
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }

            return 0;
        }
    }
}
