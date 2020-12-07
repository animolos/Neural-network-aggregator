using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeuralNetworksAggregator.Application.BotCommands
{
    public class SendCatCommand : ITextBotCommand
    {
        public string Command => "/cat";
        public string Description => "get a picture of non-existing cat";

        public async Task ExecuteCommandAsync(TelegramBotClient botClient, Message message)
            => await botClient.SendPictureFromSiteAsync(message, "https://thiscatdoesnotexist.com/");
    }
}