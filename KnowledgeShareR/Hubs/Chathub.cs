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
        
        public ChatHub(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public override async Task OnConnectedAsync()
        {
            string username = Context.User.Identity.Name;

            var optionsBuilder = new DbContextOptionsBuilder<KnowledgeShareDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext"));
            var context = new KnowledgeShareDbContext(optionsBuilder.Options);

            var isConnected = context.ConnectedUsers.Any(x => x.UserName == username);

            if (!isConnected)
            {
                await context.ConnectedUsers.AddAsync(new Models.ConnectedUser { ConnectionId = Context.ConnectionId, UserName = username });
                await context.SaveChangesAsync();
            }
            else
            {
                var currentUser = await context.ConnectedUsers.SingleAsync(x => x.UserName == username);
                currentUser.ConnectionId = Context.ConnectionId;
                context.ConnectedUsers.Update(currentUser);
                await context.SaveChangesAsync();
            }

            var allUsers = await context.ConnectedUsers.Select(x => x.UserName).ToListAsync();

            await Clients.All.SendAsync("OnConnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string username = Context.User.Identity.Name;

            var optionsBuilder = new DbContextOptionsBuilder<KnowledgeShareDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext"));
            var context = new KnowledgeShareDbContext(optionsBuilder.Options);

            var connectedUser = await context.ConnectedUsers.Where(x => x.UserName == username).ToListAsync();
            context.ConnectedUsers.RemoveRange(connectedUser);
            await context.SaveChangesAsync();

            var allUsers = await context.ConnectedUsers.Select(x => x.UserName).ToListAsync();

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
