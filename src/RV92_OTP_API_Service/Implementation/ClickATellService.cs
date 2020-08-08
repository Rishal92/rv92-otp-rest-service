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

    /// <summary>
    /// Recommended way to use RestSharp is to create a new instance per request.
    /// It differs from Singleton approach recommended for HttpClient.
    /// The reason is that under the hood RestSharp uses HttpWebRequest for HTTP interaction, not HttpClient.That's why the usage model differs.
    ///
    /// RestSharp does not use connection pool as HttpClient and does not leave opened sockets after the use.
    /// That's why it is safe (and recommended) to create a new instance of RestClient per request.
    /// </summary>

    public class ClickATellService : IClickATellService
    {
        private readonly RestClient _restClient;

        public ClickATellService()
        {
            _restClient = new RestClient("https://platform.clickatell.com/v1/message");
        }

        public SmsResult SendSms(string recipientMobileNumber, string message)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            
            var request = new RestRequest(Method.POST);
            
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", "WJKSKNPJQjCK7pnfXTO_ow=="); //ToDo: Register on ClickATell and get your key - https://portal.clickatell.com/ 
            //WJKSKNPJQjCK7pnfXTO_ow==	############################

            var payload = new SmsRequest {Messages = new List<MessageRequest>()};

            var messageRequest = new MessageRequest() { Channel = "sms", To = recipientMobileNumber, Content = message };
            payload.Messages.Add(messageRequest);

            var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented, settings);

            request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);
            
            var response = _restClient.Execute(request);

            var result = JsonConvert.DeserializeObject<SmsResult>(response.Content);

            return result;

        }
    }
}
