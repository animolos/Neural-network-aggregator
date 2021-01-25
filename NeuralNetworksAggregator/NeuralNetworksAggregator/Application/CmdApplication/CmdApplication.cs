using System;
using System.Diagnostics;
using System.IO;
using NeuralNetworksAggregator.Application.CmdApplication.CommandExecutor;
using NeuralNetworksAggregator.Domain;
using Ninject;
using Ninject.Extensions.Conventions;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class CmdApplication
    {
        private static void CaptionGeneratorInit(PythonCaptionGenerator generator)
        {
            Trace.TraceInformation("Start PythonCaptionGenerator init...");
            try
            {
                generator.StartWork();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error while init PythonCaptionGenerator: " + e.Message);
                return;
            }

            Trace.TraceInformation("PythonCaptionGenerator init finished");
        }

        private static ICommandsExecutor CreateExecutor()
        {
            var container = new StandardKernel();
            container.Bind<TextWriter>().ToConstant(Console.Out);
            container.Bind<ICommandsExecutor>().To<CommandsExecutor>()
                .InSingletonScope();

            container.Bind(c => c.FromThisAssembly().SelectAllClasses().BindAllBaseClasses());

            container.Bind<PythonCaptionGenerator>().ToSelf()
                .InSingletonScope();

            CaptionGeneratorInit(container.Get<PythonCaptionGenerator>());

            return container.Get<ICommandsExecutor>();
        }

        public static void Run(string[] args)
        {
            var executor = CreateExecutor();
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