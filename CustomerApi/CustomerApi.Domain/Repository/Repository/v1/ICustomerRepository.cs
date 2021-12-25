﻿using CustomerApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApi.Domain.NewFolder.Repository.v1
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
