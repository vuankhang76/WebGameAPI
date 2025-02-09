namespace GameAccountStore.DTOs
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public decimal Total { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int GameAccountId { get; set; }
        public string GameTitle { get; set; }
        public string GameType { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddToCartDto
    {
        public int GameAccountId { get; set; }
    }
}