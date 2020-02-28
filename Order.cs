using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo
{
    public class OrderMessage {
        public string name { get; set; }
        public string email { get; set; }
        public string productId { get; set; }
        public string quantity { get; set; }
    }

    [StorageAccount("AzureWebJobsStorage")]
    public static class Order
    {
        [FunctionName("Order")]
        [return: Queue("orders")]
        public static async Task<OrderMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(body);
            
            var returnValue = new OrderMessage() {
                name = data.name,
                email =  data.email,
                productId = data.productId,
                quantity = data.quantity
            };

            return returnValue;
        }
    }
}
