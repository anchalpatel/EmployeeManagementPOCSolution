using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagement.MVCFramework.Models
{
    public class AuthApiResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}