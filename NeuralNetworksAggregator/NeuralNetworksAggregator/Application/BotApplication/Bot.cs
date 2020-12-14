using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Application.BotHandlers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;


namespace NeuralNetworksAggregator.Application
{
    public class Bot
    {
        private TelegramBotClient botClient;

        private readonly IHandler[] handlers;

        public Bot(IHandler[] handlers)
        {
            this.handlers = handlers;
        }

        public async Task Run(string[] args)
        {
            var key = Environment.GetEnvironmentVariable("NeuralNetworksAggregatorBotKey");
            Trace.Assert(key is not null);
            
            botClient = new TelegramBotClient(key);

            var me = await botClient.GetMeAsync();

            Console.Title = me.Username;

            var cts = new CancellationTokenSource();
            botClient.OnMessage += BotOnMessageReceivedAsync;
            botClient.OnCallbackQuery += BotOnCallbackQueryReceived;

            botClient.StartReceiving(
                Array.Empty<UpdateType>(),
                cts.Token
            );

            Trace.TraceInformation($"Начинаю слушать {me.Username}");
            Console.ReadLine();
            botClient.StopReceiving();
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        private async void BotOnMessageReceivedAsync(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null)
                return;

            var handlersToExecute = handlers
                .Select(handler => (handler.GetScore(message, botClient), handler))
                .Where(pair => pair.Item1 > 0)
                .OrderBy(pair => pair.Item1)
                .Take(4)
                .ToList();

            if (handlersToExecute.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "I can't understand you :(\nPlease, use /help.",
                    replyMarkup: new ReplyKeyboardRemove()
                );
                return;
            }
            //await SendInlineKeyboard(message);

            foreach (var (score, handler) in handlersToExecute)
            {
                // Trace.TraceInformation($"Score: {score}. Name: {handler.Name}");
                await handler.ExecuteAsync(message, botClient);
            }

            //async Task SendInlineKeyboard(Message message)
            //{
            //    await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            //    // Simulate longer running task
            //    await Task.Delay(500);

            //    var inlineKeyboard = new InlineKeyboardMarkup(new[]
            //    {
            //        // first row
            //        new []
            //        {
            //            InlineKeyboardButton.WithCallbackData("1.0"),
            //            InlineKeyboardButton.WithCallBackGame("1.1"),
            //            //InlineKeyboardButton.WithCallbackData("1.2", "12"),
            //        },
            //        // second row
            //        new []
            //        {
            //            InlineKeyboardButton.WithCallBackGame("2.0"),
            //            InlineKeyboardButton.WithCallBackGame("2.1"),
            //        }
            //    });
            //    await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Choose",
            //        replyMarkup: inlineKeyboard
            //    );
            //}

            //async Task SendReplyKeyboard(Message message)
            //{
            //    var replyKeyboardMarkup = new ReplyKeyboardMarkup(
            //        new KeyboardButton[][]
            //        {
            //            new KeyboardButton[] { "1.1", "1.2" },
            //            new KeyboardButton[] { "2.1", "2.2" },
            //        },
            //        resizeKeyboard: true
            //    );

            //    await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: "Choose",
            //        replyMarkup: replyKeyboardMarkup
            //    );
            //}
        }
    }

    public static class BotExtensions
    {
        public static async Task<string> DownloadPhotoFromMessage(this TelegramBotClient botClient, Message message)
        {
            var file = await botClient.GetFileAsync(message.Photo[^1].FileId);

            var filepath = Path.GetTempPath() + @$"\photo_{Guid.NewGuid()}.gif";

            await using var stream = new FileStream(filepath, FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, stream);

            return filepath;
        }

        public static async Task SendPictureFromSiteAsync(this TelegramBotClient botClient, Message message, byte[] bytes)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            await using var stream = new MemoryStream(bytes);

            await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputOnlineFile(stream),
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
    }
}