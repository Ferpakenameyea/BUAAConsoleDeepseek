using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUAADeepseekWebAPI.Credentials
{
    public sealed class UserInfo
    {
        [JsonProperty("type_name")]
        public string TypeName { get; set; } = "";

        [JsonProperty("user_name")]
        public string UserName { get; set; } = "";

        [JsonProperty("user_number")]
        public string UserNumber { get; set; } = "";

        [JsonProperty("uid")]
        public int Uid { get; set; } = 0;
    }
}
