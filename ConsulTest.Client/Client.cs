using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsulTest.Client
{
    class Client
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<string> Get(Uri uri)
        {
            var response = await this._client.GetAsync(uri.ToString());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Invalid response: {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
