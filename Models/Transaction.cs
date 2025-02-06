using System;
using System.Collections.Generic;

namespace GameAccountStore.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameAccountId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual GameAccount GameAccount { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
