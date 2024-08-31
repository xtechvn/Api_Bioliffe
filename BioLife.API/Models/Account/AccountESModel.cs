using System;
using System.Collections.Generic;


namespace HuloToys_Service.Models.Account
{
    public partial class AccountESModel
    {
        public int id { get; set; }
        public long? clientid { get; set; }
        public int? clienttype { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string passwordbackup { get; set; }
        public string forgotpasswordtoken { get; set; }
        public byte? status { get; set; }
        public int? grouppermission { get; set; }
    }
    public partial class AccountApiESModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public byte? status { get; set; }

    }
}
