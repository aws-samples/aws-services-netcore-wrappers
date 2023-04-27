using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.SecurityToken;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Amazon.Ses.Wrapper
{
    public class SESEmailService : ISESEmailService
    {
        private readonly ILogger<SESEmailService> _logger;
        private readonly IAmazonSecurityTokenService? _securityTokenService;
        private readonly IAmazonSimpleEmailService _sesService;
        private readonly SESOptions _emailOptions;

        public SESEmailService(IAmazonSimpleEmailService sesService, IOptions<SESOptions> emailOptions, ILogger<SESEmailService> logger)
        {
            _logger = logger;
            _sesService = sesService;
            _emailOptions = emailOptions.Value;
        }
        public SESEmailService(IAmazonSecurityTokenService securityTokenService, IAmazonSimpleEmailService sesService, IOptions<SESOptions> emailOptions, ILogger<SESEmailService> logger)
        {
            _logger = logger;
            _securityTokenService = securityTokenService;
            _sesService = sesService;
            _emailOptions = emailOptions.Value;
        }

        /// <summary>
        ///   /// <summary>
        /// sends email using AWS SES Api
        /// </summary>
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtmlBody"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> SendEmailAsync(string subject, string body, bool isHtmlBody, List<string> to,
            string? sender = null, List<string> cc = null, List<string> bcc = null)
        {
            sender = string.IsNullOrWhiteSpace(sender) ? _emailOptions.Sender : sender;
            var emailMessage = BuildEmailHeaders(sender, to, cc, bcc, subject);
            var emailBody = BuildEmailBody(body, isHtmlBody);
            emailMessage.Body = emailBody.ToMessageBody();
            return await SendEmailAsync(emailMessage);
        }

        /// <summary>
        /// sends email using AWS SES Api with attachment support (file paths) 
        /// </summary>
        /// <param name="to">to address</param>
        /// <param name="cc">cc addresses</param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtmlBody"></param>
        /// <param name="fileAttachmentPath"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> SendEmailWithAttachmentAsync(string subject, string body, bool isHtmlBody, string fileAttachmentPath, List<string> to,
            string? sender = null, List<string> cc = null, List<string> bcc = null)
        {
            sender = string.IsNullOrWhiteSpace(sender) ? _emailOptions.Sender : sender;
            var emailMessage = BuildEmailHeaders(sender, to, cc, bcc, subject);
            var emailBody = BuildEmailBody(body, isHtmlBody);
            if (!string.IsNullOrEmpty(fileAttachmentPath))
            {
                emailBody.Attachments.Add(fileAttachmentPath);
            }
            emailMessage.Body = emailBody.ToMessageBody();
            return await SendEmailAsync(emailMessage);
        }

        /// <summary>
        /// sends email using AWS SES Api with attachment support (file streams)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtmlBody"></param>
        /// <param name="fileName"></param>
        /// <param name="fileAttachmentStream"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> SendEmailWithAttachmentAsync(string subject, string body, bool isHtmlBody, string fileName, Stream fileAttachmentStream, List<string> to,
            string? sender = null, List<string> cc = null, List<string> bcc = null)
        {
            sender = string.IsNullOrWhiteSpace(sender) ? _emailOptions.Sender : sender;
            var emailMessage = BuildEmailHeaders(sender, to, cc, bcc, subject);
            var emailBody = BuildEmailBody(body, isHtmlBody);
            if (fileAttachmentStream != null && !string.IsNullOrEmpty(fileName))
            {
                emailBody.Attachments.Add(fileName, fileAttachmentStream);
            }
            emailMessage.Body = emailBody.ToMessageBody();
            return await SendEmailAsync(emailMessage);
        }

        #region private-methods

        /// <summary>
        /// builds email message body 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="isHtmlBody"></param>
        /// <returns></returns>
        private static BodyBuilder BuildEmailBody(string body, bool isHtmlBody = true)
        {
            var bodyBuilder = new BodyBuilder();
            if (isHtmlBody)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            return bodyBuilder;
        }

        /// <summary>
        /// builds email message headers 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        private static MimeMessage BuildEmailHeaders(string from, IEnumerable<string> to, IReadOnlyCollection<string> cc, IReadOnlyCollection<string> bcc, string subject)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(string.Empty, from));
            foreach (var recipient in to)
            {
                message.To.Add(new MailboxAddress(string.Empty, recipient));
            }
            if (cc != null && cc.Any())
            {
                foreach (var recipient in cc)
                {
                    message.Cc.Add(new MailboxAddress(string.Empty, recipient));
                }
            }
            if (bcc != null && bcc.Any())
            {
                foreach (var recipient in bcc)
                {
                    message.Bcc.Add(new MailboxAddress(string.Empty, recipient));
                }
            }
            message.Subject = subject;
            return message;
        }

        /// <summary>
        /// sends email using AWS SES Api - using  SendRawEmail method.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<HttpStatusCode> SendEmailAsync(MimeMessage message)
        {
            HttpStatusCode responseCode = HttpStatusCode.OK;
            string responseMessageId = null;

            using (var memoryStream = new MemoryStream())
            {
                await message.WriteToAsync(memoryStream);
                memoryStream.Position = 0;

                var sendRequest = new SendRawEmailRequest { RawMessage = new RawMessage(memoryStream) };

                var response = await _sesService.SendRawEmailAsync(sendRequest);
                responseCode = response.HttpStatusCode;
                responseMessageId = response.MessageId;
            }

            if (responseCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"The email with message Id {responseMessageId} sent successfully to {message.To} on {DateTime.UtcNow:O}");
            }
            else
            {
                _logger.LogError($"Failed to send email with message Id {responseMessageId} to {message.To} on {DateTime.UtcNow:O} due to {responseCode}.");
            }

            return responseCode;
        }

        #endregion
    }
}
