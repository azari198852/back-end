using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class InsertPackageDto
    {
        public long? PackageId { get; set; }
        public long? LanguageId { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public string KeyWord { get; set; }
        public string MetaDesc { get; set; }
        public string MetaTitle { get; set; }
        public List<PackageProductsWithPrice> ProductWithPriceList { get; set; }

    }

    /// <summary>
    /// لیست آیدی محصول و قیمت
    /// </summary>
    public class PackageProductsWithPrice
    {
        public long ProductId { get; set; }
        public long Price { get; set; }
    }
}
