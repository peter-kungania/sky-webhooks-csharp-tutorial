using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyWebhooksTutorial.BusinessLogic;

namespace SkyWebhooksTutorial.Models.EventTypes
{
    public abstract class WebhookEvent : CloudEvent
    {
        protected ILogger Logger { get; set; }

        public WebhookEvent(CloudEvent cloudEvent, ILogger logger) : base(
            CloudNative.CloudEvents.CloudEventsSpecVersion.V1_0, 
            cloudEvent.Type,
            cloudEvent.Source,
            cloudEvent.Id,
            cloudEvent.Time)
        {
            this.Subject = cloudEvent.Subject;
            this.Logger = logger;
            this.Data = BuildEventDataObject(cloudEvent.Data);
        }

        /// <summary>
        /// Event-type-specific processing of the event
        /// </summary>
        /// <param name="webhookService"></param>
        public abstract Task<IActionResult> ProcessEvent(IWebhookService webhookService);

        protected abstract object BuildEventDataObject(object rawEventData);

        public static bool CreateWebhookEvent(CloudEvent cloudEvent, out WebhookEvent blackbaudEvent, ILogger logger)
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
