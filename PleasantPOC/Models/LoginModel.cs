using PleasantPOC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PleasantPOC.Attributes;

namespace PleasantPOC.Models
{
    public class LoginModel
    {
        [FormData("username")]
        public required string Username { get; set; }
        [FormData("password")]
        public required string Password { get; set; }
        [FormData("grant_type")]
        public string GrantType { get; set; } = "password";
        public required string TwoFactorToken { get; set; }
    }
}
