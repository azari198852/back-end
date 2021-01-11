using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class CountryInsertDto
    {

        public string Name { get; set; }
        public string EnName { get; set; }
        public long? LocationCode { get; set; }
        public long? PostCode { get; set; }


    }
}
