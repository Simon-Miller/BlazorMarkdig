using BlazorMarkdig.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorMarkdig.Server.Controllers
{
    [ApiController]
    public class MyOverflowProxyController : ControllerBase
    {
        public MyOverflowProxyController(HttpClient realHttpClient)
        {
            this.realHttpClient = realHttpClient;
        }

        private readonly HttpClient realHttpClient;

        [Route("MyOverflowProxy/GetFile/{*identifier}")]
        [HttpGet]
        public async Task<ActionResult> GetFileAsync(string identifier)
        {
            // TODO: This is ok for now - - but we should be able to access the file directly from Blob storage with public URL.
            using (var stream = await this.realHttpClient.GetStreamAsync($"/MyOverflow/GetFile/{identifier}"))
            {
                //var bytes = new byte[stream.Length];
                //stream.Read(bytes, 0, (int)stream.Length);

                var info = ImageFile.Deserialize(stream);

                using (var ms = new MemoryStream(info.RawData))
                {
                    return new FileStreamResult(ms, info.MimeType);
                }
            }
        }

        [Route("MyOverflowProxy/StoreFile")]
        [HttpPost]
        public ActionResult<bool> StoreFileAsync([FromBody] byte[] imageData)
        {
            //var content = new ByteArrayContent(file.Serialize());
            var content = new ByteArrayContent(imageData);

            // Close, but no cigar!  This is perhaps where we need Postman to observe it.
            var task = this.realHttpClient.PostAsync("/MyOverflow/StoreFile", content);
            task.Wait();

            return new ActionResult<bool> (task.Result.IsSuccessStatusCode);
        }

        [Route("MyOverflowProxy/Get")]
        [HttpGet]
        public IEnumerable<bool> Get()
        {
            return new bool[] { true, false };
        }
    }
}
