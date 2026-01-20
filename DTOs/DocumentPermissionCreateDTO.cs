using Projekt_Zaliczeniowy_PZ.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Projekt_Zaliczeniowy_PZ.DTOs
{
    public class DocumentPermissionCreateDTO
    {
        public int DocumentId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DocumentRole Role { get; set; } = DocumentRole.Viewer;
    }
}
