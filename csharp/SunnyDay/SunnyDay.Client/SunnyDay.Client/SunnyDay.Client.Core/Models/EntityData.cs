using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace SunnyDay.Client.Core.Models
{
    public class EntityData
    {
        // EntityData Base Type
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Version]
        public string AzureVersion { get; set; }
    }
}
