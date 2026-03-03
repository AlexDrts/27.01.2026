using System;
using System.Collections.Generic;

namespace _27._01._2026.Models;

public partial class Review
{
    public int Id { get; set; }

    public string Comment { get; set; } = null!;

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
