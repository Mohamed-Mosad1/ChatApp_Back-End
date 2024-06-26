namespace ChatApp.Persistence.Helpers
{
    public static class EmailBody
    {
        public static string GetEmailStringBody(string email, string emailToken, string baseUrl)
        {
            return $@"<html>
<head>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 10px 0;
            text-align: center;
        }}
        .content {{
            padding: 20px;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            color: #888888;
        }}
        a.button {{
            display: inline-block;
            padding: 10px 20px;
            font-size: 16px;
            color: white;
            background-color: #4CAF50;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset</h1>
        </div>
        <div class='content'>
            <p>Hello,</p>
            <p>You have requested to reset your password. Please click the button below within 15 minutes to reset your password:</p>
            <p><a href='{baseUrl}/reset-password?email={email}&token={emailToken}' class='button'>Reset Password</a></p>
            <p>If you did not request a password reset, please ignore this email or contact support if you have questions.</p>
            <p>Thank you!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 Mohamed Mosaad. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";



        }
    }
}
