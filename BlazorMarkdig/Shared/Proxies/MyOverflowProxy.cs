using BlazorMarkdig.Shared.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorMarkdig.Shared.Proxies
{
    public class MyOverflowProxy
    {
        public MyOverflowProxy(string uri)
        {
            this.uri = new Uri(uri);
        }

        private readonly Uri uri;
        private static readonly HttpClient client = new HttpClient();

        public async Task<Stream> GetFileAsync(string identifier)
        {
            return await client.GetStreamAsync($"{uri}/MyOverflow/GetFile/{identifier}");
        }

        public async Task<bool> StoreFileAsync(ImageFile file)
        {
            var content = new ByteArrayContent(file.Serialize());

            var response = await client.PostAsync($"{uri}/MyOverflow/StoreFile", content);

            return (response.IsSuccessStatusCode);
        }
    }
}
