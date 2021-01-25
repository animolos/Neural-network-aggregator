using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            await botClient.SendPictureFromBytesAsync(message.Chat.Id, await generator.GetBytes());
        }

        public double GetScore(Message message, TelegramBotClient botClient)
            => message.GetMatch("cat", "kitten", "kitty", "кот", "котэ", "котенок", "котик", "кота", "котика", "киса");
    }
}
