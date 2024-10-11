using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Services
{
    public class WhenGettingEmailTemplateInfoForTransaction
    {
        [Test]
        [AutoMoqData]
        public async Task AndTemplateIsUnknown_AndTargetIsActiveAndEligible_ThenReturnActiveTemplate(
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn = false;
            feedbackTransaction.ApprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            feedbackTransaction.ApprenticeFeedbackTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
            feedbackTransaction.TemplateName = null;

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be("Active");
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == "Active").TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_Complete)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_Complete)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_Complete)]
        public async Task AndTemplateIsUnknown_AndTargetIsNotActiveAndEligible_ThenReturnNoTemplate(
                int feedbackTargetStatus,
                int feedbackEligibilityStatus,
                [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn = false;
            feedbackTransaction.ApprenticeFeedbackTarget.Status = feedbackTargetStatus;
            feedbackTransaction.ApprenticeFeedbackTarget.FeedbackEligibility = feedbackEligibilityStatus;
            feedbackTransaction.TemplateName = null;

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().BeNull();
                result.Id.Should().BeNull();
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [Test]
        [AutoMoqData]
        public async Task AndTemplateIsUnknown_AndTargetIsWithdrawn_AndNoWithdrawnEmailSent_ThenReturnWithdrawnTemplate(
                [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn = true;
            feedbackTransaction.TemplateName = null;
                
            mockFeedbackTransactionContext.Setup(x => x.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId))
                .ReturnsAsync(new List<FeedbackTransaction>());

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be("Withdrawn");
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == "Withdrawn").TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [Test]
        [AutoMoqData]
        public async Task AndTemplateIsUnknown_AndTargetIsWithdrawn_AndTargetIsActiveAndEligible_AndWithdrawnEmailAlreadySent_ThenReturnNoTemplate(
                [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn = true;
            feedbackTransaction.ApprenticeFeedbackTarget.Status = (int)FeedbackTargetStatus.Active;
            feedbackTransaction.ApprenticeFeedbackTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
            feedbackTransaction.TemplateName = null;

            mockFeedbackTransactionContext.Setup(x => x.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId))
                .ReturnsAsync(new List<FeedbackTransaction>
                {
                    feedbackTransaction,
                    new FeedbackTransaction 
                    {
                        ApprenticeFeedbackTargetId = feedbackTransaction.ApprenticeFeedbackTargetId,
                        TemplateId = appSettings.NotificationTemplates.Single(p => p.TemplateName == "Withdrawn").TemplateId,
                        SentDate = DateTime.UtcNow.AddMonths(-1),
                    }
                });

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be("Active");
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == "Active").TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Deny_Complete)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_Complete)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Unknown)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooSoon)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [AutoMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_Complete)]
        public async Task AndTemplateIsUnknown_AndTargetIsWithdrawn_AndTargetIsNotActiveAndEligible_AndWithdrawnEmailAlreadySent_ThenReturnNoTemplate(
                int feedbackTargetStatus,
                int feedbackEligibilityStatus,
                [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.ApprenticeFeedbackTarget.Withdrawn = true;
            feedbackTransaction.ApprenticeFeedbackTarget.Status = feedbackTargetStatus;
            feedbackTransaction.ApprenticeFeedbackTarget.FeedbackEligibility = feedbackEligibilityStatus;
            feedbackTransaction.TemplateName = null;

            mockFeedbackTransactionContext.Setup(x => x.FindByApprenticeFeedbackTargetId(feedbackTransaction.ApprenticeFeedbackTargetId))
                .ReturnsAsync(new List<FeedbackTransaction>
                {
                    feedbackTransaction,
                    new FeedbackTransaction
                    {
                        ApprenticeFeedbackTargetId = feedbackTransaction.ApprenticeFeedbackTargetId,
                        TemplateId = appSettings.NotificationTemplates.Single(p => p.TemplateName == "Withdrawn").TemplateId,
                        SentDate = DateTime.UtcNow.AddMonths(-1),
                    }
                });

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().BeNull();
                result.Id.Should().BeNull();
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [AutoMoqInlineAutoData("AppStart")]
        [AutoMoqInlineAutoData("AppWelcome")]
        [AutoMoqInlineAutoData("AppMonthThree")]
        [AutoMoqInlineAutoData("AppMonthSix")]
        [AutoMoqInlineAutoData("AppMonthNine")]
        [AutoMoqInlineAutoData("AppMonthTwelve")]
        [AutoMoqInlineAutoData("AppMonthEighteen")]
        [AutoMoqInlineAutoData("AppAnnual")]
        [AutoMoqInlineAutoData("AppPreEpa")]
        public async Task AndTemplateIsKnown_ThenReturnTemplate(
                string templateName,
                [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
                [Frozen] ApplicationSettings appSettings,
                [Frozen] ApplicationUrls appUrls,
                FeedbackTransaction feedbackTransaction,
                ProcessEmailTransactionCommand command,
                EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.TemplateName = templateName;

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be(templateName);
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == templateName).TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [Test]
        [AutoMoqData]
        public async Task AndVariantIsAvailable_ThenReturnVariantTemplate(
        [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
        [Frozen] Mock<IFeedbackTargetVariantContext> mockFeedbackTargetVariantContext,
        [Frozen] ApplicationSettings appSettings,
        [Frozen] ApplicationUrls appUrls,
        FeedbackTransaction feedbackTransaction,
        ProcessEmailTransactionCommand command,
        EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.TemplateName = "AppStart";
            var variant = "A";
            var variantTemplateName = $"{feedbackTransaction.TemplateName}_{variant}";

            mockFeedbackTargetVariantContext
                .Setup(x => x.FindByApprenticeshipId(feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipId))
                .ReturnsAsync(new FeedbackTargetVariant { ApprenticeshipId = feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipId, Variant = variant });

            appSettings.NotificationTemplates.Add(new NotificationTemplate { TemplateName = variantTemplateName, TemplateId = Guid.NewGuid() });

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be(variantTemplateName);
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == variantTemplateName).TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [Test]
        [AutoMoqData]
        public async Task AndVariantIsAvailable_ButNoVariantTemplate_ThenReturnBaseTemplate(
        [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
        [Frozen] Mock<IFeedbackTargetVariantContext> mockFeedbackTargetVariantContext,
        [Frozen] ApplicationSettings appSettings,
        [Frozen] ApplicationUrls appUrls,
        FeedbackTransaction feedbackTransaction,
        ProcessEmailTransactionCommand command,
        EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.TemplateName = "AppStart";
            var variant = "A";

            mockFeedbackTargetVariantContext
                .Setup(x => x.FindByApprenticeshipId(feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipId))
                .ReturnsAsync(new FeedbackTargetVariant { Variant = variant });

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be(feedbackTransaction.TemplateName);
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == feedbackTransaction.TemplateName).TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        [Test]
        [AutoMoqData]
        public async Task AndNoVariantAvailable_ThenReturnBaseTemplate(
        [Frozen] Mock<IFeedbackTransactionContext> mockFeedbackTransactionContext,
        [Frozen] Mock<IFeedbackTargetVariantContext> mockFeedbackTargetVariantContext,
        [Frozen] ApplicationSettings appSettings,
        [Frozen] ApplicationUrls appUrls,
        FeedbackTransaction feedbackTransaction,
        ProcessEmailTransactionCommand command,
        EmailTemplateService sut)
        {
            // Arrange
            feedbackTransaction.TemplateName = "AppStart";
            mockFeedbackTargetVariantContext.Setup(x => x.FindByApprenticeshipId(feedbackTransaction.ApprenticeFeedbackTarget.ApprenticeshipId))
                .ReturnsAsync((FeedbackTargetVariant)null);

            // Act
            var result = await sut.GetEmailTemplateInfoForTransaction(feedbackTransaction, command);

            // Assert
            using (new AssertionScope())
            {
                result.Name.Should().Be(feedbackTransaction.TemplateName);
                result.Id.Should().Be(appSettings.NotificationTemplates.Single(p => p.TemplateName == feedbackTransaction.TemplateName).TemplateId);
                AssertTokens(result.Tokens, command, feedbackTransaction, appUrls);
            }
        }

        private static void AssertTokens(Dictionary<string, string> tokens, ProcessEmailTransactionCommand command, FeedbackTransaction feedbackTransaction, ApplicationUrls appUrls)
        {
            tokens.Should().ContainKey("Contact").WhoseValue.Equals(command.ApprenticeName);
            tokens.Should().ContainKey("ApprenticeFeedbackTargetId").WhoseValue.Equals(feedbackTransaction.ApprenticeFeedbackTargetId.ToString());
            tokens.Should().ContainKey("FeedbackTransactionId").WhoseValue.Equals(feedbackTransaction.Id.ToString());
            tokens.Should().ContainKey("ApprenticeFeedbackHostname").WhoseValue.Equals(appUrls.ApprenticeFeedbackUrl);
            tokens.Should().ContainKey("ApprenticeAccountHostname").WhoseValue.Equals(appUrls.ApprenticeAccountsUrl);
        }
    }
}
