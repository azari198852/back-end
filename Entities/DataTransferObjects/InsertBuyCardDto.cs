using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class InsertBuyCardDto
    {

        public string Name { get; set; }
        public long? Rkey { get; set; }
        public long? Price { get; set; }
        public long? FirstCount { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
    }
}
