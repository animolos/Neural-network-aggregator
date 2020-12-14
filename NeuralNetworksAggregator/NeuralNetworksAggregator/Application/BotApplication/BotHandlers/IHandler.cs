using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public interface IHandler
    {
        public string Name { get; }
        public string Description { get; }

        public double GetScore(Message message, TelegramBotClient botClient);
        public Task ExecuteAsync(Message message, TelegramBotClient botClient);
    }
}
