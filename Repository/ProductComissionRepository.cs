using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
  public class ProductComissionRepository : RepositoryBase<ProductComission>, IProductComissionRepository
    {
      public ProductComissionRepository(BaseContext repositoryContext)
          : base(repositoryContext)
      {
      }
    }
}
