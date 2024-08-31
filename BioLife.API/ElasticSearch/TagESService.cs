using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class TagESService : ESRepository<TagViewModel>
    {
        public string index = "tag_biolife_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public TagESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
            index = _configuration["DataBaseConfig:Elastic:Index:Tag"];
        }
        public List<TagViewModel> GetListTag()
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<TagESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q.MatchAll()
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<TagESModel>;
                    var result = data.Select(a => new TagViewModel
                    {
                        Id = a.id,
                        CreatedOn = a.createdon,
                        TagName = a.tagname,
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
        public List<TagViewModel> GetListTagByTagName(string TagName)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<TagESModel>(sd => sd
                               .Index(index)
                               .Size(4000)
                               .Query(q => q.
                                        QueryString(m => m.Fields("tagname").Query(TagName.ToLower().Replace("#", "").ToString()))
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<TagESModel>;
                    var result = data.Select(a => new TagViewModel
                    {
                        Id = a.id,
                        CreatedOn = a.createdon,
                        TagName = a.tagname,
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
        public TagViewModel GetListTagById(long id)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<TagESModel>(sd => sd
                               .Index(index)
                               .Query(q => q.
                                        Match(m => m.Field("id").Query(id.ToString()))
                               ));

                if (query.IsValid)
                {
                    var data = query.Documents as List<TagESModel>;
                    var result = data.Select(a => new TagViewModel
                    {
                        Id = a.id,
                        CreatedOn = a.createdon,
                        TagName = a.tagname,
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
