using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI
{
    public sealed class History
    {
        [JsonProperty("role")]
        public string Role { get; init; } = "";

        [JsonProperty("content")]
        public string Content { get; init; } = "";
    }

    public static class Roles
    {
        public const string USER = "user";
        public const string ASSISTANT = "assistant";
    }
}
