using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class WebCaptionGenerateHandler : IHandler
    {
        public string Name => "Web caption generator";
        public string Description => "get photo caption from website";

        private readonly WebCaptionGenerator generator;

        public WebCaptionGenerateHandler(WebCaptionGenerator generator)
        {
            this.generator = generator;
        }

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            var file = await botClient.GetFileAsync(message.Photo[^1].FileId);

            var stream = await botClient.DownloadFileAsStreamAsync(file.FilePath);

            var caption = await generator.GetCaption(stream);

            if (caption is null)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Sorry :(\nInternal HTTP server error",
                    replyMarkup: new ReplyKeyboardRemove()
                );
                return;
            }

            await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: message.Photo[^1].FileId,
                caption: caption,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        public double GetScore(Message message, TelegramBotClient botClient)
            => message.Type == MessageType.Photo ? 1 : 0;
    }
}