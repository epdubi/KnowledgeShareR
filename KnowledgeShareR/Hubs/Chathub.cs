﻿using KnowledgeShareR.Data;
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

        private readonly KnowledgeShareDbContext _db;

        private readonly Dictionary<int, string> AlphabetDict = new Dictionary<int, string> { { 0, "a. " }, { 1, "b. " }, { 2, "c. " }, { 3, "d. " }, { 4, "e. " }, { 5, "f. " } };

        public ChatHub(IConfiguration configuration, KnowledgeShareDbContext dbContext)
        {
            Configuration = configuration;
            _db = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            string username = Context.User.Identity.Name;

            var isConnected = _db.ConnectedUsers.Any(x => x.UserName == username);

            if (!isConnected)
            {
                await _db.ConnectedUsers.AddAsync(new Models.ConnectedUser { ConnectionId = Context.ConnectionId, UserName = username });
                await _db.SaveChangesAsync();
            }
            else
            {
                var currentUser = await _db.ConnectedUsers.SingleAsync(x => x.UserName == username);
                currentUser.ConnectionId = Context.ConnectionId;
                _db.ConnectedUsers.Update(currentUser);
                await _db.SaveChangesAsync();
            }

            var allUsers = await _db.ConnectedUsers.Select(x => x.UserName).ToListAsync();

            await Clients.All.SendAsync("OnConnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string username = Context.User.Identity.Name;

            var connectedUser = await _db.ConnectedUsers.Where(x => x.UserName == username).ToListAsync();
            _db.ConnectedUsers.RemoveRange(connectedUser);
            await _db.SaveChangesAsync();

            var allUsers = await _db.ConnectedUsers.Select(x => x.UserName).ToListAsync();

            await Clients.All.SendAsync("OnDisconnectedAsync", JsonConvert.SerializeObject(allUsers));
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendUserVote(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveUserVote", user, message);
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
