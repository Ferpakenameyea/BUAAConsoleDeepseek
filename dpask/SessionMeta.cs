using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public class SessionMeta
    {
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("last_access")]
        public DateTime LastAccess { get; set; } = DateTime.Now;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        public SessionMeta(string name, string? comment=null)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            Comment = comment ?? string.Empty;
        }
    }
}
