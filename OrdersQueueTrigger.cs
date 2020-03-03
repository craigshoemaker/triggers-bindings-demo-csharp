using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using SendGrid.Helpers.Mail;

namespace Demo
{
    public class InventoryItem {
        public string id { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
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
                    InventoryItem product,

            [CosmosDB(
                databaseName: "orders",
                collectionName: "products",
                ConnectionStringSetting = "AzureCosmosDB")]
                    out InventoryItem updatedProduct,

            [TwilioSms(
                AccountSidSetting = "TwilioAccountSid",
                AuthTokenSetting = "TwilioAuthToken",
                From = "+12056289114")]
                    out CreateMessageOptions smsMessage,

            [SendGrid(ApiKey = "SendGridAPIKey")] out SendGridMessage email,

            ILogger log)
        {

            try {

                // -- demo purposes only ----------------------------------------------------------
                orderMessage.email = Environment.GetEnvironmentVariable("DestinationEmail");
                orderMessage.phone = Environment.GetEnvironmentVariable("DestinationPhoneNumber");
                // --------------------------------------------------------------------------------

                smsMessage = new CreateMessageOptions(new PhoneNumber(orderMessage.phone))
                {
                    Body = string.Format("Thanks for your order, {0}!", orderMessage.name)
                };

                updatedProduct = product;
                updatedProduct.quantity = (product.quantity - orderMessage.quantity);

                email = new SendGridMessage();
                email.AddTo(orderMessage.email);
                email.AddContent("text/html", string.Format(
                    "Hi, {0}! Your order for \"{1}\" is processed.", 
                    orderMessage.name, product.description));
                email.SetFrom(new EmailAddress("test@example.com"));
                email.SetSubject("Thank you for your order!");

                log.LogInformation(orderMessage.orderId.ToString());
            }
            catch(Exception ex) {
                log.LogError("order", ex);
                throw;
            }

        }
    }
}
