using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace FileUploadForm.Models
{
    public class FormModel
    {
        [Required(ErrorMessage = "Email required!")]
        [EmailAddress(ErrorMessage = "Make sure email is correct!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "File required!")]
        public IBrowserFile File { get; set; }
        [FileExtensions(Extensions = ".doc, .docx", ErrorMessage = "Please upload a valid file (.doc/.docx format)!")]

        public string Name { get; set; }

    }
}
