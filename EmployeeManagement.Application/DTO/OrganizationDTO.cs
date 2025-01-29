using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EmployeeManagement.Core.Entites;

namespace EmployeeManagement.Application.DTO
{
    public class OrganizationDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        [DefaultValue(0)]
        public bool IsDeleted { get; set; }
        public List<EmployeeDTO> Employees { get; set; } = new List<EmployeeDTO>();
    }
}
