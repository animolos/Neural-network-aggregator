using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using NeuralNetworksAggregator.Application.BotApplication;
using NeuralNetworksAggregator.Application.CmdApplication;

namespace NeuralNetworksAggregator
{
    internal class Program
    {
        public class Options
        {
            [Option('c', "console", HelpText = "Use to run console mode")]
            public bool Console { get; set; }
        }

        public static async Task Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener {Filter = new EventTypeFilter(SourceLevels.Error)});
            Trace.Listeners.Add(new ConsoleTraceListener {Filter = new EventTypeFilter(SourceLevels.Information)});
            //Trace.Listeners.Add(..)

            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o);

            if (options.Console)
                CmdApplication.Run(Array.Empty<string>());
            else
                await BotApplication.Run(Array.Empty<string>());
        }
    }
}