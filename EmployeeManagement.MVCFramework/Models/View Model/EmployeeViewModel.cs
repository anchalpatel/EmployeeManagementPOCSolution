using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace EmployeeManagement.MVCFramework.Models.View_Model
{
    public class EmployeeViewModel
    {
        [HiddenInput]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter correct email format")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(10, ErrorMessage = "Phone Number cannot be larger than 10 digits")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be larger than 10 digits")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }

        [HiddenInput]
        public DateTime? CreatedAt { get; set; }

        [HiddenInput]
        public string OrganizationId { get; set; } = null;

        [HiddenInput]
        public string UserId { get; set; } = null;

        [HiddenInput]
        public string CreatedBy { get; set; } = null;
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}