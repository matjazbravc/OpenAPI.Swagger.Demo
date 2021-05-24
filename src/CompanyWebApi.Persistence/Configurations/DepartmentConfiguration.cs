using CompanyWebApi.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyWebApi.Persistence.Configurations
{
	public class DepartmentConfiguration
	{
		public DepartmentConfiguration(EntityTypeBuilder<Department> entity)
		{
			// Table
			entity.ToTable("Departments");

            // Keys
			entity.HasKey(dep => dep.DepartmentId);

            // Properties
			entity.Property(dep => dep.Name)
				.IsRequired();

			// Relationships
			entity.HasMany(dep => dep.Employees)
				.WithOne(emp => emp.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
				.HasForeignKey(emp => emp.DepartmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
