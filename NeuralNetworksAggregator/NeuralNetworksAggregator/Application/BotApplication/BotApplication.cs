using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;
using Ninject;
using Ninject.Extensions.Conventions;

namespace NeuralNetworksAggregator.Application.BotApplication
{
    public static class BotApplication
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

        private static Bot CreateBot()
        {
            var container = new StandardKernel();
            container.Bind<Bot>().ToSelf()
                .InSingletonScope();

            container.Bind(c => c.FromThisAssembly().SelectAllClasses().BindAllInterfaces());
            container.Bind(c => c.FromThisAssembly().SelectAllClasses().BindAllBaseClasses());

            container.Bind<PythonCaptionGenerator>().ToSelf()
                .InSingletonScope();

            CaptionGeneratorInit(container.Get<PythonCaptionGenerator>());

            return container.Get<Bot>();
        }

        public static async Task Run(string[] args)
        {
            var bot = CreateBot();
            await bot.Run(args);
        }
    }
}