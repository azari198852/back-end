using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class DynamiFormImageDto
    {
        public long Id { get; set; }
        public long? DynamicFormId { get; set; }
        public string ImageUrl { get; set; }
    }
}
