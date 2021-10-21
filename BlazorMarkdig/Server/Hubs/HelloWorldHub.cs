using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorMarkdig.Server.Hubs
{
    public class HelloWorldHub : Hub
    {
        // I'm baffled!  This 'hello world' got called, but the real SignalR hub has not.

        public Task<string> HelloWorld()
        {
            return Task.Run(() => "Hello World!");
        }

        public async Task StoreImageFile(BlazorMarkdig.Shared.Models.ImageFile data)
        {
            using (var client = new HttpClient()) // should be instances ONCE per APPLICATION
            {
                await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreImageFile", data);


                //var content = JsonContent.Create(data);
                //var xxx = content.Headers.ContentType;
                //await client.PostAsync("https://localhost:44394/MyOverflow/StoreImageFile", content);
            }
        }

        public async Task StoreImageFileBytes(byte[] data)
        {
            var obj = BlazorMarkdig.Shared.Models.ImageFile.Deserialize(new MemoryStream(data));

            var objDto = BlazorMarkdig.Shared.Models.ImageFileDTO.From(obj); // handles the RawData making it Base64 encoded string.  That's better for JSON!

            var proofShouldWorkAtOtherEnd = BlazorMarkdig.Shared.Models.ImageFile.From(objDto);

            try
            {
                //var ms = new MemoryStream(data);
                //using HttpClient _httpClient = new();
                //using var request = new HttpRequestMessage()
                //{
                //    Method = HttpMethod.Post,
                //    RequestUri = new Uri("YOUR_DESTINATION_URI"),
                //    Content = new StreamContent(ms),                    // NOTE: This is only streaming the bytes, not the ImageFile.  We need to stream the ImageFile!!!
                //};
                //using var response = await _httpClient.SendAsync(request);
                //// TODO check response status etc.




                // this method doesn't work.  We get empty data at other end.
                //using (var client = new HttpClient())
                //    await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreImageFile", data);


                //using (var client = new HttpClient())
                //    await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreFile", data);

                using (var client = new HttpClient())
                    await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreImageFile2", objDto);


                // holy crap!  No errors?  But not getting there.  argh!!  At least we got a breakpoint hit with the above!
                // OK, when we remove the "[FromBody]" attribute, we get to the controller action, but model binding fails.
                // same reason as above, perhaps?
                //using(var client = new HttpClient())
                //{
                //    var json = System.Text.Json.JsonSerializer.Serialize(obj);
                //    var content = new StringContent(json);

                //    await client.PostAsync("https://localhost:44394/MyOverflow/StoreImageFile", content);
                //}

            }
            catch (Exception ex)
            {

            }
        }
    }
}
