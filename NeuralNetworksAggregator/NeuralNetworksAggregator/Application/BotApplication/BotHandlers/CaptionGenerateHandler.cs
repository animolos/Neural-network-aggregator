using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Application.BotHandlers
{
    public class CaptionGenerateHandler : IHandler
    {
        public string Name => "Caption generator";
        public string Description => "send me photo to get caption";

        private readonly CaptionGenerator generator;

        public CaptionGenerateHandler(CaptionGenerator generator)
        {
            this.generator = generator;
        }

        public async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            var filepath = await botClient.DownloadPhotoFromMessage(message);

            var caption = generator.GetCaption(filepath);

            var fileName = filepath.Split(Path.DirectorySeparatorChar).Last();
            await using (var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: caption,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            System.IO.File.Delete(filepath);
        }

        public double GetScore(Message message, TelegramBotClient botClient)
            => message.Type == MessageType.Photo ? 1 : 0;
    }
}
