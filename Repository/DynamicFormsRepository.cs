using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
  public class DynamicFormsRepository : RepositoryBase<DynamicForms>, IDynamicFormsRepository
    {
      public DynamicFormsRepository(BaseContext repositoryContext)
          : base(repositoryContext)
      {
      }
    }
}
