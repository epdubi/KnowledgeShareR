using KnowledgeShareR.Data;
using KnowledgeShareR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeShareR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public IConfiguration Configuration { get; }

        private static HashSet<string> connectedUsers = new HashSet<string>();

        public ChatHub(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public override async Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            connectedUsers.Add(name);

            var allUsers = connectedUsers.Select(x => x);

            await Clients.All.SendAsync("OnConnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
             string name = Context.User.Identity.Name;

            connectedUsers.Remove(name);

            var allUsers = connectedUsers.Select(x => x);

            await Clients.All.SendAsync("OnDisconnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task CountDown()
        {
            var numbers = Enumerable.Range(0, 10).Select(x => x);
            foreach(var num in numbers)
            {
                await Task.Delay(1200);
                await Clients.All.SendAsync("CountDownReceived", num.ToString());  
            }
        }
    }
}
