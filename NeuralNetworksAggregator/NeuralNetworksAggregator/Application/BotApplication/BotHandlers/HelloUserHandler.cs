using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class HelloUserHandler : IHandler
    {
        public string Name => "/hello";
        public string Description => "say hello to bot)";

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Hello, {message.Chat.Username}!",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        public double GetScore(Message message, TelegramBotClient botClient)
            => message.GetMatch("hello", "привет", "hi", "hey", "хей", "хай");
    }
}
