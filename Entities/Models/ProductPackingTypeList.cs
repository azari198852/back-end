using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductPackingTypeList
    {
        public ProductPackingTypeList()
        {
            ProductPackingType = new HashSet<ProductPackingType>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual ICollection<ProductPackingType> ProductPackingType { get; set; }
    }
}
