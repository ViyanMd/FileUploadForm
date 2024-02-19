namespace FileUploadForm.Models
{
    public class BlobDTO
    {
        public string? URI { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }
}
