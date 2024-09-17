using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Drawing;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class ArticleESService : ESRepository<ArticleModel>
    {
        public string index = "";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;
        private static ElasticClient elasticClient;
        public ArticleESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Article"];
        }
        /// <summary>
        /// 
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
               .Index(index)  // Chỉ mục bạn muốn tìm kiếm
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
        /// Lấy ra thông tin bài viết cho chuyên mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public CategoryArticleModel GetArticleDetailForCategoryById(long id)
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

                var query = elasticClient.Search<CategoryArticleModel>(sd => sd
               .Index(index)
               .Query(q => q
                   .Term(t => t.Field(f => f.id).Value(id))  // Tìm kiếm chính xác theo giá trị id (dạng int)
               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<CategoryArticleModel>;
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
        /// Lấy ra danh sách các bài viết mới nhất
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<CategoryArticleModel> getTopStory(int top)
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
                        .EnableHttpCompression();                 // Bật nén HTTP để truyền tải nhanh hơn

                    elasticClient = new ElasticClient(connectionSettings);
                }

                var searchResponse = elasticClient.Search<CategoryArticleModel>(s => s
                .Size(top) // Lấy ra số lượng bản ghi (ví dụ 100)
                 .Index(index)
                    .Sort(sort => sort
                        .Descending(f => f.publishdate) // Sắp xếp giảm dần theo publishdate
                    )
                );

                if (searchResponse.IsValid)
                {
                    data = searchResponse.Documents as List<CategoryArticleModel>;
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

        //public List<ArticleViewModel> GetListArticle()
        //{
        //    try
        //    {
        //        var nodes = new Uri[] { new Uri(_ElasticHost) };
        //        var connectionPool = new StaticConnectionPool(nodes);
        //        var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
        //        var elasticClient = new ElasticClient(connectionSettings);

        //        var query = elasticClient.Search<CategoryModel>(sd => sd
        //                       .Index(index)
        //                       .Size(4000)
        //                       .Query(q => q.MatchAll()
        //                       ));

        //        if (query.IsValid)
        //        {
        //            var data = query.Documents as List<CategoryModel>;
        //            var result = data.Select(a => new ArticleViewModel
        //            {

        //                Id = a.id,
        //                Title = a.title,
        //                Lead = a.lead,
        //                Body = a.body,
        //                Status = a.status,
        //                ArticleType = a.articletype,
        //                PageView = a.pageview,
        //                PublishDate = a.publishdate,
        //                AuthorId = a.authorid,
        //                Image169 = a.image169,
        //                Image43 = a.image43,
        //                Image11 = a.image11,
        //                CreatedOn = a.createdon,
        //                ModifiedOn = a.modifiedon,
        //                DownTime = a.downtime,
        //                UpTime = a.uptime,
        //                Position = a.position,

        //            }).ToList();
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }
        //    return null;
        //}
        //public List<ArticleViewModel> GetListArticlePosition()
        //{
        //    try
        //    {
        //        var nodes = new Uri[] { new Uri(_ElasticHost) };
        //        var connectionPool = new StaticConnectionPool(nodes);
        //        var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
        //        var elasticClient = new ElasticClient(connectionSettings);

        //        var query = elasticClient.Search<CategoryModel>(sd => sd
        //                       .Index(index)
        //                       .Size(4000)
        //                       .Query(q => q
        //                           .Range(m => m.Field("position").GreaterThanOrEquals(1).LessThanOrEquals(7)
        //                       )));
        //        if (query.IsValid)
        //        {
        //            var data = query.Documents as List<CategoryModel>;
        //            var result = data.Select(a => new ArticleViewModel
        //            {

        //                Id = a.id,
        //                Title = a.title,
        //                Lead = a.lead,
        //                Body = a.body,
        //                Status = a.status,
        //                ArticleType = a.articletype,
        //                PageView = a.pageview,
        //                PublishDate = a.publishdate,
        //                AuthorId = a.authorid,
        //                Image169 = a.image169,
        //                Image43 = a.image43,
        //                Image11 = a.image11,
        //                CreatedOn = a.createdon,
        //                ModifiedOn = a.modifiedon,
        //                DownTime = a.downtime,
        //                UpTime = a.uptime,
        //                Position = a.position,

        //            }).ToList();
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }
        //    return null;
        //}
        //public List<ArticleRelationModel> GetListArticleByBody(string txt_search)
        //{
        //    try
        //    {
        //        var nodes = new Uri[] { new Uri(_ElasticHost) };
        //        var connectionPool = new StaticConnectionPool(nodes);
        //        var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
        //        var elasticClient = new ElasticClient(connectionSettings);

        //        var query = elasticClient.Search<CategoryModel>(sd => sd
        //                       .Index(index)
        //                  .Query(q =>
        //                   q.Bool(
        //                       qb => qb.Must(
        //                           //q => q.Term("id", id),
        //                           sh => sh.QueryString(qs => qs
        //                           .Fields(new[] { "title", "lead", "body" })
        //                           .Query("*" + txt_search + "*")
        //                           .Analyzer("standard")

        //                    )
        //                   )
        //                       )));

        //        if (query.IsValid)
        //        {

        //            var data = query.Documents as List<CategoryModel>;
        //            var result = data.Select(a => new ArticleRelationModel
        //            {

        //                Id = a.id,
        //                Lead = a.lead,
        //                Image = a.image169 ?? a.image43 ?? a.image11,
        //                Title = a.title,
        //                publish_date = a.publishdate ?? DateTime.Now,

        //            }).ToList();
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }
        //    return null;
        //}
    }
}
