using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Application.BotHandlers;
using NeuralNetworksAggregator.Application.CmdApplication;
using NeuralNetworksAggregator.Application.CmdApplication.CommandExecutor;
using NeuralNetworksAggregator.Application.CmdApplication.Commands;
using NeuralNetworksAggregator.Domain;
using Ninject;

namespace NeuralNetworksAggregator.Application.BotApplication
{
    public static class BotApplication
    {
        private static void CaptionGeneratorInit(CaptionGenerator generator)
        {
            Trace.TraceInformation("Start CaptionGenerator init...");
            try
            {
                generator.StartWork();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error while init CaptionGenerator: " + e.Message);
                return;
            }

            Trace.TraceInformation("CaptionGenerator init finished");
        }

        private static Bot CreateExecutor()
        {
            var container = new StandardKernel();
            container.Bind<Bot>().ToSelf()
                .InSingletonScope();

            container.Bind<IHandler>().To<CaptionGenerateHandler>();
            container.Bind<IHandler>().To<CatGenerateHandler>();
            container.Bind<IHandler>().To<ArtWorkGenerateHandler>();
            container.Bind<IHandler>().To<HelloUserHandler>();
            container.Bind<IHandler>().To<HelpMessageHandler>();
            // container.Bind<IHandler>().To<...>();

            container.Bind<BaseGenerator>().To<ArtWorkGenerator>();
            container.Bind<BaseGenerator>().To<CatGenerator>();

            container.Bind<CaptionGenerator>().ToSelf()
                .InSingletonScope();

            CaptionGeneratorInit(container.Get<CaptionGenerator>());

            return container.Get<Bot>();
        }

        public static async Task Run(string[] args)
        {
            var bot = CreateExecutor();
            await bot.Run(args);
        }
    }
}
