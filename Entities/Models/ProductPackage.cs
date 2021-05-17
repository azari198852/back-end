using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductPackage
    {
        public long Id { get; set; }
        public long? MainProductId { get; set; }
        public long? DepProductId { get; set; }
        public long? Price { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Product DepProduct { get; set; }
        public virtual Product MainProduct { get; set; }
    }
}
