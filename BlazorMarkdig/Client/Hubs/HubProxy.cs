using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace BlazorMarkdig.Client.Hubs
{
    public class HubProxy
    {
        public HubProxy(NavigationManager navigationManager, string relativeUri = "/chatHub")
        {
            this.HubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri(relativeUri)) // NOTE: This is setup in the server-side startup in endpoint configurations.
                .Build();

            Task.Run(async () =>
            {
                await this.HubConnection.StartAsync().ContinueWith(t =>
                {
                    this.IsReady = true;
                });
            });
        }

        protected readonly HubConnection HubConnection;
        protected bool IsReady { get; private set; } = false;

        public bool IsConnected => HubConnection.State == HubConnectionState.Connected;
    }
}
