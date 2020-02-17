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

namespace ChatDemoR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public Question Question {get; set;}

        public List<Answer> Answers {get; set;}

        public void OnGet()
        {
            using (StreamReader r = new StreamReader("data.json"))
            {
                string json = r.ReadToEnd();
                RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
                this.Question = root.Question;
                this.Answers = root.Question.Answers;
            }
        }
    }
}
