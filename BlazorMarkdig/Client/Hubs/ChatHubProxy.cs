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
    }

    public class ChatHubProxy : HubProxy, IChatHubProxy
    {
        public ChatHubProxy(NavigationManager nav) : base(nav, "/chatHub")
        { }

        public Task SendImageFile(ImageFile data) => base.HubConnection.SendAsync("StoreImageFile", data);

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
