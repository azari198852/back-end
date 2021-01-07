using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class DynamicForms
    {
        public DynamicForms()
        {
            DynamiFormImage = new HashSet<DynamiFormImage>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string TitleMetaData { get; set; }
        public string DescriptionMeta { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual ICollection<DynamiFormImage> DynamiFormImage { get; set; }
    }
}
