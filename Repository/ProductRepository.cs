﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {

        }

        public IQueryable<Product> GetProductListFullInfo()
        {
            var res = FindByCondition(c => c.Ddate == null && c.DaDate == null && c.FinalStatusId == 8)
                  .Include(c => c.CatProduct)
                  .Include(c => c.FinalStatus)
                  .Include(c => c.Seller)
                  .Include(c => c.ProductCustomerRate)
                  .Include(c => c.ProductOffer).ThenInclude(c => c.Offer).AsQueryable();
            return res;

        }

    }
}
