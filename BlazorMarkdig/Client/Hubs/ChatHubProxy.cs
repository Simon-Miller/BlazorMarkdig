using BlazorMarkdig.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace BlazorMarkdig.Client.Hubs
{
    public interface IChatHubProxy
    {
        Task SendImageFile(ImageFile data);
        Task SendQuestion(MyOverflow.Shared.Question question);
        Task SendImageFileBytes(byte[] data);
    }

    public class ChatHubProxy : HubProxy, IChatHubProxy
    {
        bool constructed = false;

        public ChatHubProxy(NavigationManager nav) : base(nav, "/chatHub")
        {
            this.constructed = true;
        }

        //public Task SendImageFile(ImageFile data) => base.HubConnection.SendAsync("StoreImageFile", data);

        public async Task SendImageFile(ImageFile data)
        {
            await base.CallMethodWhenReady("StoreImageFile", data);


            // can I talk to the server?



            //var task = Task.Run(async () => await base.HubConnection.SendAsync("storeImageFile", data)); // lowercase s
            //task.Wait();

            ////var task = Task.Run(async () => await base.HubConnection.SendAsync("StoreImageFile", data)); // uppercase S

            //return task;

            //var a = this.HubConnection;
            //var task = base.HubConnection.SendAsync("StoreImageFile", data);
            //return task;
        }

        public async Task SendImageFileBytes(byte[] data)
        {
            await base.CallMethodWhenReady("StoreFile", data);
        }

        public async Task SendQuestion(MyOverflow.Shared.Question question)
        {
            if (base.IsReady == false)
                throw new Exception("Not ready");

            bool guaranteedResponse = await base.HubConnection.InvokeAsync<bool>("StoreQuestion", question);

            if (guaranteedResponse == false)
            {
                // show some kind of error message, like "Please try again later" ?
            }
        }
    }
}
