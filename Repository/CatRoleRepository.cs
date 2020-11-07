﻿using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
   public class CatRoleRepository : RepositoryBase<CatRole>, ICatRoleRepository
    {
       public CatRoleRepository(BaseContext repositoryContext)
           : base(repositoryContext)
       {
       }
    }
}
