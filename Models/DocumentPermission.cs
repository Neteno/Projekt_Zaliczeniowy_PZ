using Projekt_Zaliczeniowy_PZ.Models.Enums;

namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class DocumentPermission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual AppUser? User { get; set; }
        public DocumentRole Role { get; set; }
    }
}
