using System.ComponentModel.DataAnnotations;

namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class Document
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa dokumentu jest wymagana")]
        [MaxLength(50, ErrorMessage = "Nazwa nie może przekraczać 50 znaków")]
        [Display(Name = "Nazwa dokumentu")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "Sesja Zawartość Dokumentu")]
        public string Content { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Autor")]
        public string CreatedById { get; set; } = string.Empty;
        public virtual AppUser? CreatedBy { get; set; }
    }
}
