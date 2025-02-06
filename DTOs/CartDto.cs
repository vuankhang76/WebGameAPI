namespace GameAccountStore.DTOs
{
    public class AddToCartDto
    {
        public int GameAccountId { get; set; }
    }

    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int GameAccountId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
    }
}