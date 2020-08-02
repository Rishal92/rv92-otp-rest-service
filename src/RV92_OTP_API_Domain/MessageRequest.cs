using System.Text.Json.Serialization;

namespace RV92.Otp.Api.Domain
{
    public class MessageRequest
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}