using KnowledgeShareR.Data;
using KnowledgeShareR.Models;
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
    public class ChatHub : Hub
    {
        public IConfiguration Configuration { get; }
        public ChatHub(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("OnConnectedAsync", "OnConnectedAsync Fired");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("OnDisconnectedAsync", "OnDisconnectedAsync Fired");
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

        public async Task ConnectUser(string username)
        {
            var connectedUsers = new List<ConnectedUser>();
            var optionsBuilder = new DbContextOptionsBuilder<KnowledgeShareDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext"));
                
            using(var context = new KnowledgeShareDbContext(optionsBuilder.Options))
            {
                connectedUsers = await context.ConnectedUsers.ToListAsync();
            }
            await Clients.All.SendAsync("UserConnected", JsonConvert.SerializeObject(connectedUsers));
        }
    }
}
