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

			// Properties
			entity.HasKey(e => e.DepartmentId);
			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(255);

			// Relationships
			entity.HasMany(a => a.Employees)
				.WithOne(b => b.Department)
				.HasForeignKey(c => c.DepartmentId)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
