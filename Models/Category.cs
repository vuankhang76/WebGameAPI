using System;
using System.Collections.Generic;

namespace GameAccountStore.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<GameAccount> GameAccounts { get; set; } = new List<GameAccount>();
}
