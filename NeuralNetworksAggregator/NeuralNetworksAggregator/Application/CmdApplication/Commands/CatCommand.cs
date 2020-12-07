using System;
using System.IO;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class CatCommand : ConsoleCommand
    {
        private readonly NeuralNetworksAggregator aggregator;
        private readonly TextWriter writer;

        public CatCommand(NeuralNetworksAggregator aggregator, TextWriter writer)
            : base("cat", "cat <directoryPath>      # downloads cat")
        {
            this.aggregator = aggregator;
            this.writer = writer;
        }

        public override void Execute(string[] args)
        {
            if (args.Length != 1)
            {
                writer.WriteLine($"Wrong arguments. Try help {Name}");
            }
            var filePath = aggregator.DownloadCatTo(args[0]);
            writer.WriteLine($"Cat downloaded successfully. Full path to file:\n{filePath}");
        }
    }
}