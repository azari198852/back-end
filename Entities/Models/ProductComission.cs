using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public partial class ProductComission
    {
        public long Id { get; set; }
        public long? ComissionId { get; set; }
        public long? ProductId { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Comission Comission { get; set; }
        public virtual Product Product { get; set; }
    }
}
