using Newtonsoft.Json;

namespace dpask
{
    public sealed class RuntimeConfig : IRuntimeConfig
    {
        private record Data
        {
            [JsonProperty("usingSessionId")]
            public string UsingSessionId { get; set; } = string.Empty;
        }

        private Data data { get; set; }
        private readonly IPathProvider _pathProvider;

        [JsonProperty("usingSessionId")]
        public string UsingSessionId
        {
            get => this.data.UsingSessionId;
            set => this.data.UsingSessionId = value;
        }

        public RuntimeConfig(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
            try
            {
                var path = _pathProvider.GetPath();
                Directory.CreateDirectory(path);
                var configFilePath = Path.Combine(path, "config.json");
                if (!File.Exists(configFilePath))
                {
                    this.data = new();
                    return;
                }

                using var reader = new StreamReader(configFilePath);

                string json = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<Data>(json);
                this.data = data ?? new();
            }
            catch (Exception)
            {
                // do some logging
                throw;
            }
        }

        public void Persist()
        {
            string json = JsonConvert.SerializeObject(this);
            using var writer = new StreamWriter(
                Path.Combine(_pathProvider.GetPath(), "config.json"), append: false);
            writer.Write(json);
        }
    }
}
