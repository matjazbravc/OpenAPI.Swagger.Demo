using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using CompanyWebApi.Contracts.Entities.Base;
using Newtonsoft.Json;

namespace CompanyWebApi.Contracts.Entities
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    [JsonObject(IsReference = false)]
    public class Employee : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        // Computed column
        public int Age => CalculateAge(BirthDate);

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }

        // Navigation property
        public Company Company { get; set; }

        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }

        // Navigation property
        public Department Department { get; set; }

        // Navigation property
        public EmployeeAddress EmployeeAddress { get; set; }

        public User User { get; set; }

        public override string ToString() => $"{EmployeeId}, {FirstName} {LastName}";

        private static int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
            {
                return -1;
            }
            var age = DateTime.Today.Year - birthDate.Value.Year;
            if (DateTime.Today.Month < birthDate.Value.Month ||
                DateTime.Today.Month == birthDate.Value.Month && DateTime.Today.Day < birthDate.Value.Day)
            {
                age--;
            }
            return age;
        }
    }
}