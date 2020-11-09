using System;
using System.Diagnostics;

namespace NeuralNetworks
{
    public class CaptionGenerator
    {
        private readonly Process cmd;

        public CaptionGenerator(
            string pythonPath = @"C:\Users\motgo\AppData\Local\Programs\Python\Python36\python.exe",
            string scriptPath = @"C:\Users\motgo\OneDrive\Python\Image-Caption-Generator-master\start.py"
        )
        {
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

        public void Start()
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

        public void Close()
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