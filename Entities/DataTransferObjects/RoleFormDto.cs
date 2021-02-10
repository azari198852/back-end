using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class RoleFormDto
    {
        public long? FormsId { get; set; }
        public bool? InsertRule { get; set; }
        public bool? UpdateRule { get; set; }
        public bool? DeleteRule { get; set; }
        public bool? ViewRule { get; set; }
    }
}
