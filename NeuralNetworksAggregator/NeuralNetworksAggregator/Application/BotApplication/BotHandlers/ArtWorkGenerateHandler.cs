using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class ArtWorkGenerateHandler : IHandler
    {
        public string Name => "/art";
        public string Description => "get a picture of non-existing art work";

        private readonly ArtWorkGenerator generator;

        public ArtWorkGenerateHandler(ArtWorkGenerator generator)
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

            return Regex.IsMatch(text, @"\bart\b", RegexOptions.IgnoreCase) ? 1 : 0;
        }
    }
}
