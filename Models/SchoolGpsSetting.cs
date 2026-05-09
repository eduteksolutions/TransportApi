using System.ComponentModel.DataAnnotations;

namespace TransportApi.Models
{
    public class SchoolGpsSetting
    {
        [Key]
        public int Id { get; set; }

        public int SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Provider { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [MaxLength(500)]
        public string? ApiUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}