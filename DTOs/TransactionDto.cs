namespace GameAccountStore.DTOs
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int GameAccountId { get; set; }
        public string GameTitle { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
