using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevextremeCore.Models
{
    public class TokenRequest
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string username { get; set; }
        public DateTime issued { get; set; }
        public DateTime expires { get; set; }
        public string refresh_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }
}
