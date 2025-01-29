using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagement.MVCFramework.Models.View_Model
{
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}