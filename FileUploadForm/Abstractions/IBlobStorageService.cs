using FileUploadForm.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FileUploadForm.Abstractions
{
    public interface IBlobStorageService
    {
        Task<ResponseDTO> Upload(IBrowserFile blob);
    }
}