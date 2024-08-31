using HuloToys_Service.Models;

namespace HuloToys_Service.Elasticsearch
{
    public interface IESRepository<TEntity> where TEntity : class
    {
        bool DeleteProductByCode(string index_name, string document_id);
        TEntity FindById(string indexName, object value, string field_name);

    }
}
