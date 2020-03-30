using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using KnowledgeShareR.Models;
using KnowledgeShareR.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeShareR.Pages
{
    public class VoteModel : PageModel
    {
        private readonly ILogger<VoteModel> _logger;

        public IConfiguration Configuration { get; }

        private readonly KnowledgeShareDbContext _db;

        public string UserName { get; set; }

        public Question Question { get; set; }

        public List<Answer> Answers { get; set; }

        public VoteModel(ILogger<VoteModel> logger, IConfiguration configuration, KnowledgeShareDbContext dbContext)
        {
            _logger = logger;
            Configuration = configuration;
            _db = dbContext;
        }

        public void OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var activeQuestion = _db.Questions.Include(x => x.Answers).First(x => x.IsActive);

                UserName = userName;
                Question = activeQuestion;
                Answers = activeQuestion.Answers.ToList();
            }   
        }
    }
}
