using Newtonsoft.Json;

namespace SkyWebhooksTutorial.Models.EventTypes.EmailAddress
{
    /// <summary>
    /// Record context when an email address is changed
    /// </summary>
    public class EmailAddressChangedV1Data
    {
        /// <summary>
        /// The email identifier
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// The constituent identifier
        /// </summary>
        [JsonProperty(PropertyName = "constituent_id")]
        public string ConstituentId { get; set; }
    }
}