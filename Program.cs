
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DNet_V3_Bot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = "MTAyMDM4MDQwMTkzNzY5NDgyMQ.G8v95n.CQO9asE7ZfRPw8FaSJKN42oQ5lSUWnB_i3Z3MQ";

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;
            await Announce(message.Channel.Id);
            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
        public async Task Announce(ulong channelId)
        {
            var chnl = _client.GetChannel(channelId) as IMessageChannel;
            await chnl.SendMessageAsync("پیشنهاد خرید ویژه جدید(انجام شد✅)\r\n\r\n\r\n🔸نام سرور مورد نیاز: thekal -Alliance       \r\n\r\n🔸مقدار مورد نیاز: 4,500\r\n\r\n🔸قیمت هر گلد به تومان :380\r\n\r\n🗓1401/06/22\r\n\r\nبرای فروش به آیدی زیر همین حالا پیام دهید👇👇\r\n\r\nhttps://t.me/wowgshop_sup3");
        }
    }
}