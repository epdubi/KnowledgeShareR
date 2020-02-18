using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChatDemoR.Models;
using ChatDemoR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatDemoR.Pages
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

        public Question JsonQuestion {get; set;}

        public List<Answer> JsonAnswers {get; set;}

        public void OnGet()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PresentationContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("PresentationContext"));

            using(var context = new PresentationContext(optionsBuilder.Options))
            {
                this.Questions = context.Questions.Select(x => x).ToList();
                this.Answers = context.Questions.Where(x => x.Id == 1).SelectMany(x => x.Answers).ToList();
            }
            
            using (StreamReader r = new StreamReader("data.json"))
            {
                string json = r.ReadToEnd();
                RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                this.JsonQuestion = root.Question;
                this.JsonAnswers = root.Question.Answers;
            }
        }
    }
}
