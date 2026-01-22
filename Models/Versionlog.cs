using System.ComponentModel.DataAnnotations;

namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class Versionlog
    {
        public int Id { get; set; }
        [Required]
        public int DocumentId { get; set; }
        public virtual Document? Document { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual AppUser? User { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
        public string Description { get; set; } = string.Empty;
    }
}
