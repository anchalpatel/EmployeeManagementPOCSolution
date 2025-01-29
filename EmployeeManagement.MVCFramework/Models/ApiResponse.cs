using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagement.MVCFramework.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}