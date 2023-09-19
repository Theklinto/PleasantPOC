using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PleasantPOC.Models
{
    public class LoginResponseModel
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = string.Empty;
        [JsonPropertyName("expiry")]
        public DateTime ExpirationDate { get; set; }
    }
}
