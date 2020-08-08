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
    public class KnowledgeShareRHub : Hub
    {
        public IConfiguration Configuration { get; }

        private readonly KnowledgeShareDbContext _db;

        private readonly Dictionary<int, string> AlphabetDict = new Dictionary<int, string> { { 0, "a. " }, { 1, "b. " }, { 2, "c. " }, { 3, "d. " }, { 4, "e. " }, { 5, "f. " } };

        public KnowledgeShareRHub(IConfiguration configuration, KnowledgeShareDbContext dbContext)
        {
            Configuration = configuration;
            _db = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;

            var isExisting = _db.ConnectedUsers.Any(x => x.AspNetUserId == userId);

            if (!isExisting)
            {
                await _db.ConnectedUsers.AddAsync(new Models.ConnectedUser { AspNetUserId = Context.UserIdentifier, ConnectionId = Context.ConnectionId, UserName = Context.User.Identity.Name, IsDisconnected = false });
                await _db.SaveChangesAsync();
            }
            else
            {
                var currentUser = await _db.ConnectedUsers.SingleAsync(x => x.AspNetUserId == userId);
                currentUser.ConnectionId = Context.ConnectionId;
                currentUser.IsDisconnected = false;
                _db.ConnectedUsers.Update(currentUser);
                await _db.SaveChangesAsync();
            }

            var allUsers = await _db.ConnectedUsers.Where(x => !x.IsDisconnected).Select(x => x.UserName).ToListAsync();

            await Clients.All.SendAsync("OnConnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.UserIdentifier;
            var connectedUser = await _db.ConnectedUsers.FirstOrDefaultAsync(x => x.AspNetUserId == userId);
            connectedUser.IsDisconnected = true;

            _db.ConnectedUsers.Update(connectedUser);
            await _db.SaveChangesAsync();

            var allUsers = await _db.ConnectedUsers.Where(x => !x.IsDisconnected).Select(x => x.UserName).ToListAsync();

            await Clients.All.SendAsync("OnDisconnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendUserVote(string user, string message)
        {
            var userInfo = await _db.UserInfos.FirstOrDefaultAsync(x => x.UserName == user);
            var userDisplay = userInfo != null ? userInfo.ProfilePicture : user;

            await Clients.All.SendAsync("ReceiveUserVote", userDisplay, message);
        }


        public async Task SendPrivateMessage(string user, string message)
        {
            var sendToUser = _db.ConnectedUsers.FirstOrDefault(x => x.UserName == user);
            var timestampedMessage = $"{DateTime.Now} - {message}";
            await Clients.User(sendToUser.AspNetUserId).SendAsync("ReceivePrivateMessage", timestampedMessage);
        }

        public async Task SendGroupMessage(string group, string message)
        {
            var groupUsers = await _db.ConnectedUsers.Where(x => !x.IsDisconnected && x.GroupName == group).ToListAsync();

            foreach (var groupUser in groupUsers)
            {
                await Groups.AddToGroupAsync(groupUser.ConnectionId, group);
            }

            await Clients.Group(group).SendAsync("ReceiveGroupMessage", message);
        }

        public async Task AddToGroup(string groupName)
        {
            string userId = Context.UserIdentifier;
            var connectedUser = _db.ConnectedUsers.FirstOrDefault(x => x.AspNetUserId == userId);
            connectedUser.GroupName = groupName;

            _db.ConnectedUsers.Update(connectedUser);
            _db.SaveChanges();

            await Groups.AddToGroupAsync(connectedUser.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveGroupAdd", $"{connectedUser.UserName} has joined the group {groupName}.");
        }


        public async Task NextQuestion()
        {
            var allQuestions = await _db.Questions.Select(x => x).ToListAsync();
            var activeQuestion = allQuestions.FirstOrDefault(x => x.IsActive == true);
            var nextQuestion = _db.Questions.Where(x => x.QuestionId > activeQuestion.QuestionId).Any()
                                ? _db.Questions.Where(x => x.QuestionId > activeQuestion.QuestionId).OrderBy(x => x.QuestionId).FirstOrDefault()
                                : allQuestions.FirstOrDefault();

            activeQuestion.IsActive = false;
            nextQuestion.IsActive = true;

            await _db.SaveChangesAsync();

            var newAnswers = await _db.Answers.Where(x => x.QuestionId == nextQuestion.QuestionId).ToListAsync();
            var displayAnswers = newAnswers.Select((x, i) => new { answer = AlphabetDict[i] + x.Text, isCorrect = x.IsCorrect }).ToArray();
            await Clients.All.SendAsync("NextQuestionReceived", nextQuestion.Text, displayAnswers);
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
