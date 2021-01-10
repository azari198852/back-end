using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Params
{
    public class CustomerListParam
    {
        public long? CatProductID { get; set; }
        public long? CityID { get; set; }
        public long? FromPrice { get; set; }
        public long? ToPrice { get; set; }
        public int? FromBirth { get; set; }
        public int? ToBirth { get; set; }
        public bool? HaveCard { get; set; }
        public bool? HaveWallet { get; set; }
        public DateTime? BuyDateFrom { get; set; }
        public DateTime? BuyDateTo { get; set; }
    }
}
