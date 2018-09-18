using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureQueuePocWithDotNet.Models;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureQueuePocWithDotNet.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private CloudStorageAccount storageAccount;
        private string storageConnectionString;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            /*storageConnectionString = _configuration["StorageConnectionString"];
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExistsAsync();*/
        }

        [Route("api/v0/myrestendpoint")]
        [HttpGet]
        public IActionResult GetStorageConfiguration()
        {
            storageConnectionString = _configuration["StorageConnectionString"];
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("Smart Signal POC Message");
            queue.AddMessageAsync(message);

            // Peek at the next message
            Task<CloudQueueMessage> peekedMessage = queue.PeekMessageAsync();

            var response = peekedMessage.Result.AsString;

            return Ok("Message in Queue is " + response);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
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
        }
    }
}
