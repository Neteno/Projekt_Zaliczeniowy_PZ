namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public virtual AppUser? CreatedBy { get; set; }
    }
}
