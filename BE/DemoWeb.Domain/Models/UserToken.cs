using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DemoWeb.Domain.Models
{
    public class UserToken
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } 
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}
