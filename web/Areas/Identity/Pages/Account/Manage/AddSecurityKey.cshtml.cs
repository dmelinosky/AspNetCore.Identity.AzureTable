using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using web.Models;

namespace web.Areas.Identity.Pages.Account.Manage
{
    public class AddSecurityKeyModel : PageModel
    {
        public string Username { get; set; }

        public new string Challenge { get; set; }

        public string RelyingPartyId { get; set; }

        public void OnGet()
        {
            this.RelyingPartyId = "localhost";
            //this.Challenge = CryptoRandom.CreateUniqueId(16);
            this.Challenge = "qb7mDKQAydg562lpaR41nQ==";
            this.Username = this.User.Identity.Name;
        }
    }
}
