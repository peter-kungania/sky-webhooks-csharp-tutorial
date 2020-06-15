using System;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyWebhooksTutorial.BusinessLogic;

namespace SkyWebhooksTutorial.Models.EventTypes.EmailAddress
{
    /// <summary>
    /// Event when an email address is changed
    /// </summary>
    public class EmailAddressChangedV1Event : WebhookEvent
    {
        public const string EVENTTYPE = "com.blackbaud.constituent.emailaddress.change.v1";

        public EmailAddressChangedV1Event(CloudEvent cloudEvent, ILogger logger) : base(cloudEvent, logger)
        {
        }

        public override async Task<IActionResult> ProcessEvent(IWebhookService webhookService)
        {
            return await webhookService.ProcessEmailChangedEvent(this);
        }

        /// <summary>
        /// Data for an email address change
        /// </summary>
        public EmailAddressChangedV1Data EmailAddressChangedData
        {
            get
            {
                return (EmailAddressChangedV1Data)this.Data;
            }
        }

        protected override object BuildEventDataObject(object rawEventData)
        {
            try
            {
                if (rawEventData != null)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<EmailAddressChangedV1Data>(rawEventData.ToString());
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Unable to deseralize email changed event Data. Event data:\n{rawEventData.ToString()}");
            }

            return null;
        }
    }
}