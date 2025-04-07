namespace BUAADeepseekWebAPI
{
    public interface IContextManager
    {
        public void AddHistory((History user, History system) history);
        public void ClearHistory();
        public IEnumerable<(History user, History system)> Enumerate();
        public void PersistHistories();
    }
}
