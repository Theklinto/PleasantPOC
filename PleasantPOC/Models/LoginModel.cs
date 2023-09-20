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
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("password")]
        public required string Password { get; set; }
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "password";
        public required string TwoFactorToken { get; set; }
    }
}
