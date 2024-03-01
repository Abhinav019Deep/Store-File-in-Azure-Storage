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

namespace storeFile
{
    public static class BetweenFolder
    {
        [FunctionName("BetweenFolder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            var path = Environment.GetEnvironmentVariable("ContainerFilePath");
            var fileName = Environment.GetEnvironmentVariable("ContainerFileName");
            var localFile = Path.Combine(path, fileName);
            
            var blobClient1 = containerClient.GetBlobClient(localFile);
            var blobClient2 = containerClient.GetBlobClient("SecondFolder/"+fileName);

            
            blobClient2.StartCopyFromUri(blobClient1.Uri);
            

            return new OkObjectResult("Hey Abhinav Your File is Uploaded");
        }
    }
}
