using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class Employee
    {
        [Key]
        [Required]
        [Column("employee_id")]
        public int EmployeeId { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }
        [MaxLength(50)]
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [MaxLength(20)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
    }
}
