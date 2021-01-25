using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class HelpMessageHandler : IHandler
    {
        public string Name => "/help";
        public string Description => "print available commands list";

        private readonly Lazy<IHandler[]> handlers;

        public HelpMessageHandler(Lazy<IHandler[]> handlers)
            => this.handlers = handlers;

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            var builder = new StringBuilder();
            foreach (var handler in handlers.Value.OrderBy(handler => handler.Name))
                builder.Append($"{handler.Name} - {handler.Description}\n");

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: builder.ToString(),
                replyMarkup: new ReplyKeyboardRemove()
            );
        }


        public double GetScore(Message message, TelegramBotClient botClient)
            => message.GetMatch("help", "start", "помогите", "помощь", "хелп", "бля");
    }
}
