using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
   public class CustomerFavoriteProductsRepository : RepositoryBase<CustomerFavoriteProducts>, ICustomerFavoriteProductsRepository
    {
       public CustomerFavoriteProductsRepository(BaseContext repositoryContext)
           : base(repositoryContext)
       {
       }
    }
}
