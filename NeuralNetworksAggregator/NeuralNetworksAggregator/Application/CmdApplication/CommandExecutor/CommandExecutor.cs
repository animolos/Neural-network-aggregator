using System;
using System.IO;
using System.Linq;

namespace NeuralNetworksAggregator.Application.CmdApplication.CommandExecutor
{
    public class CommandsExecutor : ICommandsExecutor
    {
        private readonly TextWriter writer;
        private readonly ConsoleCommand[] commands;

        public CommandsExecutor(ConsoleCommand[] commands, TextWriter writer)
        {
            this.commands = commands;
            this.writer = writer;
        }

        public string[] GetAvailableCommandName()
        {
            return commands.Select(c => c.Name).ToArray();
        }

        public ConsoleCommand FindCommandByName(string name)
        {
            return commands.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public void Execute(string[] args)
        {
            if (args.Length == 0)
                writer.WriteLine("Please specify <command> as the first command line argument");
            var commandName = args[0];
            var cmd = FindCommandByName(commandName);
            if (cmd == null)
            {
                writer.WriteLine("Sorry. Unknown command {0}", commandName);
                return;
            }
            
            try
            {
                cmd.Execute(args.Skip(1).ToArray());
            }
            catch (Exception e)
            {
                writer.WriteLine($"Error while executing command {commandName}: {e.Message}");
            }
        }
    }
}
