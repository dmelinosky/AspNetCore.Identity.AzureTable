using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class FidoResponse
    {
        public string AttestationObject { get; set; }
        public string ClientDataJson { get; set; }
        public string AuthenticatorData { get; set; }
        public string Signature { get; set; }
        public string UserHandle { get; set; }
    }
}
