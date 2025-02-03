using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace EmployeeManagement.Core.Entites;


public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int OrganizationId { get; set; }

    public string UserId { get; set; } = null!;
    public string CreatedBy { get; set; }

    [DefaultValue(0)]
    public bool IsDeleted { get; set; }
    public virtual Organization Organization { get; set; } = null!;


}
