using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class ProductPackingListInsertDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long PackingTypeId { get; set; }
        public List<long> ProductIdList { get; set; }
    }
}
