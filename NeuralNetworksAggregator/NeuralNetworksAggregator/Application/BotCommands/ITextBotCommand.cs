using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeuralNetworksAggregator.Application.BotCommands
{
    public interface ITextBotCommand
    {
        string Command { get; }
        string Description { get; }
        Task ExecuteCommandAsync(TelegramBotClient botClient, Message message);
    }
}