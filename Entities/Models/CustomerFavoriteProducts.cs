using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class CustomerFavoriteProducts
    {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public long? ProductId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
