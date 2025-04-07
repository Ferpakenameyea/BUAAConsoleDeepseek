using BUAADeepseekWebAPI;
using Newtonsoft.Json;

namespace dpask
{
    public sealed class InjectionManager : IInjectionManager
    {
        private readonly BUAADeepseek _deepseek;
        private readonly IPathProvider _pathProvider;
        private readonly IRuntimeConfig _config;
        private readonly List<InjectionTemplate> templates = new();
        private readonly string _templateRoot;

        public InjectionManager(BUAADeepseek deepseek, IPathProvider pathProvider, IRuntimeConfig config)
        {
            _deepseek = deepseek;
            _pathProvider = pathProvider;
            _config = config;

            _templateRoot = Path.Combine(_pathProvider.GetPath(), "templates");
            var directory = Directory.CreateDirectory(_templateRoot);

            foreach (var file in Directory.GetFiles(directory.FullName, "*.json"))
            {
                try
                {
                    using var reader = new StreamReader(file);
                    string json = reader.ReadToEnd();
                    var template = JsonConvert.DeserializeObject<InjectionTemplate>(json);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }
                catch (Exception)
                {
                    // maybe do some logging here
                }
            }
        }

        public void CommentTemplate(InjectionTemplate template, string comment)
        {
            template.Comment = comment;
            PersistChange(template);
        }

        public InjectionTemplate CreateNewTemplate(string prompt, string followInput, string? comment = null)
        {
            var template = new InjectionTemplate
            {
                Prompt = prompt,
                FollowInput = followInput,
                Comment = comment ?? string.Empty
            };
            templates.Add(template);
            PersistChange(template);
            return template;
        }

        public void DeleteTemplate(InjectionTemplate template)
        {
            var fileName = GetArchiveFileName(template);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            templates.Remove(template);
        }

        public IEnumerable<InjectionTemplate> Enumerate()
        {
            return templates;
        }

        public InjectionTemplate? GetInjectionTemplate(string id)
        {
            return templates.FirstOrDefault(t => t.Id == id);
        }

        public async Task InjectToSession(InjectionTemplate template, SessionMeta session)
        {
            var currentSession = _config.UsingSessionId;
            try
            {
                _config.UsingSessionId = session.Id;
                Console.WriteLine("Injecting...");

                await _deepseek.ChatAsync(
                    template.Content,
                    response =>
                    {
                        Console.Write(response.Data?.Answer);
                    }, useContext: true);

                Console.WriteLine();
                Console.WriteLine(SystemConstants.Seperator);
                Console.WriteLine($"Successfully injected template " +
                    $"{template.Id}{(template.Comment != "" ? $"(\"{template.Comment}\") " : "")}" +
                    $"to session {session.Id}({session.Name})");
            }
            finally
            {
                _config.UsingSessionId = currentSession;
            }
        }

        public void PersistChange(InjectionTemplate template)
        {
            var json = JsonConvert.SerializeObject(template);
            using var stream = new StreamWriter(GetArchiveFileName(template), append: false);
            stream.Write(json);
        }

        private string GetArchiveFileName(InjectionTemplate template)
        {
            return Path.Combine(_templateRoot, $"{template.Id}.json");
        }
    }
}
