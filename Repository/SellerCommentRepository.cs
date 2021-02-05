﻿using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class SellerCommentRepository : RepositoryBase<SellerComment>, ISellerCommentRepository
    {
        public SellerCommentRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
