using Models.MongoDb;
using HuloToys_Service.Utilities.Lib;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Reflection;
using Nest;

namespace HuloToys_Service.MongoDb
{
    public class OrderMongodbService
    {
        private readonly IConfiguration _configuration;
        private IMongoCollection<OrderDetailMongoDbModel> bookingCollection;

        public OrderMongodbService(IConfiguration configuration) {

            _configuration= configuration;
            //      "connection_string": "mongodb://adavigolog_writer:adavigolog_2022@103.163.216.42:27017/?authSource=Adavigo"
            string _connection = "mongodb://" + _configuration["DataBaseConfig:MongoServer:user"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:pwd"]
                 + "@" + _configuration["DataBaseConfig:MongoServer:Host"]
                 + ":" + _configuration["DataBaseConfig:MongoServer:Port"]
                 + "/?authSource=" + _configuration["DataBaseConfig:MongoServer:catalog_log"];
            var booking = new MongoClient(_connection);
            IMongoDatabase db = booking.GetDatabase(_configuration["DataBaseConfig:MongoServer:catalog_log"]);
            bookingCollection = db.GetCollection<OrderDetailMongoDbModel>("Orders");
        }
        public async Task<string> Insert(OrderDetailMongoDbModel item)
        {
            try
            {
                item.GenID();
                await bookingCollection.InsertOneAsync(item);
                return item._id;
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }
      
        public async Task<OrderDetailMongoDbModel> FindById(string id)
        {
            try
            {
                var filter = Builders<OrderDetailMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<OrderDetailMongoDbModel>.Filter.Eq(x => x._id, id);

                return await bookingCollection.Find(filterDefinition).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }
        public async Task<List<OrderDetailMongoDbModel>> GetListByOrderId(List<long> ids)
        {
            try
            {
                var filter = Builders<OrderDetailMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<OrderDetailMongoDbModel>.Filter.In(x => x.order_id, ids);

                return await bookingCollection.Find(filterDefinition).ToListAsync();
               
            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return null;

        }

        public async Task<bool> Delete(string id)
        {
            try
            {

                var filter = Builders<OrderDetailMongoDbModel>.Filter;
                var filterDefinition = filter.Empty;
                filterDefinition &= Builders<OrderDetailMongoDbModel>.Filter.Eq(x => x._id, id);

                var model = await bookingCollection.DeleteOneAsync(filterDefinition);
                return model.DeletedCount > 0;

            }
            catch (Exception ex)
            {
                string error_msg = Assembly.GetExecutingAssembly().GetName().Name + "->" + MethodBase.GetCurrentMethod().Name + "=>" + ex.Message;
                LogHelper.InsertLogTelegramByUrl(_configuration["telegram:log_try_catch:bot_token"], _configuration["telegram:log_try_catch:group_id"], error_msg);
            }
            return false;
        }
    }
}
