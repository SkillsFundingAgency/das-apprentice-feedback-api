using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class WhenMappingFromApprenticeFeedbackTargetToProvider
    {
        [Test, RecursiveMoqAutoData]
        public void ThenTheFieldsAreCorrectlyMapped(ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            var result = TrainingProvider.Create(source, appSettings);

            result.ApprenticeFeedbackTargetId.Should().Be(source.Id.Value);
            result.Ukprn.Should().Be(source.Ukprn);
            result.StartDate.Should().Be(source.StartDate);
            result.EndDate.Should().Be(source.EndDate);
            result.ProviderName.Should().Be(source.ProviderName);   
            result.Status.Should().Be(source.Status);
            result.FeedbackEligibility.Should().Be(source.FeedbackEligibility);
            result.SignificantDate.Should().Be(null);
        }

        [Test, MoqAutoData]
        public void AndEligibilityIsTooRecentThenTimeFieldsAreSetCorrectly(ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            source.FeedbackEligibility = Enums.FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently;
            var result = TrainingProvider.Create(source, appSettings);

            result.SignificantDate.Should().Be(source.LastFeedbackCompletedDate.Value.Date.AddDays(appSettings.RecentDenyPeriodDays));
        }

        [Test, MoqAutoData]
        public void AndEligibilityIsFinalDoneThenTimeFieldsAreSetCorrectly(ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            source.FeedbackEligibility = Enums.FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
            var result = TrainingProvider.Create(source, appSettings);

            result.TimeWindow.Should().Be(TimeSpan.FromDays(appSettings.InitialDenyPeriodDays));
        }

        [Test, MoqAutoData]
        public void AndEligibilityIsTooSoonThenTimeFieldsAreSetCorrectly(ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            source.FeedbackEligibility = Enums.FeedbackEligibilityStatus.Deny_TooSoon;
            var result = TrainingProvider.Create(source, appSettings);

            result.TimeWindow.Should().Be(TimeSpan.FromDays(appSettings.InitialDenyPeriodDays));
            result.SignificantDate.Should().Be(source.StartDate.Value.Date.AddDays(appSettings.InitialDenyPeriodDays));
        }

        [Test]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Deny_TooLateAfterPassing)]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        public void AndEligibilityIsTooLateThenTimeFieldsAreSetCorrectly(Enums.FeedbackEligibilityStatus eligibility, ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            source.FeedbackEligibility = eligibility;
            var result = TrainingProvider.Create(source, appSettings);

            result.TimeWindow.Should().Be(TimeSpan.FromDays(appSettings.FinalAllowedPeriodDays));
        }

        [Test]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Allow)]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Deny_Complete)]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Deny_NotEnoughActiveApprentices)]
        [MoqInlineAutoData(Enums.FeedbackEligibilityStatus.Unknown)]
        public void AndTimeFieldsAreNotRequiredThenTimeFieldsAreNull(Enums.FeedbackEligibilityStatus eligibility, ApprenticeFeedbackTarget source, ApplicationSettings appSettings)
        {
            source.FeedbackEligibility = eligibility;
            var result = TrainingProvider.Create(source, appSettings);

            result.TimeWindow.Should().BeNull();
            result.SignificantDate.Should().BeNull();
        }
    }
}
