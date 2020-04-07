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
			
			// Properties
			entity.HasKey(e => e.EmployeeId);
			entity.Property(e => e.FirstName)
				.IsRequired()
				.HasMaxLength(40);
			entity.Property(e => e.LastName)
				.IsRequired()
				.HasMaxLength(20);
		}
	}
}
