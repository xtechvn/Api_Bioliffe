using Entities.ViewModels.Products;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.MongoDb
{
    public class OrderDetailMongoDbModel
    {
        [BsonElement("_id")]
        public string _id { get; set; }
        public void GenID()
        {
            _id = ObjectId.GenerateNewId(DateTime.Now).ToString();
        }
        public long account_client_id { get; set; }
        public int payment_type { get; set; }
        public int delivery_type { get; set; }
        public long order_id { get; set; }
        public string order_no { get; set; }
       
        public double total_amount { get; set; }
        public double? total_price { get; set; }
        public double? total_profit { get; set; }
        public double? total_discount { get; set; }
        public List<CartItemMongoDbModel> carts { get; set; }
        public string utm_source { get; set; }
        public string utm_medium { get; set; }

        public int voucher_id { get; set; }

    }
}
