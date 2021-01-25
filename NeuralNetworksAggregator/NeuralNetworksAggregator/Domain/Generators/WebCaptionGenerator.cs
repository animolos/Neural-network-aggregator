using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeuralNetworksAggregator.Domain
{
    public class WebCaptionGenerator
    {
        public async Task<string> GetCaption(Stream stream)
        {
            using var httpResponse = await TryGetHttpResponse(stream, TimeSpan.FromSeconds(15));
            if (httpResponse is null)
                return null;

            using var reader = new StreamReader(await httpResponse.Content.ReadAsStreamAsync());

            var stringBuilder = new StringBuilder();
            var buffer = new char[100];
            while (true)
            {
                await reader.ReadBlockAsync(buffer, 0, 100);
                var content = string.Join("", buffer);
                stringBuilder.Append(content);
                if (stringBuilder.ToString().Contains("<end>"))
                    break;
            }

            return Regex.Match(stringBuilder.ToString(), @"<start> (.+) <end>").Groups[1].Value;
        }

        private async Task<HttpResponseMessage> TryGetHttpResponse(Stream stream, TimeSpan timeout)
        {
            HttpResponseMessage httpResponse;
            try
            {
                using var httpClient = new HttpClient { Timeout = timeout };
                httpResponse = await httpClient.PostAsync(
                    new Uri("http://35.154.182.61:26002/image-caption"),
                    new StreamContent(stream)
                );
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error while getting http response: {e.Message}");
                return null;
            }

            return httpResponse;
        }
    }
}
