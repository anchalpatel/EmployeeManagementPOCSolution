using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Core.DTO
{
    public class EmployeeDTO
    {
       
        public int Id {  get; set; }
        [Required] 
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;
        public int OrganizationId { get; set; }
        
        public string? userId { get; set; }
       
        public string? CreatedBy { get; set; }

        public DateTime CreareAt { get; set; }

        public DateTime UpdateAt { get; set; }
        public bool? IsDeleted { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
       
        public List<string>? Roles { get; set; } = new List<string>();
    }
}
