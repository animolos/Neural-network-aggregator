using System.IO;
using NeuralNetworksAggregator.Domain;

namespace NeuralNetworksAggregator.Application.CmdApplication.Commands
{
    public class WebCaptionCommand : ConsoleCommand
    {
        private readonly WebCaptionGenerator generator;
        private readonly TextWriter writer;

        public WebCaptionCommand(WebCaptionGenerator generator, TextWriter writer)
            : base("web_caption", "web_caption <filePath>      # get photo caption from website")
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

            await using var stream = File.OpenRead(args[0]);
            var caption = await generator.GetCaption(stream);

            await writer.WriteLineAsync($"Caption: {caption}");
        }
    }
}