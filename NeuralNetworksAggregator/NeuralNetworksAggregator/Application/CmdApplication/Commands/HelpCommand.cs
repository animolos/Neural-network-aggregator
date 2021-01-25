using System;
using System.IO;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class HelpCommand : ConsoleCommand
    {
        private readonly Lazy<ICommandsExecutor> executor;
        private readonly TextWriter writer;

        public HelpCommand(Lazy<ICommandsExecutor> executor, TextWriter writer)
            : base("help", "help      # prints available commands list")
        {
            this.executor = executor;
            this.writer = writer;
        }

        public override void ExecuteAsync(string[] args)
        {
            writer.WriteLine("Available commands: " +
                             string.Join(", ", executor.Value.GetAvailableCommandName()));
        }
    }
}