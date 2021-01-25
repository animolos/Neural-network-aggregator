using System.IO;
using NeuralNetworksAggregator.Domain;

namespace NeuralNetworksAggregator.Application.CmdApplication.Commands
{
    public class CaptionCommand : ConsoleCommand
    {
        private readonly PythonCaptionGenerator generator;
        private readonly TextWriter writer;

        public CaptionCommand(PythonCaptionGenerator generator, TextWriter writer)
            : base("caption", "caption <filePath>      # get caption by photo")
        {
            this.generator = generator;
            this.writer = writer;
        }

        public override void ExecuteAsync(string[] args)
        {
            if (args.Length != 1)
            {
                writer.WriteLine($"Wrong arguments. Try help {Name}");
                return;
            }
            var caption = generator.GetCaption(args[0]);
            writer.WriteLine($"Caption: {caption}");
        }
    }
}