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
                MessageCacheSize = 1000
            });

            _token = APIKeys.DiscordClientToken;

            _commands = new CommandService();
        }

        public async Task InitCommands()
        {
            _client.Log += Logger;
            _client.MessageDeleted += MessageDeleted;
            _client.MessageUpdated += MessageUpdated;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private Task Logger(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> before, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            await channel.SendMessageAsync($"Someone deleted \"{message}\" in {channel}");
            Console.WriteLine($"Someone deleted \"{message}\" in {channel}");
        }

    }

}