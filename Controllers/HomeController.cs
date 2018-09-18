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
        private IConfiguration _configuration;
        private CloudStorageAccount storageAccount;
        private string storageConnectionString;
        private CloudQueue _queue;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            storageConnectionString = _configuration["StorageConnectionString"];
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            var queueName = _configuration["QueueName"];

            // Retrieve a reference to a container.
            _queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            _queue.CreateIfNotExistsAsync();
        }

        [Route("api/v0/createmessage")]
        [HttpGet]
        public IActionResult CreateMessage()
        {
           
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("Smart Signal POC Message ID " + Guid.NewGuid());
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

            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'naveed-poc' and append a GUID value to it to make the name unique. 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("naveed-poc-" + Guid.NewGuid().ToString());
            cloudBlobContainer.CreateAsync();

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            cloudBlobContainer.SetPermissionsAsync(permissions);

            // Create a file in your local MyDocuments folder to upload to a blob.
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFileName = "SmartSignalPOC_" + Guid.NewGuid().ToString() + ".txt";
            string sourceFile = Path.Combine(localPath, localFileName);
            // Write text to the file.
            var writer = new System.IO.StreamWriter(localFileName);
            writer.WriteLine("Smart Signal Test Message" + Guid.NewGuid());
            writer.Dispose();


            // Get a reference to the blob address, then upload the file to the blob.
            // Use the value of localFileName for the blob name.
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
            cloudBlockBlob.UploadFromFileAsync(sourceFile);

            return Ok("Created a blob: " + localFileName);
        }

        [Route("api/v0/downloadblob/{blobId}")]
        [HttpGet]
        public IActionResult DownloadBlob(Guid blobId)
        {

            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'naveed-poc' and append a GUID value to it to make the name unique. 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("naveed-poc-" + Guid.NewGuid().ToString());
            cloudBlobContainer.CreateAsync();

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            cloudBlobContainer.SetPermissionsAsync(permissions);

            var localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var localFileName = "SmartSignalPOC_" + blobId + ".txt";
            var sourceFile = Path.Combine(localPath, localFileName);
            var destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
            Console.WriteLine("Downloading blob to {0}", destinationFile);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
            cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);

            return Ok("Downloaded blob to " + localPath + "/" + localFileName);
        }

        public IActionResult Index()
        {
            return View();
        }

        /*public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
    }
}
