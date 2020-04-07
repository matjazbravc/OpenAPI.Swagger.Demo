using CompanyWebApi.Contracts.Entities.Base;
using CompanyWebApi.Contracts.Entities;
using CompanyWebApi.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace CompanyWebApi.Persistence.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
	    public ApplicationDbContext(DbContextOptions options) 
		    : base(options)
        {
        }

		public DbSet<Company> Companies { get; set; }

	    public DbSet<Department> Departments { get; set; }

	    public DbSet<Employee> Employees { get; set; }

	    public DbSet<EmployeeAddress> EmployeeAddresses { get; set; }

	    public DbSet<User> Users { get; set; }

		public override int SaveChanges()
		{
			TrackChanges();
			return base.SaveChanges();
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Create EF entities and relations
            new CompanyConfiguration(modelBuilder.Entity<Company>());
	        new EmployeeConfiguration(modelBuilder.Entity<Employee>());
			new EmployeeAddressConfiguration(modelBuilder.Entity<EmployeeAddress>());
	        new DepartmentConfiguration(modelBuilder.Entity<Department>());
	        new UserConfiguration(modelBuilder.Entity<User>());
        }

        /// <summary>
        /// Automatic change tracking
        /// </summary>
        private void TrackChanges()
        {
            var entries = ChangeTracker.Entries()
	            .Where(x => x.Entity is IBaseAuditEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
	            if (entry.State == EntityState.Added)
                {
                    ((IBaseAuditEntity)entry.Entity).Created = DateTime.UtcNow;
                }
                ((IBaseAuditEntity)entry.Entity).Modified = DateTime.UtcNow;
            }
        }
    }
}
