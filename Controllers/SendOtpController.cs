using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Send_Otp_ASP_NET.Helpers;
using Send_Otp_ASP_NET.Repository;
using Send_Otp_ASP_NET.Services;

namespace Send_Otp_ASP_NET.Controllers
{
    public class SendOtpController : Controller
    {
        private readonly ILogger<SendOtpController> _logger;
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public SendOtpController(
            ILogger<SendOtpController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _userRepository = new UserRepository(configuration);
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SendOtp(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return Json(new { success = false, message = "Mobile required" });

            // Ensure +91 format
            if (!mobile.StartsWith("+"))
                mobile = "+91" + mobile;

            string otp = OtpHelper.GenerateOtp();
            _userRepository.SaveOtp(mobile, otp, DateTime.Now.AddMinutes(5));

            var sms = new TwilioSmsService(_configuration);
            sms.SendOtp(mobile, otp);

            return Json(new { success = true, message = "OTP sent successfully" });
        }



        [HttpPost]
        public IActionResult VerifyOtp(string mobile, string otp)
        {
            mobile = MobileHelper.Normalize(mobile);

            bool valid = _userRepository.VerifyOtp(mobile, otp);

            if (!valid)
                return Json(new { success = false, message = "Invalid or expired OTP" });

            return Json(new { success = true, message = "OTP verified" });
        }


    }
}
