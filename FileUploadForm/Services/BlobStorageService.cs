using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using FileUploadForm.Abstractions;
using FileUploadForm.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FileUploadForm.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly ILogger<IBlobStorageService> Logger;
        private readonly IConfiguration Configuration;
        private readonly string StorageAccount = string.Empty;
        private readonly string Key = string.Empty;
        private BlobContainerClient Container;

        public BlobStorageService(ILogger<IBlobStorageService> _logger, IConfiguration _configuration)
        {
            Logger = _logger;
            Configuration = _configuration;



            StorageAccount = Configuration.GetValue<string>("AzureConnnection:StorageAccount")!;
            Key = Configuration.GetValue<string>("AzureConnnection:StorageKey")!;


            //Establishing connection to Azure Blob Storage
            var credentials = new StorageSharedKeyCredential(StorageAccount, Key);
            var blobUri = $"https://{StorageAccount}.blob.core.windows.net";
            var BlobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
            Container = BlobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<ResponseDTO> Upload(IBrowserFile blob, string email)
        {

            string secureName = GenerateSecureName(blob.Name);


            //Metadata for the blob
            Dictionary<string, string> metadata = new Dictionary<string, string>();



            try
            {
                BlobClient client = Container.GetBlobClient(secureName);

                Uri blobSASURI = CreateServiceSASBlob(client);

                BlobClient clientSAS = new BlobClient(blobSASURI);

                //Adding metadata to the blob for trigger function
                metadata.Add("Email", email);
                metadata.Add("FileName", blob.Name);
                metadata.Add("SASURL", clientSAS.Uri.AbsoluteUri);

                var blobUploadOptions = new BlobUploadOptions()
                {
                    Metadata = metadata
                };

                using (Stream? data = blob.OpenReadStream())
                {
                    await clientSAS.UploadAsync(data, blobUploadOptions);
                }

                //Returning the ResponseDTO to work with further on the client side
                return new ResponseDTO
                {
                    Status = "File uploaded successfully. Check your email (Spam folder) for download link.",
                    Error = false,
                    Blob = new BlobDTO
                    {
                        Name = blob.Name,
                        URI = clientSAS.Uri.AbsoluteUri
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new ResponseDTO { Status = "Failed to upload file!", Error = true };
            }

        }
        private Uri CreateServiceSASBlob(BlobClient blobClient)
        {
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

                return sasURI;
            }
            else
            {
                return null;
            }
        }

        private string GenerateSecureName(string fileName)
        {
            string secureName = Path.GetRandomFileName() + Path.GetExtension(fileName);
            return secureName;
        }
    }
}
