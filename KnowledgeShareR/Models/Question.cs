using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeShareR.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}