using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.MVCFramework.Models.View_Model
{
    public class EmployeeUpdateModel
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
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a correct email format")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(10, ErrorMessage = "Phone Number cannot be larger than 10 digits")]
        [MinLength(10, ErrorMessage = "Phone Number cannot be smaller than 10 digits")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }

        [HiddenInput]
        public int OrganizationId { get; set; }
        public string Role { get; set; } = null;
    }
}