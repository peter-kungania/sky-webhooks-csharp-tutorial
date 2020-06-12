using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkyWebhooksTutorial.AppSettings;
using SkyWebhooksTutorial.BusinessLogic;
using SkyWebhooksTutorial.Models;
using SkyWebhooksTutorial.Models.EventTypes;

namespace SkyWebhooksDemo.Controllers
{
    [ApiController]
    public class WebhookController : ControllerBase
    {
        // Webhook origin allowed to send us events
        private readonly string AllowedOrgin = "eventgrid.azure.net";

        // Our webhook config settings
        private readonly WebhookConfig _webhookConfig;

        // Our webhook service
        private readonly IWebhookService _webhookService;

        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            ILogger<WebhookController> logger,
            IOptions<WebhookConfig> webhookConfig,
            IWebhookService webhookService)
        {
            _logger = logger;
            _webhookConfig = webhookConfig.Value;
            _webhookService = webhookService;
        }

        [Route("eventhandler")]
        [HttpOptions]
        public IActionResult WebhookHandshake([FromQuery] string webhookKey)
        {
            _logger.LogInformation("Received OPTIONS request for CloudEvents Abuse validation handshake.");

            // Sanity check that the webhookKey has been configured
            if (string.IsNullOrWhiteSpace(_webhookConfig?.Key) || _webhookConfig.Key.Equals("replace-with-your-secret"))
            {
                _logger.LogError("Please configure the WebhookConfig.Key in your service's appsettings.json file.");
                return new StatusCodeResult(500);
            }

            if (this.Request.Headers.TryGetValue("WebHook-Request-Origin", out Microsoft.Extensions.Primitives.StringValues requestOriginValue) &&
                requestOriginValue.Equals(this.AllowedOrgin))
            {
                this.Response.Headers.Add("WebHook-Allowed-Origin", this.AllowedOrgin);
                this.Response.Headers.Add("WebHook-Allowed-Rate", "100");
                this.Response.Headers.Add("Allow", "POST");

                if (!string.Equals(_webhookConfig.Key, webhookKey))
                {
                    // Return an OK response, so you're not helping attackers try to brute force your webhookKey.
                    // For your application to know about any errors, log it here.
                    // This could help show that you've configured your subscription incorrectly.
                    // If you have configured incorrectly, you'll need to delete this subscription.
                    _logger.LogError("Invalid webhookKey was used during our CloudEvents Abuse Protection validation handshake.");
                }
                else
                {
                    _logger.LogInformation("Successfully handled CloudEvents Abuse validation handshake.");
                }
            }

            return Ok();
        }

        [Route("eventhandler")]
        [HttpPost]
        public async Task<IActionResult> WebhookEventHandler(
          [FromBody] CloudEventsV10Event cloudEvent,
          [FromQuery] string webhookKey)
        {
            _logger.LogInformation("Received POST request to webhook event handler.");

            // Sanity check that the webhookKey has been configured
            if (string.IsNullOrWhiteSpace(_webhookConfig?.Key) || _webhookConfig.Key.Equals("replace-with-your-secret"))
            {
                _logger.LogError("Please configure the WebhookConfig.Key in your service's appsettings.json file.");
                return new StatusCodeResult(500);
            }

            if (!string.Equals(_webhookConfig.Key, webhookKey))
            {
                // Return an OK response so this unauthorized request is not retried
                // Return an OK response, so you're not helping attackers try to brute force your webhookKey
                // For your application to know about any errors, log it here.
                // Be mindful that logging these errors could allow attackers to produce noisy or overloaded logs.
                // This could help show that you've configured your subscription incorrectly.
                // If you have configured incorrectly, you'll need to delete this subscription.
                // Adding a delay to "simulate" the work we would have done so attackers can't use the
                // abbreviated processing time as a clue that their webhookKey value is wrong.

                if (this.Request.Headers.TryGetValue("aeg-subscription-name", out Microsoft.Extensions.Primitives.StringValues subscriptionHeaderValue))
                {
                    // Logging the error if we have a subscription id
                    // Could keep us from logging arbitrary POSTs against our endpoint
                    _logger.LogError($"Received a webhook event with an invalid webhookKey. Subscription: {subscriptionHeaderValue}.");
                }

                await Task.Delay(new Random().Next(200, 900));

                return Ok();
            }

            if (!WebhookEvent.CreateWebhookEvent(cloudEvent, out var webhookEvent, _logger))
            {
                _logger.LogError($"Saw an unexpected event type when trying to handle a webhook event. Type: {cloudEvent.Type}.");

                // Returing OK so Blackbaud doesn't send us this event again
                return Ok();
            }

            return await webhookEvent.ProcessEvent(_webhookService);
        }
    }
}
