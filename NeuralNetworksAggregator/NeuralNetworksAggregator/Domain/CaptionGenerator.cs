using System;
using System.Diagnostics;

namespace NeuralNetworksAggregator.Domain
{
    public class CaptionGenerator : IDisposable
    {
        private readonly Process cmd;

        public CaptionGenerator()
        {
            var pythonPath = Environment.GetEnvironmentVariable("NeuralNetworksAggregatorPythonPath");
            Trace.Assert(pythonPath is not null);
            var scriptPath = Environment.GetEnvironmentVariable("NeuralNetworksAggregatorCaptionGeneratorScriptPath");
            Trace.Assert(scriptPath is not null);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = scriptPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            cmd = new Process { StartInfo = processStartInfo };
        }

        public void StartWork()
        {
            cmd.Start();

            var loadingResult = cmd.StandardOutput.ReadLine();
            if (loadingResult != "loaded")
                throw new Exception("Error while loading");
        }
        
        public string GetCaption(string path)
        {
            cmd.StandardInput.WriteLine(path);
            var result = cmd.StandardOutput.ReadLine();
            return result;
        }

        private bool isDisposed = false;

        ~CaptionGenerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing) { }

                cmd?.Dispose();

                isDisposed = true;
            }
        }
    }
}