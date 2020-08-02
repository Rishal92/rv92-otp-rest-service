namespace RV92.Otp.Api.Domain
{
    public class MessageResult
    {
        public string ApiMessageId { get; set; }
        public bool Accepted { get; set; }
        public string To { get; set; }
    }
}