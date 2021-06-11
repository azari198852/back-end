using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
  public class OfferInsertDto
    {
        public long Id { get; set; }
        public long? OfferTypeId { get; set; }
        public long? LanguageId { get; set; }
        public string Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? Value { get; set; }
        public string OfferCode { get; set; }
        public long? MaximumPrice { get; set; }
        public string Description { get; set; }
        public bool? HaveTimer { get; set; }
        public List<long> ProductIdList { get; set; }

    }
}
