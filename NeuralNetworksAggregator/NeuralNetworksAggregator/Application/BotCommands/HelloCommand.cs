using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotCommands
{
    public class HelloCommand : ITextBotCommand
    {
        public string Command => "/hello";
        public string Description => "say hello to bot)";
        public async Task ExecuteCommandAsync(TelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Отправили что-то {message.Chat.Username}");
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Hello, {message.Chat.Username}!",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
    }
}