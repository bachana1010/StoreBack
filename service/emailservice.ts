// using System.Threading.Tasks;
// using MailKit.Net.Smtp;
// using MimeKit;
// using Microsoft.Extensions.Configuration;

// namespace YourNamespace.Services
// {
//     public class EmailService : IEmailService
//     {
//         private readonly IConfiguration _configuration;

//         public EmailService(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }

//         public async Task SendEmailAsync(string to, string subject, string body)
//         {
//             var email = new MimeMessage();
//             email.From.Add(MailboxAddress.Parse(_configuration["SmtpSettings:Email"]));
//             email.To.Add(MailboxAddress.Parse(to));
//             email.Subject = subject;
//             email.Body = new TextPart(MimeKit.TextFormat.Plain) { Text = body };

//             using var smtp = new SmtpClient();
//             smtp.Connect(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), false);
//             smtp.Authenticate(_configuration["SmtpSettings:Email"], _configuration["SmtpSettings:Password"]);
//             await smtp.SendAsync(email);
//             smtp.Disconnect(true);
//         }
//     }
// }
