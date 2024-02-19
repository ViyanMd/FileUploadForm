namespace FileUploadForm.Models
{
    public class ResponseDTO
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public BlobDTO Blob { get; set; }

        public ResponseDTO()
        {
            Blob = new BlobDTO();
        }
    }
}
