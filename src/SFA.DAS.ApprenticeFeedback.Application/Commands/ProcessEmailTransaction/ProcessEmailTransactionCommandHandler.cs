﻿using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction
{
    public class ProcessEmailTransactionCommandHandler : IRequestHandler<ProcessEmailTransactionCommand, ProcessEmailTransactionResponse>
    {
        private readonly IFeedbackTransactionContext _context;
        private readonly IEngagementEmailContext _engagementEmailContext;
        private readonly IExclusionContext _exclusionContext;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<ProcessEmailTransactionCommandHandler> _logger;
        private readonly IMessageSession _messageSession;

        public ProcessEmailTransactionCommandHandler(
            IFeedbackTransactionContext context,
            IEngagementEmailContext engagementEmailContext,
            IExclusionContext exclusionContext,
            ApplicationSettings appSettings,
            IDateTimeHelper dateTimeHelper,
            IEmailTemplateService emailTemplateService,
            ILogger<ProcessEmailTransactionCommandHandler> logger,
            IMessageSession messageSession)
        {
            _context = context;
            _engagementEmailContext = engagementEmailContext;
            _exclusionContext = exclusionContext;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _messageSession = messageSession;
        }

        public async Task<ProcessEmailTransactionResponse> Handle(ProcessEmailTransactionCommand request, CancellationToken cancellationToken)
        {
            var feedbackTransaction = await _context.FindByIdIncludeFeedbackTargetAsync(request.FeedbackTransactionId);
            if (null == feedbackTransaction) return null;

            if (feedbackTransaction.SentDate.HasValue)
            {
                // if the email has already been sent, do nothing and return a successful state
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            var emailTemplateInfo = await _emailTemplateService.GetEmailTemplateInfoForTransaction(feedbackTransaction, request);
            if (emailTemplateInfo.Id == null && feedbackTransaction.TemplateName == null)
            {
                // when no email template can be determined remove the email but return success, this is
                // only for email transactions which have no per-determined template name such as the
                // feedback or withdrawn (exit survey) as these will be periodically re-created
                _context.Entities.Remove(feedbackTransaction);
                await _context.SaveChangesAsync();
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
            }

            EmailSentStatus sendStatus = EmailSentStatus.Failed;

            var isIgnoredProvider = await _exclusionContext.HasExclusion(feedbackTransaction.ApprenticeFeedbackTarget.Ukprn.GetValueOrDefault(0));
            var isEngagementEmail = await _engagementEmailContext.HasTemplate(emailTemplateInfo.Name);

            if (emailTemplateInfo.Name == "Active" && !request.IsFeedbackEmailContactAllowed)
            {
                // do not send the feedback email when the user has set their unsubscribed preference
                sendStatus = EmailSentStatus.NotAllowed;

                if (feedbackTransaction.ApprenticeFeedbackTarget.Status == (int)FeedbackTargetStatus.Complete)
                {
                    // when the apprenticeship is complete remove the email but return success
                    _context.Entities.Remove(feedbackTransaction);
                    await _context.SaveChangesAsync();
                    return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);
                }
                else
                {
                    // delay the sending of the feedback email, as the user may re-subscribe later on
                    feedbackTransaction.SendAfter = _dateTimeHelper.Now.AddDays(_appSettings.FeedbackEmailProcessingRetryWaitDays);
                }
            }
            else if (emailTemplateInfo.Name == "Withdrawn" && isIgnoredProvider)
            {
                // when the provider is excluded remove the exit survey email but return success
                _context.Entities.Remove(feedbackTransaction);
                await _context.SaveChangesAsync();
                return new ProcessEmailTransactionResponse(feedbackTransaction.Id, EmailSentStatus.Successful);

            }
            else if (isEngagementEmail && !request.IsEngagementEmailContactAllowed)
            {
                // when the user has unsubscribed from the engagement email, do not send it but 
                // but record the date it was not sent on, we would not want to send an email
                // later on if the user re-subscribes
                sendStatus = EmailSentStatus.NotAllowed;
                feedbackTransaction.IsSuppressed = true;
                feedbackTransaction.SentDate = _dateTimeHelper.Now;
            }
            else if (isEngagementEmail && feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipStatus == ApprenticeshipStatus.Paused)
            {
                // delay the sending of the engagement email, as the apprenticeship may resume after the user has re-subscribed later on
                sendStatus = EmailSentStatus.NotAllowed;
                feedbackTransaction.SendAfter = _dateTimeHelper.Now.AddDays(_appSettings.FeedbackEmailProcessingRetryWaitDays);
            }
            else if (emailTemplateInfo.Id.HasValue)
            {
                feedbackTransaction.TemplateId = emailTemplateInfo.Id;
                feedbackTransaction.Variant = emailTemplateInfo.Variant;
                feedbackTransaction.EmailAddress = request.ApprenticeEmailAddress;
                feedbackTransaction.FirstName = request.ApprenticeName;
                feedbackTransaction.SentDate = _dateTimeHelper.Now;
                sendStatus = EmailSentStatus.Successful;
            }
            
            await _context.SaveChangesAsync();

            if (sendStatus == EmailSentStatus.Successful)
            {
                await SendEmailViaNserviceBus(
                    request.ApprenticeEmailAddress, 
                    emailTemplateInfo.Id.ToString(),
                    emailTemplateInfo.Name,
                    emailTemplateInfo.Tokens);
            }

            return new ProcessEmailTransactionResponse(feedbackTransaction.Id, sendStatus);
        }

        private async Task SendEmailViaNserviceBus(string toAddress, string templateId, string templateName, Dictionary<string, string> personalisationTokens)
        {
            try
            {
                var emailCommand = new SendEmailCommand(templateId, toAddress, personalisationTokens);
                _logger.LogInformation("Sending {TemplateName} email ({TemplateId}) to {ToAddress}", templateName, templateId, toAddress);
                await _messageSession.Send(emailCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {TemplateName} email ({TemplateId}) to {ToAddress}", templateName, templateId, toAddress);
            }
        }
    }
}
