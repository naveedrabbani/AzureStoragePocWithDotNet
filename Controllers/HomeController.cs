using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureQueuePocWithDotNet.Models;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureQueuePocWithDotNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly IConfiguration _configuration;
        private readonly CloudQueue _queue;
        private readonly CloudBlobContainer _cloudBlobContainer;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            var storageConnectionString = _configuration["StorageConnectionString"];
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            var queueName = _configuration["QueueName"];
            var blobStorageContainerName = configuration["BlobContainerName"];

            // Retrieve a reference to a container.
            _queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            _queue.CreateIfNotExistsAsync();

            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = _storageAccount.CreateCloudBlobClient();

            // Create a container called 'naveed-poc' and append a GUID value to it to make the name unique. 
            _cloudBlobContainer = cloudBlobClient.GetContainerReference(blobStorageContainerName);
            _cloudBlobContainer.CreateAsync();

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            _cloudBlobContainer.SetPermissionsAsync(permissions);
        }

        [Route("api/v0/createmessage")]
        [HttpGet]
        public IActionResult CreateMessage()
        {
           
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("naveed-queue-poc_" + Guid.NewGuid());
            _queue.AddMessageAsync(message);

            var response = new QueueEntry()
            {
                QueueName = _queue.Name,
                QueueMessage = message.AsString,
                Action = "Enqueue"
            };

            return Ok(response);
        }

        [Route("api/v0/getmessage")]
        [HttpGet]
        public IActionResult GetMessage()
        {

            // Peek at the next message
            Task<CloudQueueMessage> message = _queue.GetMessageAsync();
            _queue.DeleteMessageAsync(message.Result);

            var response = new QueueEntry()
            {
                QueueName = _queue.Name,
                QueueMessage = message.Result.AsString,
                Action = "Dequeue"
            };

            return Ok(response);
        }

        [Route("api/v0/createblob")]
        [HttpGet]
        public IActionResult CreateBlob()
        {
            // Create a file in your local MyDocuments folder to upload to a blob.
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "naveed-blob-poc_" + Guid.NewGuid().ToString() + ".txt";
            string sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            var writer = new System.IO.StreamWriter(localFileName);
            writer.WriteLine("POC test Data: " + Guid.NewGuid());
            writer.Dispose();


            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(localFileName);
            cloudBlockBlob.UploadFromFileAsync(sourceFile);

            BlobEntry response = new BlobEntry()
            {
                BlobContainer = _cloudBlobContainer.Name,
                FileName = localFileName,
                Action = "Upload"
            };

            return Ok(response);
        }

        [Route("api/v0/getblobdetails/{blobId}")]
        [HttpGet]
        public IActionResult DownloadBlob(Guid blobId)
        {
            var localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var localFileName = "naveed-blob-poc_" + blobId + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            var destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");

            CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(localFileName);
            cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);

            BlobEntry response = new BlobEntry()
            {
                BlobContainer = _cloudBlobContainer.Name,
                FileName = localFileName,
                Action = "Download"
            };

            return Ok(response);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
