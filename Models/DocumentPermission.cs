using Projekt_Zaliczeniowy_PZ.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class DocumentPermission
    {
        public int Id { get; set; }
        [Required]
        public int DocumentId { get; set; }
        public virtual Document? Document { get; set; }
        [Required]
        public string UserId { get; set; } 
        public virtual AppUser? User { get; set; }
        [Required]
        public DocumentRole Role { get; set; }
    }
}
