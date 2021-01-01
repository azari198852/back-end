﻿using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductOffer
    {
        public long Id { get; set; }
        public long? OfferId { get; set; }
        public long? ProductId { get; set; }
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public double? Value { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual Product Product { get; set; }
    }
}
