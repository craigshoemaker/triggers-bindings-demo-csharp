using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo
{
    public class OrderInfo {
        public string id { get; set; }
        public string key { get; set; }
    }

    public static class OrdersQueueTrigger
    {
        [FunctionName("OrdersQueueTrigger")]
        public static void Run(
            [QueueTrigger(
                "orders",
                Connection = "AzureWebJobsStorage")]
                    OrderMessage orderMessage,
            [CosmosDB(
                databaseName: "orders",
                collectionName: "products",
                ConnectionStringSetting = "AzureCosmosDB",
                Id = "{productId}",
                PartitionKey = "{key}")]
                    OrderInfo orderInfo,
            ILogger log)
        {
            log.LogInformation(orderMessage.orderId);
        }
    }
}
