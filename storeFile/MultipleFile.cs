using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace storeFile
{
    public static class MultipleFile
    {
        [FunctionName("MultipleFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            string sourceFolderPath = Environment.GetEnvironmentVariable("ContainerFilePath");
            string destinationFolderPath = Environment.GetEnvironmentVariable("DestinationFolderPath");


            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            //var path = Environment.GetEnvironmentVariable("ContainerFilePath");
            

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: sourceFolderPath))
            {
                if (blobItem is BlobItem blob)
                {
                    string blobName = blob.Name;
                    string destinationBlobName = blobName.Replace(sourceFolderPath, destinationFolderPath);
                    BlobClient sourceBlobClient = containerClient.GetBlobClient(blobName);
                    BlobClient destinationBlobClient = containerClient.GetBlobClient(destinationBlobName);
                    await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
                }
            }
            return new OkObjectResult("All files moved");


            
        }
    }
}
