using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMarkdig.Client.Hubs
{
    public class HubProxy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationManager">Needed to construct a full URI including domain name, from <paramref name="relativeUri"/>.</param>
        /// <param name="relativeUri">URI pointing at the named SignalR hub you want to communicate with. Relative paths work?  /charHub is default.</param>
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

                    Task.WaitAll(this.codeWaitingForIsReady.ToArray());
                });
            });
        }

        protected readonly HubConnection HubConnection;
        protected bool IsReady { get; private set; } = false;

        private List<Task> codeWaitingForIsReady = new List<Task>();

        public bool IsConnected => HubConnection.State == HubConnectionState.Connected;


        public async Task CallMethodWhenReady(string methodName, object arg1)
        {
            if(this.IsReady)
            {
                Task task = null;  
                try
                {
                    task = this.HubConnection.InvokeAsync(methodName, arg1);
                    await task;
                }
                catch (Exception ex)
                {
                    // when debugging, I'm seeing an error about not being able to evaluate children.
                    // Yet the task seems to be executing -- I think the error invovles the debugger itself?

                    // look at task status
                    if (task.IsFaulted == false)
                        await task;
                }
            }
            else
            {
                this.codeWaitingForIsReady.Add(new Task(async () => 
                {
                    await this.HubConnection.InvokeAsync(methodName, arg1);
                }));
            }
        }
    }
}
