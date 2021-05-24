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

			// Keys
			entity.HasKey(empa => empa.EmployeeId);

			// Properties
			entity.Property(empa => empa.Address)
				.IsRequired();

			// Relationships
			entity.HasOne(empa => empa.Employee)
				.WithOne(emp => emp.EmployeeAddress)
                .OnDelete(DeleteBehavior.ClientSetNull)
				.HasForeignKey<EmployeeAddress>("EmployeeId")
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
