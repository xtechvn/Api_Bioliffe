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
        public string index = "";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public GroupProductESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:GroupProduct"];

        }
        public List<GroupProductModel> GetListGroupProductByParentId(long parent_id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<GroupProductModel>(sd => sd
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
                    var data = query.Documents as List<GroupProductModel>;
                    var result = data.Select(a => new GroupProductModel
                    {
                        id = a.id,
                        parentid = a.parentid,
                        positionid = a.positionid,
                        name = a.name,
                        imagepath = a.imagepath,
                        orderno = a.orderno,
                        path = a.path,
                        status = a.status,
                        createdon = a.createdon,
                        modifiedon = a.modifiedon,
                        description = a.description,
                        isshowheader = a.isshowheader,
                        isshowfooter = a.isshowfooter,

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
        public GroupProductModel GetDetailGroupProductById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<GroupProductModel>(sd => sd
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
                    var result = data.Select(a => new GroupProductModel
                    {
                        id = a.id,
                        parentid = a.parentid,
                        positionid = a.positionid,
                        name = a.name,
                        imagepath = a.imagepath,
                        orderno = a.orderno,
                        path = a.path,
                        status = a.status,
                        createdon = a.createdon,
                        modifiedon = a.modifiedon,
                        description = a.description,
                        isshowheader = a.isshowheader,
                        isshowfooter = a.isshowfooter,

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
