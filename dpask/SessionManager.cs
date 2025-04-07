using Newtonsoft.Json;

namespace dpask
{
    public sealed class SessionManager : ISessionManager
    {
        private readonly IPathProvider _pathProvider;

        private readonly string _sessionMetaRoot;
        private readonly string _sessionHistoryRoot;
        private readonly List<SessionMeta> _loadedSessions = new();
        private readonly IRuntimeConfig _runtimeConfig;

        public SessionManager(IPathProvider pathProvider, IRuntimeConfig runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
            _pathProvider = pathProvider;
            string root = _pathProvider.GetPath();

            _sessionMetaRoot = Path.Combine(root, "session_meta");
            _sessionHistoryRoot = Path.Combine(root, "session_history");
            Directory.CreateDirectory(_sessionMetaRoot);
            Directory.CreateDirectory(_sessionHistoryRoot);

            var sessionMetaFiles = Directory.GetFiles(_sessionMetaRoot, "*.json");

            foreach (var file in sessionMetaFiles)
            {
                try
                {
                    using var reader = new StreamReader(file);
                    string json = reader.ReadToEnd();
                    var sessionMeta = JsonConvert.DeserializeObject<SessionMeta>(json);
                    if (sessionMeta != null)
                    {
                        _loadedSessions.Add(sessionMeta);
                    }
                }
                catch (Exception)
                {
                    // maybe do some logging here
                }
            }

            if (this._loadedSessions.Count == 0 || 
                runtimeConfig.UsingSessionId == string.Empty ||
                this.GetSession(runtimeConfig.UsingSessionId) == null)
            {
                var session = this.CreateSession("system-created", "unnamed session");
                runtimeConfig.UsingSessionId = session.Id;
            }
        }

        public SessionMeta CreateSession(string name, string? comment = null)
        {
            SessionMeta session = new(name, comment);
            string sessionMetaPath = Path.Combine(_sessionMetaRoot, $"{session.Id}.json");

            using (var writer = new StreamWriter(sessionMetaPath, append: false))
            {
                string json = JsonConvert.SerializeObject(session);
                writer.Write(json);
            }

            _loadedSessions.Add(session);

            return session;
        }

        public SessionMeta? GetSession(string id)
        {
            return _loadedSessions.FirstOrDefault(s => s.Id == id);
        }

        public string GetSessionHistoryPath(SessionMeta session)
        {
            return Path.Combine(_sessionHistoryRoot, $"{session.Id}.json");
        }

        public IEnumerable<SessionMeta> GetSessions()
        {
            return _loadedSessions;
        }

        public void PersistChange(SessionMeta session)
        {
            string sessionMetaPath = Path.Combine(_sessionMetaRoot, $"{session.Id}.json");
            using var writer = new StreamWriter(sessionMetaPath, append: false);
            string json = JsonConvert.SerializeObject(session);
            writer.Write(json);
        }

        public bool TryDeleteSession(string id)
        {
            var session = GetSession(id);
            if (session != null)
            {
                string sessionMetaPath = Path.Combine(_sessionMetaRoot, $"{session.Id}.json");
                string sessionHistoryPath = GetSessionHistoryPath(session);
                if (File.Exists(sessionMetaPath))
                {
                    File.Delete(sessionMetaPath);
                }
                if (File.Exists(sessionHistoryPath))
                {
                    File.Delete(sessionHistoryPath);
                }
                _loadedSessions.Remove(session);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
