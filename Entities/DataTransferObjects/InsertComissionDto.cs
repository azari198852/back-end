using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class InsertComissionDto
    {
        public string Title { get; set; }
        public int? Value { get; set; }
        public bool SendSms { get; set; }
        public bool SendEmail { get; set; }
        public List<long> ProductIdList { get; set; }

    }
}
