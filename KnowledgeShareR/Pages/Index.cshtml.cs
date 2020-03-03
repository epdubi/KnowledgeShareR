using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KnowledgeShareR.Models;
using KnowledgeShareR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KnowledgeShareR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IConfiguration Configuration { get; }
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public List<Question> Questions {get; set;}
        public List<Answer> Answers {get; set;}
        public List<ConnectedUser> ConnectedUsers {get; set;}

        public void OnGet()
        {
            var optionsBuilder = new DbContextOptionsBuilder<KnowledgeShareDbContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext"));

            using(var context = new KnowledgeShareDbContext(optionsBuilder.Options))
            {
                this.Questions = context.Questions.Select(x => x).ToList();
                this.Answers = context.Questions.Where(x => x.QuestionId == 1).SelectMany(x => x.Answers).ToList();
                this.ConnectedUsers = context.ConnectedUsers.Select(x => x).OrderByDescending(x => x.ConnectedUserId).ToList();
            }
        }
    }
}
