using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;

namespace NeuralNetworksAggregator.Domain
{
    public abstract class BaseGenerator
    {
        private readonly string uriString;

        protected BaseGenerator(string uriString)
        {
            this.uriString = uriString;
        }

        public async Task<byte[]> GetBytes()
        {
            using var client = new HttpClient();

            return await client.GetByteArrayAsync(new Uri(uriString));
        }
    }
}
