using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            await botClient.SendPictureFromBytesAsync(message.Chat.Id, await generator.GetBytes());
        }

        public double GetScore(Message message, TelegramBotClient botClient)
            => message.GetMatch("art", "picture", "image", "artwork", "painting", "арт", "картина", "пикча");
    }
}
