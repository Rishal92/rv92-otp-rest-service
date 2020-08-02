using System.Collections.Generic;

namespace RV92.Otp.Api.Domain
{
    public class SmsRequest
    {
        public List<MessageRequest> Messages { get; set; }
        public string Error { get; set; }
    }
}