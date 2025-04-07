using BUAADeepseekWebAPI;

namespace BUAADeepseekWebAPI
{
    public class MemoryContextManager : IContextManager
    {
        private readonly LinkedList<(History user, History system)> _histories = new();

        private readonly int _maxHistories = 100;

        public MemoryContextManager(int maxHistories = 100)
        {
            this._maxHistories = maxHistories;
        }

        public void AddHistory((History user, History system) history)
        {
            this._histories.AddLast(history);
            while (this._histories.Count > _maxHistories)
            {
                this._histories.RemoveFirst();
            }
        }

        public void ClearHistory()
        {
            this._histories.Clear();
        }

        public IEnumerable<(History user, History system)> Enumerate()
        {
            return this._histories.AsEnumerable();
        }

        public void PersistHistories(){}
    }
}
