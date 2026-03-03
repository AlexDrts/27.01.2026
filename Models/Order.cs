using System;
using System.Collections.Generic;

namespace _27._01._2026.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
