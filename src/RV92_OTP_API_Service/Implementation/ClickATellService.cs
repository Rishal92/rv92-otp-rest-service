using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RV92.Otp.Api.Domain;
using RV92.Otp.Api.Service.Interface;

namespace RV92.Otp.Api.Service.Implementation
{
    //ClickATell is a service provider that lets you send SMSs, WhatsApp messages and enables you to run campaigns
    //They allow you do create an sms service which allows you to whitelist 3 mobile numbers in a sandbox environment
    //https://portal.clickatell.com/

    public class ClickATellService : IClickATellService
    {
        public SmsResult SendSms(string recipientMobileNumber, string message)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

            var client = new RestClient("https://platform.clickatell.com/v1/message") {Timeout = 30000};


            var request = new RestRequest(Method.POST);
            
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "############################"); //ToDo: Register on ClickATell and get your key - https://portal.clickatell.com/

            var payload = new SmsRequest {Messages = new List<MessageRequest>()};

            var messageRequest = new MessageRequest() { Channel = "sms", To = recipientMobileNumber, Content = message };
            payload.Messages.Add(messageRequest);

            var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented, settings);

            request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);
            
            var response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<SmsResult>(response.Content);

            return result;

        }
    }
}
