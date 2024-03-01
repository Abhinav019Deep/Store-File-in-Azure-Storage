using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

    public static class StoreFile
    {
        [FunctionName("StoreFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string containerName = Environment.GetEnvironmentVariable("ContainerName"); 
        var serviceClient = new BlobServiceClient(connectionString);
        var containerClient = serviceClient.GetBlobContainerClient(containerName);
        var path = Environment.GetEnvironmentVariable("FilePath");
        var fileName = Environment.GetEnvironmentVariable("FileName");
        var localFile = Path.Combine(path, fileName);
        await File.WriteAllTextAsync(localFile, "This is a test message");
        var blobClient = containerClient.GetBlobClient(fileName);
        
        using FileStream uploadFileStream = File.OpenRead(localFile);
        await blobClient.UploadAsync(uploadFileStream, true);
        uploadFileStream.Close();

        return new OkObjectResult("Hey Abhinav Your File is Uploaded");
    }
    }
