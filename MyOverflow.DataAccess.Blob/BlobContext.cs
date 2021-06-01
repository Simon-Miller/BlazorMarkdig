using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyOverflow.DataAccess.Blob
{
    public interface IBlobContext
    {
        /// <summary>
        /// call this first to get the container in which to upload files.
        /// </summary>
        Task<BlobContainerClient> SetupBlobContainerClient(string containerName);

        Task<BlobContentInfo> Upload(BlobContainerClient client, string fileName, Stream file);
        Task<BlobContentInfo> Upload(BlobClient client, Stream file);
    }

    public class BlobContext : IBlobContext
    {
        public BlobContext(string blobEndpoint = "UseDevelopmentStorage=true") // http://127.0.0.1:10000/
        {
            this.blobEndpointConnectionString = blobEndpoint;
        }

        private readonly string blobEndpointConnectionString;

        // In Azure, we first : 
        // 1. Create service client = new BlobServiceClient(connectionString) or new BlobServiceClient(serviceUri, credential) or even new BlobServiceClient(uriToAzureSharedAccessSignature);
        // 2. Then create container client = blobServiceClientInstance.GetBlobContainerClient(containerName)
        
        // Using the emulator, we skip the authentication steps, we just need the endpoint connection string, and the container name,
        // and therefore get a BlobContainerClient.
        public async Task<BlobContainerClient> SetupBlobContainerClient(string containerName)
        {
            var containerClient = new BlobContainerClient(this.blobEndpointConnectionString, containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            return containerClient;
        }

        public async Task<BlobContentInfo> Upload(BlobContainerClient client, string fileName, Stream file)
        {
            var blobClient = client.GetBlobClient(fileName);

            return await this.Upload(blobClient, file);
        }

        public async Task<BlobContentInfo> Upload(BlobClient client, Stream file)
        {
            return await client.UploadAsync(file, overwrite: true);
        }
    }
}
