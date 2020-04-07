using CompanyWebApi.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyWebApi.Persistence.Configurations
{
	public class EmployeeAddressConfiguration
	{
		public EmployeeAddressConfiguration(EntityTypeBuilder<EmployeeAddress> entity)
		{
			// Table
			entity.ToTable("EmployeeAddresses");

			// Keys, Properties
			entity.HasKey(e => e.EmployeeId);
			entity.Property(e => e.Address)
				.IsRequired()
				.HasMaxLength(255);

			// Relationships
			entity.HasOne(a => a.Employee)
				.WithOne(b => b.EmployeeAddress)
				.HasForeignKey<EmployeeAddress>("EmployeeId")
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
