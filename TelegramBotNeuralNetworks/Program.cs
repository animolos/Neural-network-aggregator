using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using NeuralNetworks;

namespace TelegramBotNeuralNetworks
{
    class Program
    {
        private static CaptionGenerator captionGenerator;
        private static TelegramBotClient botClient;

        private static void CaptionGeneratorInit()
        {
            Console.WriteLine("Инициализация генератора описаний...");
            captionGenerator = new CaptionGenerator();
            captionGenerator.Start();
            Console.WriteLine("Инициализация завершена.");
        }

        static async Task Main(string[] args)
        {
            CaptionGeneratorInit();

            const string key = "1304989664:AAFzMaU3smtiydm6eUMkG4EdAdoxPtmxdug";
            botClient = new TelegramBotClient(key);
            var me = await botClient.GetMeAsync();

            Console.Title = me.Username;

            var cts = new CancellationTokenSource();
            botClient.OnMessage += BotOnMessageReceivedAsync;
            // botClient.OnMessageEdited:
            // botClient.OnCallbackQuery:
            // botClient.OnInlineQuery:
            // botClient.OnInlineResultChosen:
            // botClient.OnReceiveError:
            
            botClient.StartReceiving(
                Array.Empty<UpdateType>(),
                cts.Token
            );

            Console.WriteLine($"Начинаю слушать {me.Username}");
            Console.ReadLine();
            botClient.StopReceiving();
        }
        
        private static async void BotOnMessageReceivedAsync(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null)
                return;

            switch (message.Type)
            {
                case MessageType.Text:
                {
                    var action = message.Text.Split(' ').First() switch
                    {
                        "/hello" => SendMessageAsync(message, $"Привет, {message.Chat.Username}!"),
                        "/picture" => SendFileAsync(message),
                        _ => UsageAsync(message)
                    };
                    await action;
                    break;
                }
                case MessageType.Photo:
                {
                    var file = await botClient.GetFileAsync(message.Photo[^1].FileId);

                    var filepath = Path.GetTempPath() + @$"\photo_{DateTime.Now:MM_dd_yyyy_HH_mm_ss}.gif";

                    await using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await botClient.DownloadFileAsync(file.FilePath, stream);
                    }

                    var caption = captionGenerator.GetCaption(filepath);

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

                    Console.WriteLine($"Отправил обратно фотографию к {message.Chat.Username} с описанием:\n{caption}");

                    System.IO.File.Delete(filepath);

                    break;
                }
                default:
                    return;
            }

            static async Task SendMessageAsync(Message message, string str)
            {
                Console.WriteLine($"Отправили что-то {message.Chat.Username}");
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: str,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            static async Task SendFileAsync(Message message)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
                
                Console.WriteLine($"Отправили картинку {message.Chat.Username}");
                await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: "https://thiscatdoesnotexist.com/",
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            static async Task UsageAsync(Message message)
            {
                Console.WriteLine($"С нами пытается общаться {message.Chat.Username}, он написал \"{message.Text}\"");
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "/hello - поздороваться с ботом :)\n" +
                          "/picture - одинаковый котик\nОтправьте фото, чтобы получить его обратно (временно)",
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }
    }
}