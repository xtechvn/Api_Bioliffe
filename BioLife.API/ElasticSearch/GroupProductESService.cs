using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Models.Entities;
using HuloToys_Service.Models.Products;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Linq;
using System.Reflection;
using Utilities.Contants;

namespace HuloToys_Service.ElasticSearch
{
    public class GroupProductESService : ESRepository<GroupProduct>
    {
        public string index = "group_product_biolife_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public GroupProductESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:GroupProduct"];

        }
        public List<GroupProduct> GetListGroupProductByParentId(long parent_id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<GroupProductESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  q => q.Match(m => m.Field("status").Query(ArticleStatus.PUBLISH.ToString())),
                                   sh => sh.Match(m => m.Field("parentid").Query(parent_id.ToString())
                                   )
                                   )
                               )
                          ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<GroupProductESModel>;
                    var result = data.Select(a => new GroupProduct
                    {
                        Id = a.id,
                        ParentId = a.parentid,
                        PositionId = a.positionid,
                        Name = a.name,
                        ImagePath = a.imagepath,
                        OrderNo = a.orderno,
                        Path = a.path,
                        Status = a.status,
                        CreatedOn = a.createdon,
                        ModifiedOn = a.modifiedon,
                        Description = a.description,
                        IsShowHeader = a.isshowheader,
                        IsShowFooter = a.isshowfooter,

                    }).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;
        }
        public GroupProduct GetDetailGroupProductById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<GroupProductESModel>(sd => sd
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
                    var data = query.Documents as List<GroupProductESModel>;
                    var result = data.Select(a => new GroupProduct
                    {
                        Id = a.id,
                        ParentId = a.parentid,
                        PositionId = a.positionid,
                        Name = a.name,
                        ImagePath = a.imagepath,
                        OrderNo = a.orderno,
                        Path = a.path,
                        Status = a.status,
                        CreatedOn = a.createdon,
                        ModifiedOn = a.modifiedon,
                        Description = a.description,
                        IsShowHeader = a.isshowheader,
                        IsShowFooter = a.isshowfooter,

                    }).ToList();
                    return result.FirstOrDefault();
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
