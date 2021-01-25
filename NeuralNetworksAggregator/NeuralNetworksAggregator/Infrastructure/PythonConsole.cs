using System;
using System.Diagnostics;

namespace NeuralNetworksAggregator.Infrastructure
{
    public class PythonConsole : IDisposable
    {
        private readonly Process cmd;

        public PythonConsole(string pythonPath, string scriptPath)
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
            => cmd.Start();

        public string ReadLine()
            => cmd.StandardOutput.ReadLine();

        public void WriteLine(string line)
            => cmd.StandardInput.WriteLine(line);

        #region Disposable Pattern

        private bool isDisposed = false;

        ~PythonConsole()
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

        #endregion
    }
}
