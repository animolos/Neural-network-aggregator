using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworksAggregator.Domain;

namespace NeuralNetworksAggregator.Application
{
    public class NeuralNetworksAggregator
    {
        private CaptionGenerator captionGenerator { get; }

        public NeuralNetworksAggregator()
        {
            captionGenerator = new CaptionGenerator();
            CaptionGeneratorInit();
        }

        private void CaptionGeneratorInit()
        {
            Console.WriteLine("Start CaptionGenerator init...");
            try
            {
                captionGenerator.StartWork();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while init CaptionGenerator: " + e.Message);
                return;
            }

            Console.WriteLine("CaptionGenerator init finished");
        }

        public string GetCaptionPhotoFrom(string path)
            => captionGenerator.GetCaption(path);

        public string DownloadCatTo(string directoryPath)
            => default; //CatGenerator.DownloadCatTo(directoryPath);
    }
}
