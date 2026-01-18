namespace Projekt_Zaliczeniowy_PZ.Models
{
    public class Versionlog
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public virtual Document? Document { get; set; }
    }
}
