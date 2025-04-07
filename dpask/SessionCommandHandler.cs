using dpask.Exceptions;

namespace dpask
{
    internal sealed class SessionCommandHandler : ICommandHandler
    {
        private readonly ISessionManager _sessionManager;
        private readonly IRuntimeConfig _config;

        public SessionCommandHandler(
            ISessionManager sessionManager,
            IRuntimeConfig config)
        {
            _sessionManager = sessionManager;
            _config = config;
        }

        public void Dispatch(string[] args)
        {
            ICommandHandler.ExpectArgsCount(args, 1, int.MaxValue);

            switch (args[0])
            {
                case "list":
                case "l":
                    {
                        ICommandHandler.ExpectArgsCount(args, 1);

                        Console.WriteLine(SystemConstants.Seperator);
                        foreach (var session in _sessionManager.GetSessions())
                        {
                            Console.WriteLine($"""
                                {(_config.UsingSessionId == session.Id ? "[√]" : "")}{session.Name} : {session.Id}
                            
                                CreateTime:
                                    {session.CreateTime}
                                LastAccessTime:
                                    {session.LastAccess}
                                Comment:
                                    {session.Comment}
                                {SystemConstants.Seperator}
                                """);
                        }
                        break;
                    }

                case "view":
                case "v":
                    {
                        ICommandHandler.ExpectArgsCount(args, 2);
                        var session = _sessionManager.GetSession(args[1]);
                        if (session == null)
                        {
                            Console.WriteLine($"Session {args[1]} not found!");
                            return;
                        }
                        Console.WriteLine(SystemConstants.Seperator);
                        Console.WriteLine($"Session {session.Name} : {session.Id}");
                        Console.WriteLine(SystemConstants.Seperator);
                        Console.WriteLine($"CreateTime: {session.CreateTime}");
                        Console.WriteLine($"LastAccessTime: {session.LastAccess}");
                        Console.WriteLine($"Comment: {session.Comment}");
                        Console.WriteLine(SystemConstants.Seperator);
                        Console.WriteLine("History:");
                        var context = new FileContextManager(new DelegatePathProvider(() => _sessionManager.GetSessionHistoryPath(session)));
                        foreach (var (user, system) in context.Enumerate())
                        {
                            Console.WriteLine("User:");
                            Console.WriteLine(user.Content);

                            Console.WriteLine("AI:");
                            Console.WriteLine(system.Content);
                        }
                        break;
                    }

                case "create":
                case "c":
                    {
                        ICommandHandler.ExpectArgsCount(args, min: 2, max: 3);
                        var session = _sessionManager.CreateSession(args[1], args.Length == 3 ? args[2] : null);
                        Console.WriteLine($"Session {session.Name} : {session.Id}");
                        break;
                    }

                case "comment":
                case "cm":
                    {
                        ICommandHandler.ExpectArgsCount(args, 3);
                        var session = _sessionManager.GetSession(args[1]);
                        if (session == null)
                        {
                            Console.WriteLine($"Session {args[1]} not found!");
                            return;
                        }
                        session.Comment = args[2];
                        _sessionManager.PersistChange(session);
                        break;
                    }

                case "delete":
                case "d":
                    {
                        ICommandHandler.ExpectArgsCount(args, 2);
                        var session = _sessionManager.GetSession(args[1]);
                        if (session == null)
                        {
                            Console.WriteLine($"Session {args[1]} not found!");
                            return;
                        }
                        _sessionManager.TryDeleteSession(session);
                        break;
                    }

                case "switch":
                case "s":
                    {
                        ICommandHandler.ExpectArgsCount(args, 2);
                        var session = _sessionManager.GetSession(args[1]);
                        if (session == null)
                        {
                            Console.WriteLine($"Session {args[1]} not found!");
                            return;
                        }
                        _config.UsingSessionId = session.Id;
                        break;
                    }

                default:
                    {
                        throw new UnknownCommandException(args[0]);
                    }
            }
        }
    }
}
