using BlazorMarkdig.Shared.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorMarkdig.Shared.Proxies
{
    public interface IMyOverflowProxy
    {
        Task<Stream> GetFileAsync(string identifier);
        Task<bool> StoreFileAsync(ImageFile file);
    }

    public class MyOverflowProxy : IMyOverflowProxy
    {
        public MyOverflowProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private readonly HttpClient httpClient = new HttpClient();

        public async Task<Stream> GetFileAsync(string identifier)
        {
            return await httpClient.GetStreamAsync($"/MyOverflow/GetFile/{identifier}");
        }

        public async Task<bool> StoreFileAsync(ImageFile file)
        {
            var content = new ByteArrayContent(file.Serialize());

            // Close, but no cigar!  This is perhaps where we need Postman to observe it.
            var response = await httpClient.PostAsync("/MyOverflow/StoreFile", content);

            return (response.IsSuccessStatusCode);
        }
    }
}
