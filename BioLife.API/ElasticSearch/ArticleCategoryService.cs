using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class ArticleCategoryService : ESRepository<CategoryArticleModel>
    {
        public string index = "article_category_biolife_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;
        private static ElasticClient elasticClient;
        private ISearchResponse<ArticleIdCategoryModel> search_response;

        public ArticleCategoryService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:ArticleCategory"];

        }

        /// <summary>
        /// batchSize: kích thước mỗi lô
        /// size: tổng số bản ghi tối đa cần lấy
        /// </summary>
        /// <param name="category_id"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        //public List<ArticleIdCategoryModel?> GetArticleIdByCategoryId(long category_id, int size, int batchSize = 1000)
        //{
        //    try
        //    {
        //        int _top = 0;
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

        //        var lst_article_id = new List<ArticleIdCategoryModel>();
        //        // Khởi tạo truy vấn đầu tiên với từ khóa và điều kiện lọc
        //        if (category_id > 0)
        //        {
        //            search_response = elasticClient.Search<ArticleIdCategoryModel>(s => s
        //                .Index(index)       // Chỉ mục cần tìm kiếm
        //                .Size(batchSize)         // Kích thước mỗi lô
        //                .Scroll("2m")            // Phiên scroll có thời hạn 2 phút
        //                 .Query(q => q
        //                     .Term(t => t.Field("categoryid").Value(category_id))  // Tìm kiếm theo giá trị id (dạng int)
        //                )
        //            );
        //        }
        //        else
        //        {
        //            search_response = elasticClient.Search<ArticleIdCategoryModel>(s => s
        //              .Size(batchSize) // Lấy ra số lượng bản ghi (ví dụ 100)
        //              .Index(index)
        //              .Scroll("2m")
        //              .Sort(sort => sort
        //                    .Descending(f => f.updatelast) // Sắp xếp giảm dần theo publishdate
        //                )
        //             );
        //        }
        //        // Lưu kết quả ban đầu
        //        if (search_response.IsValid && search_response.Documents.Any())
        //        {
        //            lst_article_id.AddRange(search_response.Documents);
        //        }

        //        var scrollId = search_response.ScrollId;

        //        // Tiếp tục cuộn cho đến khi không còn kết quả
        //        while (search_response.Documents.Any() && _top < size)
        //        {
        //            lst_article_id.AddRange(search_response.Documents);
        //            _top += search_response.Documents.Count;

        //            if (_top >= size)
        //            {
        //                lst_article_id = lst_article_id.Take(size).ToList();  // Dừng lại và lấy đúng 10 bản ghi
        //                break;
        //            }
        //            // Lấy thêm lô tiếp theo
        //            search_response = elasticClient.Scroll<ArticleIdCategoryModel>("2m", search_response.ScrollId);
        //        }

        //        // Dọn dẹp scroll ID
        //        elasticClient.ClearScroll(c => c.ScrollId(scrollId));

        //        return lst_article_id;
        //    }

        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }
        //    return null;
        //}

        //public int getTotalItemNewsByCategoryId(int category_id)
        //{
        //    try
        //    {
        //        int totalCount = 0;
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
        //        if (category_id > 0)
        //        {
        //            var countResponse = elasticClient.Count<ArticleIdCategoryModel>(c => c
        //            .Index(index)
        //            .Query(q => q
        //                .Term(t => t.Field("categoryid").Value(category_id))  // Tìm theo category_id
        //            ));
        //            totalCount = Convert.ToInt32(countResponse.Count);
        //        }
        //        else
        //        {
        //            var countResponse = elasticClient.Count<ArticleIdCategoryModel>(c => c.Index(index));
        //            totalCount = Convert.ToInt32(countResponse.Count);
        //        }

        //        return Convert.ToInt32(totalCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
        //    }

        //    return 0;
        //}
    }
}
