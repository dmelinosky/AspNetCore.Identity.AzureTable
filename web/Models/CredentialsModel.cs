using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class CredentialsModel
    {
        public string Id { get; set; }
        public string RawId { get; set; }
        public string Type { get; set; }
        public FidoResponse Response { get; set; }
    }
}
