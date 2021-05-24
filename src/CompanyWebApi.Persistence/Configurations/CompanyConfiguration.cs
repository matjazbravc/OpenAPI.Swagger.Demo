using CompanyWebApi.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyWebApi.Persistence.Configurations
{
	public class CompanyConfiguration
	{
		public CompanyConfiguration(EntityTypeBuilder<Company> entity)
		{
			// Table
			entity.ToTable("Companies");

			// Keys
			entity.HasKey(comp => comp.CompanyId);

            // Properties
			entity.Property(comp => comp.Name)
				.IsRequired();

			// Relationships
			entity.HasMany(comp => comp.Departments)
				.WithOne(dep => dep.Company)
                .OnDelete(DeleteBehavior.ClientSetNull)
				.HasForeignKey(dep => dep.CompanyId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
