using Azure.Storage;
using Azure.Storage.Blobs;
using FileUploadForm.Abstractions;
using FileUploadForm.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FileUploadForm.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string StorageAccount = "uploadformstorage";
        private readonly string Key = "M+MlhoPyrtmCRfzB2LcMGWNBRQefCOxlztn29ofnOXvv3XUcXQeNsJbX/OVPLrNsghhExd7XPQt9+AStX5dSRg==";
        private BlobContainerClient Container;

        public BlobStorageService()
        {
            var credentials = new StorageSharedKeyCredential(StorageAccount, Key);
            var blobUri = $"https://{StorageAccount}.blob.core.windows.net";
            var BlobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
            Container = BlobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<ResponseDTO> Upload(IBrowserFile blob)
        {
            ResponseDTO response = new();

            BlobClient client = Container.GetBlobClient(blob.Name);

            using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = "File uploaded successfully";
            response.Error = false;
            response.Blob.Name = blob.Name;
            response.Blob.URI = client.Uri.AbsoluteUri;

            return response;
        }

        //private readonly string StorageAccount = "uploadformstorage";
        //private readonly string Key = "M+MlhoPyrtmCRfzB2LcMGWNBRQefCOxlztn29ofnOXvv3XUcXQeNsJbX/OVPLrNsghhExd7XPQt9+AStX5dSRg==";
        //private BlobServiceClient Client;

        //public StorageService()
        //{
        //    var credentials = new StorageSharedKeyCredential(StorageAccount, Key);
        //    var blobUri = $"https://{StorageAccount}.blob.core.windows.net";

        //    Client = new BlobServiceClient(new Uri(blobUri), credentials);

        //}

        //public async Task ListAllContainers()
        //{
        //    var containers = Client.GetBlobContainers();
        //    foreach (var container in containers)
        //    {
        //        Console.WriteLine(container.Name);
        //    }
        //}

        //public async Task<List<Uri>> Upload()
        //{
        //    var blobUri = new List<Uri>();

        //    var filePath = "test.txt";

        //    var container = Client.GetBlobContainerClient("files");

        //    await container.CreateIfNotExistsAsync();

        //    var blob = container.GetBlobClient(filePath);

        //    await blob.UploadAsync(filePath, true);

        //    blobUri.Add(blob.Uri);

        //    return blobUri;
        //}
    }
}
