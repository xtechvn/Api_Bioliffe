using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.APIRequest
{
    public class CartConfirmRequestModel
    {
        public long account_client_id { get; set; }
        public int payment_type { get; set; }
        public int delivery_type { get; set; }
        public List<CartConfirmItemRequestModel> carts { get; set; }
    }
    public class CartConfirmItemRequestModel
    {
        public string id { get; set; }
        public int quanity { get; set; }
    }
}
