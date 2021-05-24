using CompanyWebApi.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyWebApi.Persistence.Configurations
{
	public class EmployeeConfiguration
	{
		public EmployeeConfiguration(EntityTypeBuilder<Employee> entity)
		{
			// Table
			entity.ToTable("Employees");
			
			// Keys
			entity.HasKey(e => e.EmployeeId);
			
            // Properties
            entity.Property(e => e.FirstName)
				.IsRequired();
			entity.Property(e => e.LastName)
				.IsRequired();

            // Indexes
            entity.HasIndex(b => b.FirstName);
			entity.HasIndex(b => b.LastName);
            entity.HasIndex(b => b.BirthDate);
			
			// Relationships
			entity.HasOne(a => a.Department)
                .WithMany(b => b.Employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
