using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public partial class Comission
    {
        public Comission()
        {
            ProductComission = new HashSet<ProductComission>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public int? Value { get; set; }
        public bool? SendSms { get; set; }
        public bool? SendEmail { get; set; }
        public long? FinalStatusId { get; set; }
        public string Description { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual ICollection<ProductComission> ProductComission { get; set; }
    }
}
