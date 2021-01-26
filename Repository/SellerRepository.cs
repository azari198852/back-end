using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class SellerRepository : RepositoryBase<Seller>, ISellerRepository
    {
        public SellerRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public long GetSellerIdByUserId(long userId)
        {

            return RepositoryContext.Seller.Where(c => c.UserId == userId).Select(c => c.Id).FirstOrDefault();
        }
    }
}
