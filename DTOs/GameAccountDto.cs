using System.ComponentModel.DataAnnotations;

namespace GameAccountStore.DTOs
{
    public class CreateGameAccountDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string GameType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class GameAccountResponseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GameType { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}