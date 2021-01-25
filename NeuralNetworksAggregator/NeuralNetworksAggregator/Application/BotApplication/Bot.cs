using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using NeuralNetworksAggregator.Application.BotHandlers;
using NeuralNetworksAggregator.Infrastructure;
using NeuralNetworksAggregator.Infrastructure.Repository;

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

        private readonly AsyncLock asyncLock;

        private readonly Repository<Message> repository;

        public Bot(IHandler[] handlers)
        {
            this.handlers = handlers;
            asyncLock = new AsyncLock();
            repository = new Repository<Message>();
        }

        public async Task Run(string[] args)
        {
            var token = Environment.GetEnvironmentVariable("NeuralNetworksAggregatorBotKey");

            Trace.Assert(token is not null);

            botClient = new TelegramBotClient(token);
            var me = await botClient.GetMeAsync();
            Console.Title = me.Username;

            var cts = new CancellationTokenSource();
            botClient.OnMessage += BotOnMessageReceivedAsync;
            botClient.OnCallbackQuery += BotOnCallbackQueryReceived;

            botClient.StartReceiving(
                Array.Empty<UpdateType>(),
                cts.Token
            );

            Trace.TraceInformation($"Start listening {me.Username}");
            Console.ReadLine();
            botClient.StopReceiving();
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await botClient.DeleteMessageAsync(callbackQuery.From.Id, callbackQuery.Message.MessageId);

            var handlerName = callbackQuery.Data;
            var handlerToExecute = handlers.First(handler => handler.Name == handlerName);

            var message = repository.Get(callbackQuery.From.Username);

            await handlerToExecute.ExecuteAsync(message, botClient);

            asyncLock.Release(callbackQuery.From.Username);
        }

        private async void BotOnMessageReceivedAsync(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null)
                return;

            var username = message.Chat.Username;

            await asyncLock.AcquireLockAsync(username);

            var handlersToExecute = handlers
                .Select(handler => (handler.GetScore(message, botClient), handler))
                .Where(pair => pair.Item1 > 0)
                .OrderByDescending(pair => pair.Item1)
                .Take(4)
                .ToArray();

            if (handlersToExecute.Length == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "I can't understand you :(\nPlease, use /help.",
                    replyMarkup: new ReplyKeyboardRemove()
                );
                asyncLock.Release(username);
                return;
            }

            if (handlersToExecute.Length == 1)
            {
                await handlersToExecute.First().handler.ExecuteAsync(message, botClient);
                asyncLock.Release(username);
                return;
            }

            repository.CreateOrUpdate(username, message);

            await SendInlineKeyboard(message, handlersToExecute
                .Select(pair => pair.handler)
                .ToArray());
        }

        private async Task SendInlineKeyboard(Message message, IEnumerable<IHandler> handlersToExecute)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var inlineKeyboard = new InlineKeyboardMarkup(handlersToExecute
                .Select(handler =>
                    new[] {InlineKeyboardButton.WithCallbackData(handler.Description, handler.Name)})
                .ToArray());

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose action",
                replyMarkup: inlineKeyboard
            );
        }
    }

    public static class TelegramBotClientExtensions
    {
        private static readonly string botToken;

        static TelegramBotClientExtensions()
        {
            botToken = Environment.GetEnvironmentVariable("NeuralNetworksAggregatorBotKey");
        }

        public static async Task<Stream> DownloadFileAsStreamAsync(this TelegramBotClient _, string filePath)
        {
            var uriString = @$"https://api.telegram.org/file/bot{botToken}/{filePath}";
            using var client = new HttpClient();
            return await client.GetStreamAsync(new Uri(uriString));
        }

        public static async Task<string> DownloadPhotoFromMessageAsync(this TelegramBotClient botClient, Message message)
        {
            var file = await botClient.GetFileAsync(message.Photo[^1].FileId);

            var filepath = Path.GetTempPath() + @$"\photo_{Guid.NewGuid()}.gif";

            await using var stream = new FileStream(filepath, FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, stream);

            return filepath;
        }

        public static async Task SendPictureFromBytesAsync(this TelegramBotClient botClient, long chatId,
            byte[] bytes)
        {
            await botClient.SendChatActionAsync(chatId, ChatAction.UploadPhoto);

            await using var stream = new MemoryStream(bytes);

            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: new InputOnlineFile(stream),
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
    }

    public static class MessageExtensions
    {
        public static double GetMatch(this Message message, params string[] matchWords)
        {
            if (message.Type != MessageType.Text && message.Type != MessageType.Photo)
                return 0;

            var text = message.Type == MessageType.Text ? message.Text : message.Caption;

            if (text is null)
                return 0;

            return matchWords.Sum(word =>
                Regex.IsMatch(text, @$"\b{word}\b", RegexOptions.IgnoreCase) ? 1 : 0);
        }
    }
}