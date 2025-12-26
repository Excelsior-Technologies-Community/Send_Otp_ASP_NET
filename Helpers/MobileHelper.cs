using System.Text.RegularExpressions;

namespace Send_Otp_ASP_NET.Helpers
{
    public static class MobileHelper
    {
        public static string Normalize(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return mobile;

            // remove spaces, dashes, brackets
            mobile = Regex.Replace(mobile, @"\D", "");

            // if already has country code
            if (mobile.StartsWith("91") && mobile.Length == 12)
                return "+" + mobile;

            // if normal 10 digit Indian number
            if (mobile.Length == 10)
                return "+91" + mobile;

            // fallback
            return "+" + mobile;
        }
    }
}
