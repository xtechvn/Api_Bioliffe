using Elasticsearch.Net;
using HuloToys_Service.Elasticsearch;
using HuloToys_Service.Models.Account;
using HuloToys_Service.Utilities.Lib;
using Nest;
using System.Reflection;

namespace HuloToys_Service.ElasticSearch
{
    public class AccountApiESService : ESRepository<AccountApiESModel>
    {
        //public string index = "account_api_biolife_store";
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public AccountApiESService(string Host, IConfiguration _configuration) : base(Host, _configuration)
        {
            _ElasticHost = Host;
            configuration = _configuration;
        }
        public AccountESModel GetByUsername(string user_name)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var query = elasticClient.Search<AccountESModel>(sd => sd
               .Index(configuration["Elastic:Index:AccountApi"])
               .Query(q => q
                   .Term(t => t
                       .Field(f => f.username.Suffix("keyword")) // Use .keyword field for exact match
                       .Value(user_name)
                   )
               ));

                if (query.IsValid)
                {
                    var result = query.Documents as List<AccountESModel>;
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
