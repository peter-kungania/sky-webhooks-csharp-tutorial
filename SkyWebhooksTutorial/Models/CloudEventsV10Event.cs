using System;
using Newtonsoft.Json;

namespace SkyWebhooksTutorial.Models
{
    // CloudEvents V1.0 Schema Events
    public class CloudEventsV10Event
    {
        /// <summary>
        /// Event type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// CloudEvents Spec Type (1.0)
        /// </summary>
        [JsonProperty(PropertyName = "specversion")]
        public string SpecVersion { get; set; }

        /// <summary>
        /// Event source
        /// </summary>
        [JsonProperty(PropertyName = "source")]
        public Uri Source { get; set; }

        /// <summary>
        /// Event subject
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Event ID
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Event date time
        /// </summary>
        [JsonProperty(PropertyName = "time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Time { get; set; }

        /// <summary>
        /// Event data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}
