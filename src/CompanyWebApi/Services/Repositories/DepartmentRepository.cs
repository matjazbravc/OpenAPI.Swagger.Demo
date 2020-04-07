﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.DbContexts;
using CompanyWebApi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyWebApi.Services.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext appDbContext) : base(appDbContext)
        {
        }

        // https://www.learnentityframeworkcore.com/dbset/querying-data
        public override async Task<Department> GetSingleAsync(Expression<Func<Department, bool>> predicate, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            var company = await _databaseSet
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            return company;
        }

        public override async Task<IList<Department>> GetAllAsync(bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<Department> query = _databaseSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            var result = await query
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.EmployeeAddress)
                .Include(cmp => cmp.Employees).ThenInclude(emp => emp.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
