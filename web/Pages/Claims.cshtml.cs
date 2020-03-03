using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace web
{
    public class ClaimsModel : PageModel
    {
        public ClaimsModel(ILogger<ClaimsModel> logger)
        {
            this.Logger = logger;
        }

        public ILogger<ClaimsModel> Logger { get; }

        public void OnGet()
        {
            this.Logger.LogInformation("Claims page GET handler.");
        }
    }
}