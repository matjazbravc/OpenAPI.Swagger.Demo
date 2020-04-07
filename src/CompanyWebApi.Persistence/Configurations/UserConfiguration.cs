using CompanyWebApi.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyWebApi.Persistence.Configurations
{
	public class UserConfiguration
	{
		public UserConfiguration(EntityTypeBuilder<User> entity)
		{
			// Table
			entity.ToTable("Users");
			
			// Keys, Properties
			entity.HasKey(e => e.EmployeeId);
			entity.Property(e => e.Username)
				.IsRequired()
				.HasMaxLength(255);
			entity.Property(e => e.Password)
				.IsRequired()
				.HasMaxLength(255);

			// Relationships
			entity.HasOne(a => a.Employee)
				.WithOne(b => b.User)
				.HasForeignKey<User>("EmployeeId")
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
