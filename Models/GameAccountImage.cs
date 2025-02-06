using System;
using System.Collections.Generic;

namespace GameAccountStore.Models;

public partial class GameAccountImage
{
    public int Id { get; set; }

    public int GameAccountId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMainImage { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual GameAccount GameAccount { get; set; } = null!;
}
