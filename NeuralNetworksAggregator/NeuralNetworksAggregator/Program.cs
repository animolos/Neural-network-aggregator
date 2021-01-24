using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Application;
using NeuralNetworksAggregator.Application.BotApplication;
using NeuralNetworksAggregator.Application.CmdApplication;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeuralNetworksAggregator
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener {Filter = new EventTypeFilter(SourceLevels.Error)});
            Trace.Listeners.Add(new ConsoleTraceListener {Filter = new EventTypeFilter(SourceLevels.Information)});
            // Trace.Listeners.Add(..)

            await BotApplication.Run(args);
        }

        public static void MainX(string[] args)
        {
            CmdApplication.Run(args);
        }
    }
}