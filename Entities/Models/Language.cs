using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Language
    {
        public Language()
        {
            CatProduct = new HashSet<CatProduct>();
            CatProductLanguage = new HashSet<CatProductLanguage>();
            DynamicForms = new HashSet<DynamicForms>();
            FamousComments = new HashSet<FamousComments>();
            Package = new HashSet<Package>();
            PackingType = new HashSet<PackingType>();
            Parameters = new HashSet<Parameters>();
            PaymentType = new HashSet<PaymentType>();
            Product = new HashSet<Product>();
            ProductLanguage = new HashSet<ProductLanguage>();
            Slider = new HashSet<Slider>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string FlagIcon { get; set; }
        public string ShortName { get; set; }
        public long? Rkey { get; set; }
        public bool? IsRtl { get; set; }
        public long? FinalStatusId { get; set; }
        public long? CuserId { get; set; }
        public long? Cdate { get; set; }
        public long? DuserId { get; set; }
        public long? Ddate { get; set; }
        public long? MuserId { get; set; }
        public long? Mdate { get; set; }
        public long? DaUserId { get; set; }
        public long? DaDate { get; set; }

        public virtual ICollection<CatProduct> CatProduct { get; set; }
        public virtual ICollection<CatProductLanguage> CatProductLanguage { get; set; }
        public virtual ICollection<DynamicForms> DynamicForms { get; set; }
        public virtual ICollection<FamousComments> FamousComments { get; set; }
        public virtual ICollection<Package> Package { get; set; }
        public virtual ICollection<PackingType> PackingType { get; set; }
        public virtual ICollection<Parameters> Parameters { get; set; }
        public virtual ICollection<PaymentType> PaymentType { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<ProductLanguage> ProductLanguage { get; set; }
        public virtual ICollection<Slider> Slider { get; set; }
    }
}
