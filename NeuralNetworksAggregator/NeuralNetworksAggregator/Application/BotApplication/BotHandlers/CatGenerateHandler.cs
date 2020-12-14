using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class CatGenerateHandler : IHandler
    {
        public string Name => "/cat";
        public string Description => "get a picture of non-existing cat";

        private readonly CatGenerator generator;

        public CatGenerateHandler(CatGenerator generator)
        {
            this.generator = generator;
        }

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            await botClient.SendPictureFromSiteAsync(message, await generator.GetBytes());
        }

        public double GetScore(Message message, TelegramBotClient botClient)
        {
            if (message.Type != MessageType.Text && message.Type != MessageType.Photo)
                return 0;

            var text = message.Type == MessageType.Text ? message.Text : message.Caption;

            if (text is null)
                return 0;

            return Regex.IsMatch(text, @"\bcat\b", RegexOptions.IgnoreCase) ? 1 : 0;
        }
    }
}
