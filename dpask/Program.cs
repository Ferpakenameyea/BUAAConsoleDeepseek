
using BUAADeepseekWebAPI;
using BUAADeepseekWebAPI.Exceptions;
using dpask;
using dpask.Exceptions;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

var runtimeConfig = new RuntimeConfig(
    new ConstantPathProvider(Path.Combine(SystemConstants.PersistPathRoot, "config")));
var sessionManager = new SessionManager(
    new ConstantPathProvider(Path.Combine(SystemConstants.PersistPathRoot, "sessions")), runtimeConfig);

using var contextManager = new FileContextManager(
    new DelegatePathProvider(() =>
    {
        var session = sessionManager.GetSession(runtimeConfig.UsingSessionId);
        session ??= sessionManager.CreateSession("system-created", "unnamed session");
        runtimeConfig.UsingSessionId = session.Id;
        return sessionManager.GetSessionHistoryPath(session);
    }));

var deepseek = new BUAADeepseek(
        new FileCredentialProvider(SystemConstants.PersistPathRoot + "/creds.txt"),
        contextManager);

var injectionManager = new InjectionManager(
    deepseek,
    new ConstantPathProvider(Path.Combine(SystemConstants.PersistPathRoot, "injections")),
    runtimeConfig);

string[] features = [
    "help", "hp",
    "history", "h",
    "clear", "c",
    "sessions", "s", "session",
    "injections", "i", "injection",
];

try
{
    if (features.Contains(args[0]))
    {
        try
        {
            // dispatch to feature
            switch (args[0])
            {
                case "history":
                case "h":
                case "clear":
                case "c":
                    new HistoryCommandHandler(contextManager).Dispatch(args);
                    break;
                case "help":
                case "hp":
                    Console.WriteLine(SystemConstants.LongHelpMessage);
                    break;

                case "sessions":
                case "session":
                case "s":
                    new SessionCommandHandler(sessionManager, runtimeConfig).Dispatch(args[1..]);
                    break;

                case "injections":
                case "injection":
                case "i":
                    await new InjectionCommandHander(
                        injectionManager, 
                        sessionManager, 
                        runtimeConfig).DispatchAsync(args[1..]);
                    break;
                default:
                    Console.WriteLine("This feature is still in development!");
                    break;
            }
            return 0;
        }
        finally
        {
            runtimeConfig.Persist();
        }
    }
}
catch(UnknownCommandException e)
{
    Console.WriteLine(e.Message);
    return -1;
}
catch(ArgumentException e)
{
    if (e.ParamName == "")
    {
        Console.WriteLine("Your command is missing arguments");
        return -1;
    }

    Console.WriteLine($"Argument {e.ParamName} is unexpected for command {args[0]}");
    return -1;
}
catch(Exception e)
{
    Console.WriteLine("Unexpected fatal exception");
    Console.WriteLine(e);
    return -1;
}


var argument = new AIInteractionArgParser();
if (!argument.Parse(args))
{
    return 1;
}

try
{
    try
    {
        Console.WriteLine("AI assistant:");
        await deepseek.ChatAsync(argument.Question, response =>
        {
            Console.Write(response.Data?.Answer);
        }, useContext: argument.UseContext);
    }
    catch (OutdatedException)
    {
        Console.WriteLine("Your software is outdated!");
        return -1;
    }
    catch (JsonSerializationException)
    {
        Console.WriteLine("Json serialization error, " +
            "your credential might be expired or your " +
            "software might be out-dated");
        return -1;
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("Http error occured, maybe try to update your cookie");
        Console.WriteLine(e);
        return -1;
    }
    catch (Exception e)
    {
        Console.WriteLine("Unexpected fatal exception");
        Console.WriteLine(e);
        return -1;
    }

    return 0;
}
finally
{
    runtimeConfig.Persist();
    var currentSession = sessionManager.GetSession(runtimeConfig.UsingSessionId);
    if (currentSession != null)
    {
        sessionManager.PersistChange(currentSession);
    }
}


internal class AIInteractionArgParser
{
    [AllowNull]
    public string Question { get; private set; }

    public bool UseContext { get; private set; } = true;

    // dpask [some args] <question>
    public bool Parse(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return false;
        }

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--no-context")
            {
                UseContext = false;
            }
            else
            {
                Console.WriteLine($"Unknown argument: {args[i]}");
                PrintHelp();
                return false;
            }
        }

        Question = args[^1];
        return true;
    }

    private void PrintHelp()
    {
        Console.WriteLine(SystemConstants.ShortHelpMessage);
    }
}