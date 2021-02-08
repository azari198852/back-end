using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class PackingTypeInsertDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? Price { get; set; }
        public long? Weight { get; set; }
    }
}
