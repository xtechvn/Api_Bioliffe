using Elasticsearch.Net;
using HuloToys_Service.Utilities.Lib;
using Nest;
using Newtonsoft.Json;
using System.Reflection;

namespace HuloToys_Service.Elasticsearch
{
    //https://www.steps2code.com/post/how-to-use-elasticsearch-in-csharp
    public class ESRepository<TEntity> : IESRepository<TEntity> where TEntity : class
    {
        private readonly IConfiguration configuration;
        private static string _ElasticHost;

        public ESRepository(string Host, IConfiguration _configuration)
        {
            _ElasticHost = Host;
            this.configuration = _configuration;
        }

        public bool DeleteProductByCode(string index_name, string document_id)
        {
            string key_es_id = string.Empty;
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
               
                var result = elasticClient.Search<object>(sd => sd
                               .Index(index_name)
                               .Query(q => q
                                   .Match(m => m.Field("product_code").Query(document_id)
                                   )));

                if (result.IsValid && result.Documents.Count > 0)
                {
                    foreach (Hit<object> hit in (IHit<object>[])result.HitsMetadata.Hits)
                    {
                        key_es_id = ((Hit<object>)((IHit<object>[])result.HitsMetadata.Hits)[0]).Id;
                        var response = elasticClient.Delete<object>(key_es_id.ToString(), d => d
                        .Index(index_name)
                        );
                    }
                    return result.IsValid;
                }
                else
                {
                    return true; // chưa có mã này
                }
            }
            catch (Exception ex)
            {
                string error = ("DeleteProductByCode key_es_id= " + key_es_id + " document_id" + document_id + " Exception" + ex.ToString());
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], MethodBase.GetCurrentMethod().Name + "=>" + error);
                return false;
            }
        }

       
        /// <summary>

        /// <summary>

        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value">Giá trị cần tìm kiếm</param>
        /// <param name="field_name">Tên cột cần search</param>
        /// <returns></returns>
        public TEntity FindById(string indexName, object value, string field_name = "id")
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var searchResponse = elasticClient.Search<object>(s => s
                    .Index(indexName)
                    .Query(q => q.Term(field_name, value))
                );

                var JsonObject = JsonConvert.SerializeObject(searchResponse.Documents);
                var ListObject = JsonConvert.DeserializeObject<List<TEntity>>(JsonObject);
                return ListObject.FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString());
                return null;
            }
        }
       

        public int UpSert(TEntity entity, string indexName)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);
                var indexResponse = elasticClient.Index(new IndexRequest<TEntity>(entity, indexName));

                if (!indexResponse.IsValid)
                {
                    LogHelper.WriteLogActivity(Directory.GetCurrentDirectory(), indexResponse.OriginalException.Message);
                    LogHelper.WriteLogActivity(Directory.GetCurrentDirectory(), "apicall" + indexResponse.ApiCall.OriginalException.Message);
                    return 0;
                }

                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString());
                return -1;
            }
        }

        public async Task<int> UpSertAsync(TEntity entity, string indexName)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("people");
                var elasticClient = new ElasticClient(connectionSettings);

                var indexResponse = elasticClient.Index(entity, i => i.Index(indexName));
                if (!indexResponse.IsValid)
                {
                    // If the request isn't valid, we can take action here
                }

                var indexResponseAsync = await elasticClient.IndexDocumentAsync(entity);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], MethodBase.GetCurrentMethod().Name + "=>" + ex.ToString());
            }
            return 0;
        }           
    }
}
