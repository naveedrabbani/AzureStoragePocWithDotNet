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
            storageConnectionString = _configuration["StorageConnectionString"];

            
        }

        [Route("api/v0/myrestendpoint")]
        [HttpGet]
        public IActionResult GetStorageConfiguration()
        {

            storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(storageConnectionString));
            var s = storageAccount.Credentials.AccountName;
            string response = "Account Name that has been retrieved is " + s;
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
