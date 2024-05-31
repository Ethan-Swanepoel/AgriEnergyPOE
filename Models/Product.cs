using System;
using System.Collections.Generic;

namespace AgriEnergy.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Category { get; set; }

    public DateOnly ProductionDate { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
