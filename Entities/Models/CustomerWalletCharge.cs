using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class CustomerWalletCharge
    {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public long? ChargeDate { get; set; }
        public long? ChargePrice { get; set; }
        public long? FinalStatusId { get; set; }
        public string BankCard { get; set; }
        public string TraceNo { get; set; }
        public long? OrderNo { get; set; }
        public string RefNum { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
