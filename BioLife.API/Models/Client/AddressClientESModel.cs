using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class AddressClientESModel
    {
        public long id { get; set; }
        public long clientid { get; set; }
        public string receivername { get; set; }
        public string phone { get; set; }
        public string provinceid { get; set; }
        public string districtid { get; set; }
        public string wardid { get; set; }
        public string address { get; set; }
        public int? status { get; set; }
        public bool isactive { get; set; }
        public DateTime? createdon { get; set; }
        public DateTime? updatetime { get; set; }
    }
}
