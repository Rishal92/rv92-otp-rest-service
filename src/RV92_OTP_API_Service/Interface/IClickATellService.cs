using System.Net.Mail;
using RV92.Otp.Api.Domain;

namespace RV92.Otp.Api.Service.Interface
{
    public interface IClickATellService
    {
        public SmsResult SendSms(string recipientMobileNumber, string message);
    }
}