using System;
using NeuralNetworksAggregator.Infrastructure;

namespace NeuralNetworksAggregator.Domain
{
    public class PythonCaptionGenerator : PythonConsole
    {
        public PythonCaptionGenerator() : base(
            Environment.GetEnvironmentVariable("NeuralNetworksAggregatorPythonPath"),
            Environment.GetEnvironmentVariable("NeuralNetworksAggregatorCaptionGeneratorScriptPath"))
        { }

        public void StartWork()
        {
            Start();

            var loadingResult = ReadLine();
            if (loadingResult != "loaded")
                throw new Exception("Error while loading caption generator");
        }
        
        public string GetCaption(string path)
        {
            WriteLine(path);
            var result = ReadLine();
            return result;
        }
    }
}