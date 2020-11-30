using System;
using System.Diagnostics;
using System.Formats.Asn1;

namespace NeuralNetworksAggregator
{
    public class CaptionGenerator
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

        public void FinishWork()
        {
            cmd.StandardInput.WriteLine("end");

            cmd.WaitForExit();
            cmd.Close();
        }

        //private static void OutputHandler(object sendingProcess,
        //    DataReceivedEventArgs outLine)
        //{
        //    if (!string.IsNullOrEmpty(outLine.Data))
        //        Console.WriteLine(outLine.Data);

        //}
    }
}