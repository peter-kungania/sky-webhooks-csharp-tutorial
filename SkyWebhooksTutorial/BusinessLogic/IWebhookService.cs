using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkyWebhooksTutorial.Models.EventTypes.EmailAddress;

namespace SkyWebhooksTutorial.BusinessLogic
{
    public interface IWebhookService
    {
        /// <summary>
        /// Process email changed webhook event
        /// </summary>
        /// <param name="emailChangedEvent">The email changed event</param>
        Task<IActionResult> ProcessEmailChangedEvent(EmailAddressChangedV1Event emailChangedEvent);
    }
}