using System;
using System.Collections.Generic;

namespace GameAccountStore.Models;

public partial class GameAccount
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string GameType { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? Rank { get; set; }
    public int? NumberOfSkins { get; set; }
    public int? NumberOfChamps { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<GameAccountImage> GameAccountImages { get; set; } = new List<GameAccountImage>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
