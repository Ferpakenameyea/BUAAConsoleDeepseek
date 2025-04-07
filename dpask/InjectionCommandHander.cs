using dpask.Exceptions;

namespace dpask
{
    public sealed class InjectionCommandHander : IAsyncCommandHandler
    {
        private readonly IInjectionManager _injectionManager;

        private readonly ISessionManager _sessionManager;

        private readonly IRuntimeConfig _config;

        public InjectionCommandHander(
            IInjectionManager injectionManager, 
            ISessionManager sessionManager, 
            IRuntimeConfig config)
        {
            _injectionManager = injectionManager;
            _sessionManager = sessionManager;
            _config = config;
        }

        public async Task DispatchAsync(string[] args)
        {
            IAsyncCommandHandler.ExpectArgsCount(args, 1, int.MaxValue);
            switch (args[0])
            {
                case "list":
                case "l":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 1);
                        Console.WriteLine(SystemConstants.Seperator);
                        foreach (var injection in _injectionManager.Enumerate())
                        {
                            Console.WriteLine($"""
                                {injection.Id}
                                CreateTime:
                                    {injection.CreateTime}
                                LastUpdate:
                                    {injection.UpdateTime}
                                Comment:
                                    {injection.Comment}
                                {SystemConstants.Seperator}
                                Prompt:
                                    {injection.Prompt}
                                Input:
                                    {injection.FollowInput}
                                """);

                            Console.WriteLine(SystemConstants.Seperator);
                        }
                        break;
                    }

                case "create":
                case "c":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 3, 4);
                        var prompt = args[1];
                        var followInput = args[2];
                        var comment = args.Length == 4 ? args[3] : null;

                        var template = _injectionManager.CreateNewTemplate(prompt, followInput, comment);
                        Console.WriteLine($"Created new template {template.Id}" +
                            $"{(template.Comment == null ? "" : $"(\"{template.Comment}\")")}");
                        break;
                    }

                case "delete":
                case "d":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 2);
                        var template = _injectionManager.GetInjectionTemplate(args[1]);
                        if (template == null)
                        {
                            Console.WriteLine($"Template {args[1]} not found!");
                            return;
                        }
                        _injectionManager.DeleteTemplate(template);
                        Console.WriteLine($"Deleted template {template.Id}" +
                            $"{(template.Comment == null ? "" : $"(\"{template.Comment}\")")}");
                        break;
                    }

                case "edit":
                case "e":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 4, 6);
                        var template = _injectionManager.GetInjectionTemplate(args[1]);
                        if (template == null)
                        {
                            Console.WriteLine($"Template {args[1]} not found!");
                            return;
                        }

                        if (args.Length == 5)
                        {
                            Console.WriteLine($"Command2 {args[4]} missing value");
                            return;
                        }

                        var command = args[2];
                        var value = args[3];

                        static void EditTemplate(InjectionTemplate template, string command, string value)
                        {
                            switch (command)
                            {
                                case "-set-prompt":
                                case "-sp":
                                    template.Prompt = value;
                                    break;

                                case "-set-follow-input":
                                case "-sfi":
                                    template.FollowInput = value;
                                    break;

                                default:
                                    throw new UnknownCommandException(command);
                            }
                        }

                        EditTemplate(template, command, value);

                        if (args.Length == 6)
                        {
                            command = args[4];
                            value = args[5];
                            EditTemplate(template, command, value);
                        }

                        _injectionManager.PersistChange(template);
                        Console.WriteLine(template.Content);
                        Console.WriteLine(SystemConstants.Seperator);
                        break;
                    }

                case "use":
                case "u":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 2);
                        var template = _injectionManager.GetInjectionTemplate(args[1]);
                        if (template == null)
                        {
                            Console.WriteLine($"Template {args[1]} not found!");
                            return;
                        }
                        var session = _sessionManager.GetSession(_config.UsingSessionId);
                        if (session == null)
                        {
                            Console.WriteLine($"Session {_config.UsingSessionId} not found!");
                            return;
                        }
                        await _injectionManager.InjectToSession(template, session);
                        break;
                    }

                case "comment":
                case "cm":
                    {
                        IAsyncCommandHandler.ExpectArgsCount(args, 3);
                        var template = _injectionManager.GetInjectionTemplate(args[1]);
                        if (template == null)
                        {
                            Console.WriteLine($"Template {args[1]} not found!");
                            return;
                        }
                        var comment = args[2];
                        _injectionManager.CommentTemplate(template, comment);
                        Console.WriteLine($"Commented template {template.Id} with \"{comment}\"");
                        break;
                    }

            }
        }
    }
}
