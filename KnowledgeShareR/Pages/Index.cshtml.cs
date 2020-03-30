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

        private readonly KnowledgeShareDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, KnowledgeShareDbContext dbContext)
        {
            _logger = logger;
            Configuration = configuration;
            _db = dbContext;
        }

        public List<Question> Questions {get; set;}
        public List<Answer> Answers {get; set;}
        public void OnGet()
        {
            this.Questions = _db.Questions.Select(x => x).ToList();
            this.Answers = _db.Questions.Where(x => x.IsActive).SelectMany(x => x.Answers).ToList();
        }
    }
}
