using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeuralNetworksAggregator.Application.BotCommands
{
    public class SendPersonCommand : ITextBotCommand
    {
        public string Command => "/person";
        public string Description => "get a picture of non-existing person";
        public async Task ExecuteCommandAsync(TelegramBotClient botClient, Message message)
            => await botClient.SendPictureFromSiteAsync(message, "https://thisartworkdoesnotexist.com/");
    }
}