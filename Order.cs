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
            var message = JsonConvert.DeserializeObject<OrderMessage>(body);
            message.orderId = Guid.NewGuid().ToString();
            return message;
        }
    }
}
