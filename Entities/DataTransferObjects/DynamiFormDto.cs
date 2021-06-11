using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
   public class DynamiFormDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string TitleMetaData { get; set; }
        public string DescriptionMeta { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public long? LanguageId { get; set; }
        public List<DynamiFormImageDto> ImageList { get; set; }
    }
}
