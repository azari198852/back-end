﻿using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
   public class TablesServiceDiscoveryRepository : RepositoryBase<TablesServiceDiscovery>, ITablesServiceDiscoveryRepository
    {
       public TablesServiceDiscoveryRepository(BaseContext repositoryContext)
           : base(repositoryContext)
       {
       }
    }
}
