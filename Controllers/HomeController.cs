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
        private CloudQueue _queue;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            storageConnectionString = _configuration["StorageConnectionString"];
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            _queue = queueClient.GetQueueReference("my-analytics-poc-queue");

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

            // Peek at the next message
            Task<CloudQueueMessage> peekedMessage = _queue.PeekMessageAsync();

            var response = peekedMessage.Result.AsString;

            return Ok(message + " created in queue " + _queue.Name);
        }

        [Route("api/v0/peekmessage")]
        [HttpGet]
        public IActionResult PeekMessage()
        {

            // Peek at the next message
            Task<CloudQueueMessage> message = _queue.GetMessageAsync();
            var response = message.Result.AsString;
            _queue.DeleteMessageAsync(message.Result);

            return Ok("Retrieved and deleted " + response + " from " + _queue.Name);
        }

        public IActionResult Index()
        {
            return Ok("Naveed's Azure Storage POC");
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
