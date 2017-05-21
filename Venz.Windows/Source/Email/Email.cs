using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

namespace Venz.Windows
{
    public static class Email
    {
        public async static Task ShowAsync(EmailRecipient to, String subject)
        {
            var email = new EmailMessage();
            email.Subject = subject;
            email.To.Add(to);
            await EmailManager.ShowComposeNewEmailAsync(email).AsTask().ConfigureAwait(false);
        }
    }
}
