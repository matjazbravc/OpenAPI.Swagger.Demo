using CompanyWebApi.Contracts.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CompanyWebApi.Contracts.Entities
{
    [Serializable]
    public class Employee : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        // Computed column
        public int Age => CalculateAge(BirthDate);

        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }

        // Inverse navigation property
        public Company Company { get; set; }

        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }

        // Inverse navigation property
        public Department Department { get; set; }

        // Reference navigation property
        public EmployeeAddress EmployeeAddress { get; set; }

        // Reference navigation property
        public User User { get; set; }

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