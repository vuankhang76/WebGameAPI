using System;
using System.Collections.Generic;

namespace GameAccountStore.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int GameAccountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual GameAccount GameAccount { get; set; } = null!;
}
