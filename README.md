📱 OTP Authentication in ASP.NET Core MVC using Twilio
This project implements mobile number based OTP authentication in ASP.NET Core MVC using Twilio for SMS delivery.
It allows users to login or register using only a mobile number, without passwords.

🔹 How OTP Sending Works (High Level)
User enters mobile number
Application generates a 6-digit OTP
OTP is saved in database with expiry time
OTP is sent to user via Twilio SMS
User enters OTP
OTP is verified from database
User is authenticated

🔧 Technologies Used
ASP.NET Core MVC
Twilio SMS API
ADO.NET (SQL Server)
Razor Views + jQuery AJAX

🔐 Twilio Prerequisites
To send SMS using Twilio, the following are required:
Twilio Account
Account SID
Auth Token
Twilio SMS-enabled phone number (FromNumber)

⚙️ Configuration (appsettings.json)

"Twilio": {

  "AccountSid": "ACxxxxxxxxxxxxxxxxxxxx",
  
  "AuthToken": "your_auth_token",
  
  "FromNumber": "+15xxxxxxxxxxx"
}

Generates a secure 6-digit OTP
OTP is valid for a limited time (e.g., 5 minutes)
📤 Sending OTP using Twilio
TwilioClient.Init(accountSid, authToken);
MessageResource.Create(
    body: $"Your OTP is {otp}. Valid for 5 minutes.",
    from: new PhoneNumber(fromNumber),
    to: new PhoneNumber(mobile)
);

Twilio SDK sends SMS to the user
No DLT registration required (unlike Indian SMS gateways)
Works globally
🗄️ OTP Storage (Database)
OTP is stored temporarily in database:

🔒 Security Considerations
OTP expires after a fixed time
OTP is single-use
No password stored
SMS sent via trusted provider (Twilio)
Can be enhanced with:
OTP hashing
Retry limits
Session or JWT authentication

🌍 Why Twilio?
No DLT registration required
Works internationally
Reliable delivery
Easy API integration
Ideal for OTP authentication systems

🚀 Use Cases
Mobile login without password
Registration via OTP
Two-factor authentication (2FA)
Secure user verification

<img width="1159" height="683" alt="Screenshot 2025-12-26 184658" src="https://github.com/user-attachments/assets/4046a69b-b6c7-4cdb-9ed7-6649f0b95744" />
