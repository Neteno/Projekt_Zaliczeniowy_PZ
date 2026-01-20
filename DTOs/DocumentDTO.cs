using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.DTOs
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public DocumentDTO() { }
        public DocumentDTO(Document document)
        {
            Id = document.Id;
            Title = document.Title;
        }
    }
}
