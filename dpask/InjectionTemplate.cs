using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public sealed class InjectionTemplate
    {
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("comment")]
        public string Comment { get; set; } = string.Empty;

        [JsonProperty("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonProperty("input")]
        public string FollowInput { get; set; } = string.Empty;

        [JsonIgnore]
        public string Content => $"""
            <system>
            {Prompt}
            </system>
            <input>
            {FollowInput}
            </input>
            """;
    }
}
