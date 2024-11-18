using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IEmailService
    {
        public Task TestSendEmail(string to, string subject, string body);
        public Task SendEmailConfirmation(string toEmail, string userName, string callbackUrl);

        public Task SendPasswordChangeConfirmation(string toEmail, string userName, string callbackUrl);


    }
}
