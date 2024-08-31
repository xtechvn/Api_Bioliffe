using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace HuloToys_Service.Models.Client
{
    public partial class ClientESModel
    {


        public long id { get; set; }
        public int? clientmapid { get; set; }
        public int? salemapid { get; set; }
        public int? clienttype { get; set; }
        public string clientname { get; set; }
        public string email { get; set; }
        public int? gender { get; set; }
        public int status { get; set; }
        public string note { get; set; }
        public string avartar { get; set; }
        public DateTime joindate { get; set; }
        public bool? isreceiverinfoemail { get; set; }
        public string phone { get; set; }
        public DateTime? birthday { get; set; }
        public DateTime? upDateTime { get; set; }
        public string taxno { get; set; }
        public int? agencytype { get; set; }
        public int? permisiontype { get; set; }
        public string businessaddress { get; set; }
        public string exportbilladdress { get; set; }
        public string clientcode { get; set; }
        public bool? isregisteraffiliate { get; set; }
        public string referralid { get; set; }
        public int? parentid { get; set; }

    }
}
