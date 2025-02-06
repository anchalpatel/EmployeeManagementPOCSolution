using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Core.DTO
{
    public class TokenDTO
    {
        public string userId {  get; set; }
        public string userName { get; set; }
        public int organizationId { get; set; }
        public string createdBy { get; set; }
        public string organizationName { get; set; }
    }
}
