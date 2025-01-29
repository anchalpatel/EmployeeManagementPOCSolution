using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EmployeeManagement.Core.Entites;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [DefaultValue(0)]
    public bool IsDeleted { get; set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
