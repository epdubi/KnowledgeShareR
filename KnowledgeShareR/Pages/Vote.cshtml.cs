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

        public string UserName { get; set; }

        public VoteModel(ILogger<VoteModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

         

        public void OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                this.UserName = User.Identity.Name;
            }   
        }

        /*public async Task<IActionResult>  OnPostAsync()
        {
            var userName = Request.Form["UserName"];
            if(!string.IsNullOrWhiteSpace(userName))
            {
                this.UserName = userName;

                var optionsBuilder = new DbContextOptionsBuilder<KnowledgeShareDbContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("KnowledgeShareDbContext"));
                
                var context = new KnowledgeShareDbContext(optionsBuilder.Options);
                await context.ConnectedUsers.AddAsync(new Models.ConnectedUser { UserName = userName });
                await context.SaveChangesAsync();
            }

            return Page();
        }*/
    }
}
