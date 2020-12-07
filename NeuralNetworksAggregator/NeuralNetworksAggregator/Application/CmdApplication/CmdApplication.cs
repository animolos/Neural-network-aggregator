using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Application.CmdApplication.CommandExecutor;
using NeuralNetworksAggregator.Application.CmdApplication.Commands;
using Ninject;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class CmdApplication
    {
        private static ICommandsExecutor CreateExecutor()
        {
            var container = new StandardKernel();
            container.Bind<TextWriter>().ToConstant(Console.Out);
            container.Bind<ICommandsExecutor>().To<CommandsExecutor>()
                .InSingletonScope();

            container.Bind<ConsoleCommand>().To<HelpCommand>();
            container.Bind<ConsoleCommand>().To<DetailedHelpCommand>();
            container.Bind<ConsoleCommand>().To<CatCommand>();
            container.Bind<ConsoleCommand>().To<CaptionCommand>();
            container.Bind<NeuralNetworksAggregator>().ToSelf().InSingletonScope();

            return container.Get<ICommandsExecutor>();
        }

        static void MainX(string[] args)
        {
            ICommandsExecutor executor = CreateExecutor();
            //if (args.Length > 0)
            //    executor.Execute(args);
            //else
            RunInteractiveMode(executor);
        }

        public static void RunInteractiveMode(ICommandsExecutor executor)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line == "exit")
                    return;
                executor.Execute(line.Split(' '));
            }
        }
    }
}
