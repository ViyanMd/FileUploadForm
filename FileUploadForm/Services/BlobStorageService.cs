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
            
            var credentials = new StorageSharedKeyCredential(StorageAccount, Key);
            var blobUri = $"https://{StorageAccount}.blob.core.windows.net";
            var BlobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
            Container = BlobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<ResponseDTO> Upload(IBrowserFile blob)
        {
            try
            {
                BlobClient client = Container.GetBlobClient(blob.Name);

                Uri blobSASURI = await CreateServiceSASBlob(client);

                BlobClient clientSAS = new BlobClient(blobSASURI);

                using (Stream? data = blob.OpenReadStream())
                {
                    await clientSAS.UploadAsync(data);
                }

                return new ResponseDTO
                {
                    Status = "File uploaded successfully",
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
                return new ResponseDTO { Status = "Filed to upload file!", Error = true };
            }

        }
        private async Task<Uri> CreateServiceSASBlob(BlobClient blobClient,string storedPolicyName = null)
        {
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

                return sasURI;
            }
            else
            {
                return null;
            }
        }
    }
}
