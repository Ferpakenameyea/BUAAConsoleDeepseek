using BUAADeepseekWebAPI;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace dpask
{
    [JsonSerializable(typeof(History))]
    public partial class HistoryJsonContext : JsonSerializerContext { }

    public sealed class FileContextManager : IContextManager, IDisposable
    {
        private readonly LinkedList<(History user, History system)> _histories;
        private readonly string _path;
        private readonly int _maxHistories = 100;

        public FileContextManager(IPathProvider persistPathProvider, int maxHistories = 100)
        {
            this._maxHistories = maxHistories;
            this._path = persistPathProvider.GetPath();
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? string.Empty);
            var fileInfo = new FileInfo(_path);

            if (!fileInfo.Exists)
            {
                this._histories = new();
                return;
            }

            using var reader = new StreamReader(_path);
            var json = reader.ReadToEnd();
            _histories = JsonConvert.DeserializeObject<LinkedList<(History user, History system)>>(json) ?? new();
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

        public void Dispose()
        {
            this.PersistHistories();
        }

        public IEnumerable<(History user, History system)> Enumerate()
        {
            return this._histories.AsEnumerable();
        }

        public void PersistHistories()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? string.Empty);
            using var writer = new StreamWriter(_path, append: false);
            var text = JsonConvert.SerializeObject(this._histories);
            
            if (text == null)
            {
                Console.WriteLine("Error when persisting histories!");
            }
            
            writer.Write(text);
        }
    }
}
