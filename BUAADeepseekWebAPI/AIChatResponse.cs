using Newtonsoft.Json;

namespace BUAADeepseekWebAPI
{
    public sealed class AIChatResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("answer")]
        public string Answer { get; set; } = "";

        [JsonProperty("message_id")]
        public string MessageId { get; set; } = "";
    }
}