using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KnowledgeShareR.Pages
{
    public class GroupsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IConfiguration Configuration { get; }

        public List<string> HubGroups => new List<string> { "Group1", "Group2" };

        public GroupsModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }
        public void OnGet()
        {
        }
    }
}