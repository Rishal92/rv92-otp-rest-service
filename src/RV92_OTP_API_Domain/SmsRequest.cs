using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RV92.Otp.Api.Domain
{
    public class SmsResult
    {
        [JsonPropertyName("messages")]
        public List<MessageResult> Messages { get; set; }
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}