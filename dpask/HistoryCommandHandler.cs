using BUAADeepseekWebAPI;
using dpask.Exceptions;

namespace dpask
{
    public class HistoryCommandHandler : ICommandHandler
    {
        private readonly IContextManager _contextManager;

        public HistoryCommandHandler(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public void Dispatch(string[] args)
        {
            ICommandHandler.ExpectArgsCount(args, 1);

            switch (args[0])
            {
                case "history":
                case "h":
                    Console.WriteLine(SystemConstants.Seperator);
            
                    foreach (var (user, system) in _contextManager.Enumerate())
                    {
                        Console.WriteLine($"""
                        User:
                        > {user.Content}

                        AI Assistant:
                        > {system.Content}
                        """);

                        Console.WriteLine(SystemConstants.Seperator);
                    }
                    break;

                case "clear":
                case "c":
                    _contextManager.ClearHistory();
                    _contextManager.PersistHistories();
                    break;

                default:
                    throw new UnknownCommandException(args[0]);
            }
        }
    }
}
