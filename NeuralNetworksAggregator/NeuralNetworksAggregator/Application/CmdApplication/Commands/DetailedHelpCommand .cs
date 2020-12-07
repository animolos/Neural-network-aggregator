using System;
using System.IO;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class DetailedHelpCommand : ConsoleCommand
    {
        private readonly Lazy<ICommandsExecutor> executor;
        private readonly TextWriter writer;

        public DetailedHelpCommand(Lazy<ICommandsExecutor> executor, TextWriter writer)
            : base("help", "help <command>      # prints help for <command>")
        {
            this.executor = executor;
            this.writer = writer;
        }

        public override void Execute(string[] args)
        {
            var commandName = args[0];
            var cmd = executor.Value.FindCommandByName(commandName);
            if (cmd != null)
                writer.WriteLine(cmd.Help);
            else
                writer.WriteLine("Not a command " + commandName);
        }
    }
}