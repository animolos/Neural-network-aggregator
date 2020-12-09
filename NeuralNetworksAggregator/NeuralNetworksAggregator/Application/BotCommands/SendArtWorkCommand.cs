using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeuralNetworksAggregator.Application.BotCommands
{
    public class SendArtWorkCommand : ITextBotCommand
    {
        public string Command => "/art";
        public string Description => "get a picture of non-existing art work";
        public async Task ExecuteCommandAsync(TelegramBotClient botClient, Message message)
            => await botClient.SendPictureFromSiteAsync(message, "https://thisartworkdoesnotexist.com/");
    }
}