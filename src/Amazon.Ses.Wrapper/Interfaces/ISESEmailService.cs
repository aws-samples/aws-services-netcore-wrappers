using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Amazon.Ses.Wrapper
{
      /// <summary>
    /// Interface to provide SendEmail Operations by using Amazon Simple Mail Service (SES)
    /// </summary>
    public interface ISESEmailService
    {
        /// <summary>
        /// Sends an email by using Amazon SES Service.
        /// </summary>
        /// <param name="subject">Contains subject of the email</param>
        /// <param name="body">Contains body of the email</param>
        /// <param name="isHtmlBody">Whether the email body is in HTML or not</param>
        /// <param name="to">List containing email ids to send the email to</param>
        /// <param name="sender">Contains email id of sender</param>
        /// <param name="cc">List containing ccemail ids to send the email copy to</param>
        /// <param name="bcc">List containing bcc email ids to send the bcc email copy to</param>
        /// <returns>Returns the HttpStatusCode that Amazon SES returned</returns>
        Task<HttpStatusCode> SendEmailAsync(string subject, string body, bool isHtmlBody, List<string> to, 
            string? sender = null, List<string> cc = null, List<string> bcc = null);
        
        /// <summary>
        /// Sends an email with attachment by using Amazon SES Service.
        /// </summary>
        /// <param name="subject">Contains subject of the email</param>
        /// <param name="body">Contains body of the email</param>
        /// <param name="isHtmlBody">Whether the email body is in HTML or not</param>
        /// <param name="fileAttachmentPath">path containing file to be send as an attachment in the email</param>
        /// <param name="to">List containing email ids to send the email to</param>
        /// <param name="sender">Contains email id of sender</param>
        /// <param name="cc">List containing ccemail ids to send the email copy to</param>
        /// <param name="bcc">List containing bcc email ids to send the bcc email copy to</param>
        /// <returns>Returns the HttpStatusCode that Amazon SES returned</returns>
        Task<HttpStatusCode> SendEmailWithAttachmentAsync(string subject, string body, bool isHtmlBody, string fileAttachmentPath, List<string> to, 
            string? sender = null, List<string> cc = null, List<string> bcc = null);
        
          
        /// <summary>
        /// Sends an email with attachment by using Amazon SES Service.
        /// </summary>
        /// <param name="subject">Contains subject of the email</param>
        /// <param name="body">Contains body of the email</param>
        /// <param name="isHtmlBody">Whether the email body is in HTML or not</param>
        /// <param name="fileName">filename to be used for attachment in the email</param>
        /// <param name="fileAttachmentStream">Stream to be send as an attachment in the email</param>
        /// <param name="to">List containing email ids to send the email to</param>
        /// <param name="sender">Contains email id of sender</param>
        /// <param name="cc">List containing ccemail ids to send the email copy to</param>
        /// <param name="bcc">List containing bcc email ids to send the bcc email copy to</param>
        /// <returns>Returns the HttpStatusCode that Amazon SES returned</returns>
        Task<HttpStatusCode> SendEmailWithAttachmentAsync(string subject, string body, bool isHtmlBody, string fileName, Stream fileAttachmentStream, List<string> to,
            string? sender = null, List<string> cc = null, List<string> bcc = null);
    }
}
