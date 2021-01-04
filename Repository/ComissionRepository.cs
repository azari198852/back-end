using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class ComissionRepository : RepositoryBase<Comission>, IComissionRepository
    {
        public ComissionRepository(BaseContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
