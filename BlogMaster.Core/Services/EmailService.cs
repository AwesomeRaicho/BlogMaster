using BlogMaster.Core.Contracts;
using BlogMaster.Core.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;




        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        private string GenerateConfirmationBody(string userName, string callbackUrl)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Confirm Your Email</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                            color: #333;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background: #fff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #007bff;
                        }}
                        p {{
                            font-size: 16px;
                            line-height: 1.5;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            font-size: 16px;
                            color: white;
                            background-color: #007bff;
                            text-decoration: none;
                            border-radius: 4px;
                        }}
                        .button:hover {{
                            background-color: #0056b3;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Email Confirmation</h1>
                        <p>Hello {userName},</p>
                        <p>Thank you for registering with us. Please confirm your email by clicking the link below:</p>
                        <a href='{callbackUrl}' class='button' style='color: white;'>Confirm Email</a>
                        <p>If the button doesn't work, you can copy and paste the following URL into your browser:</p>
                        <p>{callbackUrl}</p>
                        <p>Thank you!</p>
                        <p>The Team</p>
                    </div>
                </body>
                </html>";

        }

        public string GeneratePasswordResetBody(string userName, string callbackUrl)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Reset Your Password</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                            color: #333;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background: #fff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #ff5722;
                        }}
                        p {{
                            font-size: 16px;
                            line-height: 1.5;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            font-size: 16px;
                            color: white;
                            background-color: #ff5722;
                            text-decoration: none;
                            border-radius: 4px;
                        }}
                        .button:hover {{
                            background-color: #e64a19;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Reset Your Password</h1>
                        <p>Hello {userName},</p>
                        <p>We received a request to reset your password. You can reset your password by clicking the button below:</p>
                        <a href='{callbackUrl}' class='button' style='color: white;'>Reset Password</a>
                        <p>If the button doesn't work, you can copy and paste the following URL into your browser:</p>
                        <p>{callbackUrl}</p>
                        <p>If you did not request to reset your password, please ignore this email or contact support if you have concerns.</p>
                        <p>Thank you!</p>
                        <p>The Team</p>
                    </div>
                </body>
                </html>";

        }



        public async Task TestSendEmail(string toEmail, string userName, string confirmationToken)
        {
            string body = GenerateConfirmationBody(userName, confirmationToken);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", _emailSettings.Username));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Email Confirmation";
            message.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// Sends registration confirmation email.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="userName"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public async Task SendEmailConfirmation(string toEmail, string userName, string callbackUrl)
        {
            string body = GenerateConfirmationBody(userName, callbackUrl);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BlogMaster", _emailSettings.Username));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Email Confirmation";
            message.Body = new TextPart("html") { Text = body };


            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// Send Password reset confirmation email.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="userName"></param>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        public async Task SendPasswordChangeConfirmation(string toEmail, string userName, string callbackUrl)
        {
            string body = GeneratePasswordResetBody(userName, callbackUrl);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BlogMaster", _emailSettings.Username));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Password Reset";
            message.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

    }


}
