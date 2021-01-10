using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Params;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Repository
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {
        }


        public List<CustomerLIstByFilterDto> GetCustomerListByFilter(CustomerListParam input)
        {

            var list = RepositoryContext.Customer.Select(c => c.Id);

            if (input.CatProductID.HasValue)
            {


                var list1 = RepositoryContext.CustomerOrderProduct
                    .Where(c => (c.Product.CatProductId == input.CatProductID || !input.CatProductID.HasValue) &&
                                c.DaDate == null & c.Ddate == null)
                    .Select(c => c.CustomerOrder.CustomerId.Value);
                list = list.Intersect(list1);
            }

            if (input.HaveCard.HasValue)
            {
                var list1 = RepositoryContext.CustomerOrderProduct
                    .Where(c => (c.Product.CatProduct.Rkey == 1 || !input.HaveCard.HasValue) &&
                                c.DaDate == null & c.Ddate == null)
                    .Select(c => c.CustomerOrder.CustomerId.Value);
                list = list.Intersect(list1);

            }

            if (input.CityID.HasValue)
            {
                var list1 = RepositoryContext.CustomerAddress
                    .Where(c => (c.CityId == input.CityID || !input.CityID.HasValue) && c.DaDate == null & c.Ddate == null).Select(c => c.CustomerId.Value).ToList();
                list = list.Intersect(list1);
            }


            if (input.ToPrice.HasValue || input.FromPrice.HasValue || input.BuyDateFrom.HasValue ||
                input.BuyDateTo.HasValue)
            {
                var list1 = RepositoryContext.CustomerOrder
                    .Where(c => (c.FinalPrice <= input.ToPrice || !input.ToPrice.HasValue) &&
                                (c.FinalPrice >= input.FromPrice || !input.FromPrice.HasValue) &&
                                (!input.BuyDateFrom.HasValue || c.OrderDate >= input.BuyDateFrom.Value.Ticks) &&
                                (!input.BuyDateTo.HasValue || c.OrderDate >= input.BuyDateTo.Value.Ticks) && c.DaDate == null & c.Ddate == null)
                    .Select(c => c.CustomerId.Value).ToList();
                list = list.Intersect(list1);
            }

           
       
            var currentYear = new System.Globalization.PersianCalendar().GetYear(DateTime.Now);
            long? minYear = null;
            long? maxYear = null;
            if (input.FromBirth.HasValue)
            {
                minYear = ((new PersianCalendar().ToDateTime(currentYear, 1, 1, 0, 0, 0, 0)).AddYears(input.FromBirth.Value)).Ticks;
            }

            if (input.ToBirth.HasValue)
            {
                maxYear = (new PersianCalendar().ToDateTime(currentYear, 12, 29, 0, 0, 0, 0).AddYears(input.ToBirth.Value)).Ticks;
            }




            var res = RepositoryContext.Customer
                 .Where(c =>
                (list.Contains(c.Id)) &&
                (!input.HaveWallet.HasValue || input.HaveWallet == c.HaveWallet) &&
                (minYear <= c.Bdate || minYear == null) &&
                (maxYear >= c.Bdate || maxYear == null) &&
                c.Ddate == null)
                .Select(c => new CustomerLIstByFilterDto
                {
                    CustomerId = c.Id,
                    Name = c.Name,
                    Fname = c.Fname,
                    WalletFinalPrice = c.WalletFinalPrice,
                    OrderCount = c.CustomerOrder.SelectMany(xc => xc.CustomerOrderPayment.Where(v => v.FinalStatusId == 25)).ToList().Count,
                    CatProduct = c.CustomerOrder.SelectMany(x => x.CustomerOrderProduct.Select(v => v.Product.CatProduct.Name)).ToList()
                }).ToList();

            var a = new List<CustomerLIstByFilterDto>();
            foreach (var item in res)
            {
                var ss = new CustomerLIstByFilterDto
                {
                    CatProduct = item.CatProduct.Distinct().ToList(),
                    Name = item.Name,
                    Fname = item.Fname,
                    CustomerId = item.CustomerId,
                    WalletFinalPrice = item.WalletFinalPrice,
                    OrderCount = item.OrderCount
                };
                a.Add(ss);

            }
            return a;

        }

    }
}
