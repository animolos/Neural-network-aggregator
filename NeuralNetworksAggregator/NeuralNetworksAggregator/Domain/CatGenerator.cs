using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator.Domain
{
    public class CatGenerator
    {
        private static readonly string uriString = "https://thiscatdoesnotexist.com/";

        public static string DownloadCatTo(string directoryPath)
        {
            var filePath = directoryPath + @$"\photo_{Guid.NewGuid()}.gif";
            var webClient = new WebClient();
            webClient.DownloadFile(new Uri(uriString), filePath);
            return filePath;
        }
    }
}
