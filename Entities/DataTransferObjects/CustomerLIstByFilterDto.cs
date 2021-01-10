using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class CustomerLIstByFilterDto
    {
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public string Fname { get; set; }
        public int OrderCount { get; set; }
        public long? WalletFinalPrice { get; set; }
        public List<string> CatProduct { get; set; }
    }
}
