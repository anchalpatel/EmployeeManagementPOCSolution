using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmployeeManagement.MVCFramework.Models.View_Model
{
    public class OrganizationViewModel
    {
        [HiddenInput]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DisplayName("Organization Name")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }
        [HiddenInput]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("employees")]
        public List<EmployeeViewModel> Employees { get; set; } = new List<EmployeeViewModel>();
    }
}