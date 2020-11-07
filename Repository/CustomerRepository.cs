﻿using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
  public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
      public CustomerRepository(BaseContext repositoryContext)
          : base(repositoryContext)
      {
      }
    }
}
