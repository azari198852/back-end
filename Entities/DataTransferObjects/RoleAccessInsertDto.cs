using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class RoleAccessInsertDto
    {
        public long RoleId { get; set; }
        public List<RoleFormDto> FormList { get; set; }
    }
}
