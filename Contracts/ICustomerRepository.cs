using System;
using System.Collections.Generic;
using System.Text;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Params;

namespace Contracts
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        List<CustomerLIstByFilterDto> GetCustomerListByFilter(CustomerListParam input);
    }
}
