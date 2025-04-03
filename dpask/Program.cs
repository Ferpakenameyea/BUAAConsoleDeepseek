
using BUAADeepseekWebAPI;
using dpask;
using System.Diagnostics.CodeAnalysis;

var argument = new ArgsParser();
if (!argument.Parse(args))
{
    return 1;
}

var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var configPath = Path.Combine(folder, "dpask", "creds.txt");

var deepseek = new BUAADeepseek(new FileCredentialProvider(configPath));

try
{
    Console.WriteLine("AI assistant:");
    await deepseek.ChatAsync(argument.Question, response =>
    {
        Console.Write(response.Data?.Answer);
    }, useContext: false);
}
catch (HttpRequestException e)
{
    Console.WriteLine("Http error occured, maybe try to update your cookie");
    Console.WriteLine(e);
}
catch (Exception e)
{
    Console.WriteLine("Unexpected fatal exception");
    Console.WriteLine(e);
}

return 0;

class ArgsParser
{
    [AllowNull]
    public string Question { get; private set; }

    // dpask [some args] <question>
    public bool Parse(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return false;
        }
        Question = args[^1];
        return true;
    }

    private void PrintHelp()
    {
        Console.WriteLine("Usage: dpask [options] <question>");
    }
}