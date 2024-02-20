using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace FileUploadForm.Models
{
    public class FormModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Make sure email is correct!")]
        public string Email { get; set; }
        [Required]
        [FileExtensions(Extensions = ".doc,.docx,", ErrorMessage = "Please upload a valid file (.doc/.docx)!")]
        public IBrowserFile File { get; set; } 

    }
}
