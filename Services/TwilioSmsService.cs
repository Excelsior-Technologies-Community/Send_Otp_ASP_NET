using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Send_Otp_ASP_NET.Services
{
    public class TwilioSmsService
    {
        private readonly IConfiguration _config;

        public TwilioSmsService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtp(string mobile, string otp)
        {
            TwilioClient.Init(
                _config["Twilio:AccountSid"],
                _config["Twilio:AuthToken"]
            );

            MessageResource.Create(
                body: $"Your OTP is {otp}. Valid for 5 minutes.",
                from: new Twilio.Types.PhoneNumber(_config["Twilio:FromNumber"]),
                to: new Twilio.Types.PhoneNumber(mobile)
            );
        }
    }
}
