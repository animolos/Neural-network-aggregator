using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class HelpMessageHandler : IHandler
    {
        public string Name => "/help";
        public string Description => "prints available commands list";

        private readonly Lazy<IHandler[]> handlers;

        public HelpMessageHandler(Lazy<IHandler[]> handlers)
            => this.handlers = handlers;

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            var builder = new StringBuilder();
            foreach (var handler in handlers.Value)
                builder.Append($"{handler.Name} - {handler.Description}\n");

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: builder.ToString(),
                replyMarkup: new ReplyKeyboardRemove()
            );
        }


        public double GetScore(Message message, TelegramBotClient botClient)
        {
            if (message.Type != MessageType.Text && message.Type != MessageType.Photo)
                return 0;

            var text = message.Type == MessageType.Text ? message.Text : message.Caption;

            if (text is null)
                return 0;

            return Regex.IsMatch(text, @"\bhelp\b", RegexOptions.IgnoreCase) ? 1 : 0;
        }
    }
}
