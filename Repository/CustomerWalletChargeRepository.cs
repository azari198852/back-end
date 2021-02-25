using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class CustomerWalletChargeRepository : RepositoryBase<CustomerWalletCharge>, ICustomerWalletChargeRepository
    {
        public CustomerWalletChargeRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
