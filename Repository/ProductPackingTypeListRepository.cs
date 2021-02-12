using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
  public  class ProductPackingTypeListRepository : RepositoryBase<ProductPackingTypeList>, IProductPackingTypeListRepository
    {
      public ProductPackingTypeListRepository(BaseContext repositoryContext)
          : base(repositoryContext)
      {
      }
    }
}
