using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace REST_KUNDER.Models
{
    public class DataRest
    {
        [JsonProperty("code")]
        public string code { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [Required]
        [JsonProperty("data", Required = Required.Always)]
        public string data { get; set; }
    }
}