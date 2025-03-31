using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BUAADeepseekWebAPI
{
    public sealed class Response
    {
        [JsonProperty("d")]
        public JObject? Data { get; set; }

        [JsonProperty("e")]
        public int Error { get; set; }

        [JsonProperty("m")]
        public string? Message { get; set; }
    }

    public sealed class Response<T>
    {
        [JsonProperty("d")]
        public T? Data { get; set; }

        [JsonProperty("e")]
        public int Error { get; set; }
        
        [JsonProperty("m")]
        public string? Message { get; set; }
    }
}
