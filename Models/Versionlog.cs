using System.ComponentModel.DataAnnotations;

namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class Versionlog
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public virtual Document? Document { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual AppUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
