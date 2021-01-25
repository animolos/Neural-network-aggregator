using System.IO;
using NeuralNetworksAggregator.Domain;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class ArtWorkCommand : ConsoleCommand
    {
        private readonly ArtWorkGenerator generator;
        private readonly TextWriter writer;

        public ArtWorkCommand(ArtWorkGenerator generator, TextWriter writer)
            : base("art", "art <directoryPath>      # downloads art-work")
        {
            this.generator = generator;
            this.writer = writer;
        }

        public override async void ExecuteAsync(string[] args)
        {
            if (args.Length != 1)
            {
                await writer.WriteLineAsync($"Wrong arguments. Try help {Name}");
                return;
            }

            var bytes = await generator.GetBytes();
            await File.WriteAllBytesAsync(args[0], bytes);

            await writer.WriteLineAsync($"Art downloaded successfully.\nFull path to file: {args[0]}");
        }
    }
}