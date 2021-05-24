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
			
			// Keys 
			entity.HasKey(e => e.EmployeeId);

			// Properties
			entity.Property(e => e.Username)
				.IsRequired();

			entity.Property(e => e.Password)
				.IsRequired();

            // Indexes
            entity.HasIndex(b => b.Username)
                .IsUnique();

			// Relationships
			entity.HasOne(a => a.Employee)
				.WithOne(b => b.User)
				.HasForeignKey<User>("EmployeeId")
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
