using Microsoft.AspNetCore.SignalR;
using MyOverflow.Shared;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorMarkdig.Server.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub()
        {

        }



        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task StoreFile(byte[] rawData)
        {
            // write code to talk to MyOverflowApi

            using (var client = new HttpClient()) // should be instances ONCE per APPLICATION
            {
                //var response = await client.GetAsync("https://localhost:44394/MyOverflow/GetFile/123");

                try
                {
                    //var form = new MultipartFormDataContent();
                    var content = new ByteArrayContent(rawData);
                    //content.Headers.ContentDisposition = new ContentDispositionHeaderValue("file");
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    //form.Add(content, "file");

                    var response = await client.PostAsync("https://localhost:44394/MyOverflow/StoreFile", content);


                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine(responseBody);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task StoreImageFile(Shared.Models.ImageFile data)
        {
            using (var client = new HttpClient()) // should be instances ONCE per APPLICATION
            {
                await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreImageFile", data);


                //var content = JsonContent.Create(data);
                //var xxx = content.Headers.ContentType;
                //await client.PostAsync("https://localhost:44394/MyOverflow/StoreImageFile", content);
            }
        }

        public async Task<bool> StoreQuestion(Question question)
        {
            try
            {
                using (var client = new HttpClient()) // should be instances ONCE per APPLICATION
                {
                    await client.PostAsJsonAsync("https://localhost:44394/MyOverflow/StoreQuestion", question);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
