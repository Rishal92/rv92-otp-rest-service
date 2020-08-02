using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RV92.Otp.Api.Domain;
using RV92.Otp.Api.Service.Interface;
using Swashbuckle.Swagger.Annotations;

namespace RV92.Otp.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly ILogger<OtpController> _logger;
        private readonly IClickATellService _clickATellService;

        public List<OtpItem> OtpList = new List<OtpItem>();

        public const string SessionKeyName = "_OtpSession";

        public OtpController(ILogger<OtpController> logger, IClickATellService iClickATellService)
        {
            _logger = logger;
            _clickATellService = iClickATellService;
            OtpList = new List<OtpItem>();
        }

        //Configure Swagger Middleware: this makes up your documentation you see on the Swagger UI
        /// <summary>
        /// Sends an OTP sms to the provided cell number and provides you with a message identifier.
        /// </summary>
        /// <remarks>
        /// Sample request:<br/><br/>
        /// recipientMobileNumber = +27720720722
        /// 
        /// </remarks>
        /// <param name="recipientMobileNumber"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost("~/SendOtp")]
        [SwaggerOperation("SendOtp")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "OTP sent successfully.")]
        public IActionResult SendOtp(string recipientMobileNumber)
        {
            var regex = @"^\+(?:[0-9] ?){6,14}[0-9]$";

            var matchValidMobileNumber = Regex.Match(recipientMobileNumber, regex, RegexOptions.IgnoreCase);

            if (!matchValidMobileNumber.Success)
            {
                return StatusCode(400, "Please specify a valid phone number with the international dialing code.\nExample: +27720720722");
            }
           

            var otp = GenerateAlphaNumericOtp();

            var otpMessage = $"Welcome to Team RV92 \nYour OTP is: {otp}";

            var result = _clickATellService.SendSms(recipientMobileNumber, otpMessage);
            var messageId = result.Messages.FirstOrDefault()?.ApiMessageId;

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                OtpList = JsonConvert.DeserializeObject<List<OtpItem>>(HttpContext.Session.GetString(SessionKeyName));
            }

            OtpList.Add(new OtpItem()
            {
                Otp = otp,
                MessageId = messageId
            });

            HttpContext.Session.SetString(SessionKeyName, JsonConvert.SerializeObject(OtpList));

            return StatusCode(200, messageId);
        }

        //Configure Swagger Middleware: this makes up your documentation you see on the Swagger UI
        /// <summary>
        /// Validates an OTP request against the message identifier.
        /// </summary>
        /// <remarks>
        /// Sample request:<br/><br/>
        /// messageId = 33a48194a50644fea15589d7068aaa00<br/>
        /// otp = d1PVj4
        /// 
        /// </remarks>
        /// <param name="messageId"></param>
        /// <param name="otp"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost("~/ValidateOtp")]
        [SwaggerOperation("ValidateOtp")]
        [SwaggerResponse(statusCode: 200, type: typeof(IsValidResult), description: "Validate OTP.")]
        public IsValidResult ValidateOtp(string messageId, string otp)
        {
            var result = new IsValidResult();
            
            OtpList = JsonConvert.DeserializeObject<List<OtpItem>>(HttpContext.Session.GetString(SessionKeyName));

            if (OtpList.Any(item => item.MessageId == messageId && item.Otp == otp))
            {
                result.IsValid = true;
                return result;
            }

            result.IsValid = false;
            result.Message = "OTP does not match.";
            return result;
        }

        private string GenerateAlphaNumericOtp()
        {
            const string chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var otp = new char[6];
            var random1 = new Random();

            for (var i = 0; i < otp.Length; i++)
            {
                otp[i] = chars1[random1.Next(chars1.Length)];
            }

            return new string(otp);
        }
    }
}