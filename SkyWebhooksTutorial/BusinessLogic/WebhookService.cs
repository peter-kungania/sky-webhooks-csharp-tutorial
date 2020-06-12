using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SkyWebhooksTutorial.BusinessLogic
{
    public class WebhookService : IWebhookService
    {
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(ILogger<WebhookService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Process email changed webhook event
        /// </summary>
        /// <param name="emailChangedEvent">The email changed event</param>
        public async Task<IActionResult> ProcessEmailChangedEvent(Models.EventTypes.EmailAddress.EmailAddressChangedV1Event emailChangedEvent)
        {
            // Process the email changed event here.
            // For simplicity of the tutorial, we'll simply log the event.
            _logger.LogInformation($"We have received the following email changed event for processing:\n{Newtonsoft.Json.JsonConvert.SerializeObject(emailChangedEvent)}.");

            // Example of event validation you might perform
            if (emailChangedEvent?.EmailAddressChangedData == null)
            {
                _logger.LogError("We tried to process the email changed event, but there is no Data to process.");

                // Return a 200 so Blackbaud doesn't send us this event again
                return new StatusCodeResult(200);
            }

            await Task.Yield(); // Here to satisfy async pattern until your handler does real work

            return new StatusCodeResult(200);
        }
    }
}
