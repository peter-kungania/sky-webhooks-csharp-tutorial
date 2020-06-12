using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyWebhooksTutorial.BusinessLogic;

namespace SkyWebhooksTutorial.Models.EventTypes
{
    public abstract class WebhookEvent : CloudEventsV10Event
    {
        protected ILogger Logger { get; set; }

        public WebhookEvent(CloudEventsV10Event cloudEvent, ILogger logger)
        {
            this.Id = cloudEvent.Id;
            this.SpecVersion = cloudEvent.SpecVersion;
            this.Type = cloudEvent.Type;
            this.Subject = cloudEvent.Subject;
            this.Source = cloudEvent.Source;
            this.Time = cloudEvent.Time;
            this.Logger = logger;
            this.Data = BuildEventDataObject(cloudEvent.Data);
        }

        /// <summary>
        /// Event-type-specific processing of the event
        /// </summary>
        /// <param name="webhookService"></param>
        public abstract Task<IActionResult> ProcessEvent(IWebhookService webhookService);

        protected abstract object BuildEventDataObject(object rawEventData);

        public static bool CreateWebhookEvent(CloudEventsV10Event cloudEvent, out WebhookEvent blackbaudEvent, ILogger logger)
        {
            var validEvent = true;

            switch (cloudEvent.Type)
            {
                case EmailAddress.EmailAddressChangedV1Event.EVENTTYPE:
                    blackbaudEvent = new EmailAddress.EmailAddressChangedV1Event(cloudEvent, logger);
                    break;

                default:
                    blackbaudEvent = null;
                    validEvent = false;
                    break;
            }

            return validEvent;
        }
    }
}
