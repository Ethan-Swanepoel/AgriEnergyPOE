using System;
using System.Collections.Generic;

namespace AgriEnergy.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserUid { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int UserRole { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // Computed property to display full name
    public string FullName => $"{Name} {Surname}";
}
