using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using KnowledgeShareR.Data;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace KnowledgeShareR.Pages
{
    public class MessageModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IConfiguration Configuration { get; }

        private readonly KnowledgeShareDbContext _db;

        public List<string> AllUsers { get; set; }

        public List<string> HubGroups => new List<string> { "Group1", "Group2" };

        public MessageModel(ILogger<IndexModel> logger, IConfiguration configuration, KnowledgeShareDbContext dbContext)
        {
            _logger = logger;
            Configuration = configuration;
            _db = dbContext;
        }
        public void OnGet()
        {
            var connectedUsers = _db.ConnectedUsers.Select(x => x.UserName).ToList();
            AllUsers = connectedUsers;
        }
    }
}