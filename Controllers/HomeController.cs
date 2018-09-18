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
            //storageConnectionString = _configuration["StorageConnectionString"];
            //Temporary Access Key
            storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccounforpoc;AccountKey=HIAt8XEikd9tJQFkPEpgGh9Dg1PCZ0OQHNYK1KUoEpOIuRnvw1Xi8UOwJ9ex7aDZ4d+0CIi3oCIiG6oRp3Z9yw==;EndpointSuffix=core.windows.net";
        }

        [Route("api/v0/myrestendpoint")]
        [HttpGet]
        public IActionResult GetStorageConfiguration()
        {

            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var s = storageAccount.Credentials.AccountName;
            var t =  _configuration["StorageConnectionString"];
            string response = "Account Name that has been retrieved is " + s + " and connection from settings is " + t;
            return Ok(response);
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
