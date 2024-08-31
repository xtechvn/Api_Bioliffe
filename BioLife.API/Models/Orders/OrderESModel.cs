using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuloToys_Service.Models.Orders
{
    public class OrderESModel
    {
        public long orderid { get; set; }

        public long clientid { get; set; }

        public string orderno { get; set; }

        public DateTime createddate { get; set; }

        public DateTime? updatelast { get; set; }

        public double? price { get; set; }

        public double? profit { get; set; }

        public double? discount { get; set; }

        public double? amount { get; set; }

        public int status { get; set; }

        public short paymenttype { get; set; }

        public int paymentstatus { get; set; }

        public string utmsource { get; set; }

        public string utmmedium { get; set; }
        public string note { get; set; }

        public int? voucherid { get; set; }

        public int? isdelete { get; set; }

        public int? userid { get; set; }

        public string usergroupids { get; set; }
    }
}
