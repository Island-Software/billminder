using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Paybills.API.Infrastructure.Services
{
    public class SESService
    {
        
        private IAmazonSimpleEmailService _amazonSimpleEmailService;

        public SESService(IAmazonSimpleEmailService amazonSimpleEmailService)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
        }

        /// <summary>
        /// Starts verification of an email identity. This request sends an email
        /// from Amazon SES to the specified email address. To complete
        /// verification, follow the instructions in the email.
        /// </summary>
        /// <param name="recipientEmailAddress">Email address to verify.</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> VerifyEmailIdentityAsync(string recipientEmailAddress)
        {
            var success = false;
            try
            {
                var response = await _amazonSimpleEmailService.VerifyEmailIdentityAsync(
                    new VerifyEmailIdentityRequest
                    {
                        EmailAddress = recipientEmailAddress
                    });

                success = response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine("VerifyEmailIdentityAsync failed with exception: " + ex.Message);
            }

            return success;
        }

        /// <summary>
        ///  Send an email by using Amazon SES.
        /// </summary>
        /// <param name="toAddresses">List of recipients.</param>
        /// <param name="ccAddresses">List of cc recipients.</param>
        /// <param name="bccAddresses">List of bcc recipients.</param>
        /// <param name="bodyHtml">Body of the email in HTML.</param>
        /// <param name="bodyText">Body of the email in plain text.</param>
        /// <param name="subject">Subject line of the email.</param>
        /// <param name="senderAddress">From address.</param>
        /// <returns>The messageId of the email.</returns>
        public async Task<string> SendEmailAsync(List<string> toAddresses,
            List<string> ccAddresses, List<string> bccAddresses,
            string bodyHtml, string bodyText, string subject, string senderAddress)
        {
            var messageId = "";
                var response = await _amazonSimpleEmailService.SendEmailAsync(
                    new SendEmailRequest
                    {
                        Destination = new Destination
                        {
                            BccAddresses = bccAddresses,
                            CcAddresses = ccAddresses,
                            ToAddresses = toAddresses
                        },
                        Message = new Message
                        {
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyHtml
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyText
                                }
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = subject
                            }
                        },
                        Source = senderAddress
                    });
                Console.WriteLine(response);
                messageId = response.MessageId;

            return messageId;
        }
    }
}