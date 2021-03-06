﻿using System.IO;
using NeuralNetworksAggregator.Domain;

namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public class CatCommand : ConsoleCommand
    {
        private readonly CatGenerator generator;
        private readonly TextWriter writer;

        public CatCommand(CatGenerator generator, TextWriter writer)
            : base("cat", "cat <directoryPath>      # downloads cat")
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

            var bytes = await generator.GetBytes();
            await File.WriteAllBytesAsync(args[0], bytes);

            await writer.WriteLineAsync($"Cat downloaded successfully.\nFull path to file: {args[0]}");
        }
    }
}