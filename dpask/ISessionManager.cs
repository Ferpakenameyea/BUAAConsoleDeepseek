using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpask
{
    public interface ISessionManager
    {
        public IEnumerable<SessionMeta> GetSessions();
        public SessionMeta? GetSession(string id);
        public void PersistChange(SessionMeta session);
        public bool TryDeleteSession(string id);
        public bool TryDeleteSession(SessionMeta session) => TryDeleteSession(session.Id);
        public SessionMeta CreateSession(string name, string? comment = null);
        public string GetSessionHistoryPath(SessionMeta session);
    }
}
