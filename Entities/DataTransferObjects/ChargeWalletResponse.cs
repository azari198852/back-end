using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class ChargeWalletResponse
    {
        public long? CustomerWalletChargeId { get; set; }
        public long? OrderNo { get; set; }
        public bool RedirectToBank { get; set; }
        public string BankUrl { get; set; }
    }
}
