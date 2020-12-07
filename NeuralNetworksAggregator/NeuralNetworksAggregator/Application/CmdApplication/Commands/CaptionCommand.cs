using System.IO;

namespace NeuralNetworksAggregator.Application.CmdApplication.Commands
{
    public class CaptionCommand : ConsoleCommand
    {
        private readonly NeuralNetworksAggregator aggregator;
        private readonly TextWriter writer;

        public CaptionCommand(NeuralNetworksAggregator aggregator, TextWriter writer)
            : base("caption", "caption <filePath>      # get caption by photo")
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
            var caption = aggregator.GetCaptionPhotoFrom(args[0]);
            writer.WriteLine($"Caption:\n{caption}");
        }
    }
}