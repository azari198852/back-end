﻿using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class SellerAddress
    {
        public long Id { get; set; }
        public long? SellerId { get; set; }
        public string Titel { get; set; }
        public string Address { get; set; }
        public string Xgps { get; set; }
        public string Ygps { get; set; }
        public long? ProvinceId { get; set; }
        public long? CityId { get; set; }
        public long? PostalCode { get; set; }
        public long? Tel { get; set; }
        public long? Fax { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Seller Seller { get; set; }
    }
}
