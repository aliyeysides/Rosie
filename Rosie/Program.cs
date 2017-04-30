using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace Rosie
{
    public class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;
        private CommandService _commands;
        private DependencyMap _map;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client.Log += Logger;

            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });

            _token = APIKeys.DiscordClientToken;

            _commands = new CommandService();
        }

        public async Task InitCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private Task Logger(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }

}