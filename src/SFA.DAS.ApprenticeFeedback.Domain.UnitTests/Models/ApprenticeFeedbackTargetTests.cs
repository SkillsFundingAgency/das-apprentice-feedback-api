using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class ApprenticeFeedbackTargetTests
    {
        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, true)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsActive(FeedbackTargetStatus status, bool IsActive, ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = status;

            // Act & Assert
            target.IsActive().Should().Be(IsActive);
        }

        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, true)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsNotYetActive(FeedbackTargetStatus status, bool NotYetActive, ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = status;

            // Act & Assert
            target.IsInactive().Should().Be(NotYetActive);
        }

        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, true)]
        [MoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsComplete(FeedbackTargetStatus status, bool isComplete, ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = status;

            // Act & Assert
            target.IsComplete().Should().Be(isComplete);
        }

        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Allow, true)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Unknown, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_Complete, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPassing, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooSoon, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Allow, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Allow, false)]
        [MoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Allow, false)]
        public void WhenCalling_IsActiveAndEligible_SpecifiesIfTargetIsEligibleAndActive(FeedbackTargetStatus status, FeedbackEligibilityStatus eligibility, bool isComplete, ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.FeedbackEligibility = eligibility;
            target.Status = status;

            // Act & Assert
            target.IsActiveAndEligible().Should().Be(isComplete);
        }

        [Test]
        [MoqInlineAutoData(9, 10, false)]
        [MoqInlineAutoData(10, 10, true)]
        [MoqInlineAutoData(11, 10, true)]
        //If Start Date is same as cut off date, then we allow feedback to be given
        public void WhenCalling_HasApprenticeshipStartedForFeedback_SpecifiesIfApprenticeshipHasStarted(
            int advanceDateDays,
            int initialDenyPeriodDays,
            bool hasApprenticeshipStarted,
            ApprenticeFeedbackTarget target,
            ApplicationSettings settings)
        {
            // Arrange
            var now = DateTime.UtcNow;
            target.StartDate = now;
            settings.InitialDenyPeriodDays = initialDenyPeriodDays;
            var timeHelper = new SpecifiedTimeProvider(now);
            timeHelper.Advance(TimeSpan.FromDays(advanceDateDays));

            // Act & Assert
            target.HasApprenticeshipStartedForFeedback(settings, timeHelper).Should().Be(hasApprenticeshipStarted);
        }

        [Test]
        [MoqInlineAutoData(9, 10, false)]
        [MoqInlineAutoData(10, 10, false)]
        [MoqInlineAutoData(11, 10, true)]
        //If End Date is same as Cut off, we say apprenticeship hasn't finished, to allow feedback to be
        //Given up to the final day
        public void WhenCalling_HasApprenticeshipFinishedForFeedback_SpecifiesIfApprenticeshipHasFinished(
            int advanceDateDays,
            int finalAllowPeriodDays,
            bool hasApprenticeshipStarted,
            ApprenticeFeedbackTarget target,
            ApplicationSettings settings)
        {
            // Arrange
            var now = DateTime.UtcNow;
            target.EndDate = now;
            settings.FinalAllowedPeriodDays = finalAllowPeriodDays;
            var timeHelper = new SpecifiedTimeProvider(now);
            timeHelper.Advance(TimeSpan.FromDays(advanceDateDays));

            // Act & Assert
            target.HasApprenticeshipFinishedForFeedback(settings, timeHelper).Should().Be(hasApprenticeshipStarted);
        }

        [Test]
        [MoqInlineAutoData(9, 10, true)]
        [MoqInlineAutoData(10, 10, false)]
        [MoqInlineAutoData(11, 10, false)]
        //If feedback was given the same day as the cut off, we now allow it
        public void WhenCalling_HasApprenticeProvidedFeedbackRecently_SpecifiesIfFeedbackHasBeenRecent(
            int advanceDateDays,
            int recentDenyPeriodDays,
            bool hasRecentlyGivenFeedback,
            ApprenticeFeedbackTarget target,
            ApplicationSettings settings)
        {
            var now = DateTime.UtcNow;
            target.LastFeedbackSubmittedDate = now;
            settings.RecentDenyPeriodDays = recentDenyPeriodDays;
            var timeHelper = new SpecifiedTimeProvider(now);
            timeHelper.Advance(TimeSpan.FromDays(advanceDateDays));

            target.HasRecentlyProvidedFeedback(settings, timeHelper).Should().Be(hasRecentlyGivenFeedback);
        }

        [Test]
        [MoqInlineAutoData(-1, false)]
        [MoqInlineAutoData(0, true)]
        [MoqInlineAutoData(1, true)]
        //If feedback was given the same date as the end date we classify it as final feedback
        public void WhenCalling_HasApprenticeProvidedFinalFeedback_SpecifiesIfFinalFeedbackHasBeenGiven(
            int advanceDateDays,
            bool hasGivenFinalFeedback,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.EndDate = DateTime.UtcNow;
            var lastFeedbackCompletedDate = DateTime.UtcNow.AddDays(advanceDateDays);

            // Act & Assert
            target.HasProvidedFinalFeedback(lastFeedbackCompletedDate).Should().Be(hasGivenFinalFeedback);
        }

        [Test, MoqAutoData]
        public void WhenCalling_ResetFeedbackTarget_SetsDefaultsOnTarget(ApprenticeFeedbackTarget target)
        {
            // Act
            target.ResetFeedbackTarget();

            // Assert
            target.StartDate.Should().BeNull();
            target.EndDate.Should().BeNull();
            target.Ukprn.Should().BeNull();
            target.ProviderName.Should().BeNull();
            target.StandardUId.Should().BeNull();
            target.StandardName.Should().BeNull();
            target.EligibilityCalculationDate.Should().BeNull();
            target.FeedbackEligibility.Should().Be(FeedbackEligibilityStatus.Unknown);
            target.Status.Should().Be(FeedbackTargetStatus.Unknown);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNull_AndTargetActive_UpdatesTargetToComplete(
            ApplicationSettings settings,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = FeedbackTargetStatus.Active;
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(null, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be(FeedbackTargetStatus.Complete);
            target.EligibilityCalculationDate.Should().Be(now);
            target.FeedbackEligibility.Should().Be(FeedbackEligibilityStatus.Deny_Complete);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNotNull_FieldsAreNotUpdated_IfTargetIsComplete(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = FeedbackTargetStatus.Complete;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.Ukprn.Should().NotBe(learner.Ukprn);
            target.ProviderName.Should().NotBe(learner.ProviderName);
            target.StandardName.Should().NotBe(learner.StandardName);
            target.StandardUId.Should().NotBe(learner.StandardUId);
            target.StartDate.Should().NotBe(learner.LearnStartDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNotNull_FieldsAreUpdated(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.Ukprn.Should().Be(learner.Ukprn);
            target.ProviderName.Should().Be(learner.ProviderName);
            target.StandardName.Should().Be(learner.StandardName);
            target.StandardUId.Should().Be(learner.StandardUId);
            target.StartDate.Should().Be(learner.LearnStartDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsPassed_SetsAchievementDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.Outcome = "pass";

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.AchievementDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_SetsStoppedDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 3;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.ApprovalsStopDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsPaused_SetsPausedDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 6;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.ApprovalsPauseDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsInProgress_SetsInProgress(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 1;
            learner.Outcome = "";

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.EstimatedEndDate);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsComplete_NoStatusOrEligibilityChanges(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = FeedbackTargetStatus.Complete;
            target.FeedbackEligibility = FeedbackEligibilityStatus.Deny_Complete;
            var previousCalculatedDate = target.EligibilityCalculationDate;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.Status.Should().Be(FeedbackTargetStatus.Complete);
            target.FeedbackEligibility.Should().Be(FeedbackEligibilityStatus.Deny_Complete);
            target.EligibilityCalculationDate.Should().Be(previousCalculatedDate);
        }

        [Test]
        //Outcome, CompletionStatus, recentFeedbackToEndDate, expected status, expected eligibility
        [MoqInlineAutoData("", 1, true, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [MoqInlineAutoData("pass", 4, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPassing)]
        [MoqInlineAutoData("", 3, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [MoqInlineAutoData("", 6, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [MoqInlineAutoData("", 5, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_Complete)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForHasApprenticeFinishedForFeedback_SetsCorrectEligiblityAndStatus(
            string outcome,
            int completionStatus,
            bool recentFeedbackToEndDate,
            FeedbackTargetStatus expectedStatus,
            FeedbackEligibilityStatus expectedEligibility,
            Learner learner,
            ApplicationSettings settings,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            //Setup for ApprenticeFinished for Feedback
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.AchievementDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;
            // Setup for feedback
            target.LastFeedbackSubmittedDate = recentFeedbackToEndDate ? now.AddDays(1) : now.AddDays(-1);

            //Advance time 1 day beyond the final allowed period days
            settings.FinalAllowedPeriodDays = 30;
            dateTimeHelper.Advance(TimeSpan.FromDays(31));

            // Set the learner up with expected outcome and completion status
            learner.Outcome = outcome;
            learner.CompletionStatus = completionStatus;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be(expectedStatus);
            target.FeedbackEligibility.Should().Be(expectedEligibility);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        //recentFeedbackToEndDate, expected eligibility
        [MoqInlineAutoData(true, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [MoqInlineAutoData(false, FeedbackEligibilityStatus.Allow)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedButActive_SetsCorrectEligiblityAndStatus(
            bool recentFeedbackForApprenticeship,
            FeedbackEligibilityStatus expectedEligibility,
            Learner learner,
            ApplicationSettings settings,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            // Final Allow is 10 days, but as time not advanced it won't be finished.
            // Recent Deny set to 10
            settings.FinalAllowedPeriodDays = 10;
            settings.RecentDenyPeriodDays = 10;

            //Setup for Apprenticeship to be active but not finished
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);
            target.Status = FeedbackTargetStatus.Active;
            target.LastFeedbackSubmittedDate = recentFeedbackForApprenticeship ? now.AddDays(-5) : now.AddDays(-15);

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.AchievementDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Set start date to now as well
            learner.LearnStartDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.FeedbackEligibility.Should().Be(expectedEligibility);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test, MoqAutoData]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedNotActiveNotStarted_SetsStatusToNotYetActiveAndTooSoon(
            Learner learner,
            ApplicationSettings settings,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            // Final Allow is 10 days, but as time not advanced it won't be finished.
            // Initial Deny Set to 10
            settings.FinalAllowedPeriodDays = 10;
            settings.InitialDenyPeriodDays = 10;

            //Setup for Apprenticeship to be Not Active, Not Finished and Not Started
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);
            learner.LearnStartDate = dateTimeHelper.Now;

            // Advance time but still within the Initial Deny Window
            dateTimeHelper.Advance(TimeSpan.FromDays(5));
            target.Status = FeedbackTargetStatus.NotYetActive;

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.AchievementDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be(FeedbackTargetStatus.NotYetActive);
            target.FeedbackEligibility.Should().Be(FeedbackEligibilityStatus.Deny_TooSoon);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        [MoqInlineAutoData(FeedbackEligibilityStatus.Allow, FeedbackTargetStatus.Active)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedNotActiveButNowStarted_SetsStatusAndEligibilityCorrectly(
            FeedbackEligibilityStatus expectedFeedbackEligibilityStatus,
            FeedbackTargetStatus expectedFeedbackTargetStatus,
            Learner learner,
            ApplicationSettings settings,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            // Final Allow is 10 days, but as time not advanced it won't be finished.
            // Initial Deny Set to 10
            settings.FinalAllowedPeriodDays = 10;
            settings.InitialDenyPeriodDays = 10;

            //Setup for Apprenticeship to be Not Active, Not Finished and Not Started
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);
            learner.LearnStartDate = dateTimeHelper.Now;

            // Advance time but now beyond the initial deny window
            dateTimeHelper.Advance(TimeSpan.FromDays(15));
            target.Status = FeedbackTargetStatus.Unknown;

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.AchievementDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be(expectedFeedbackTargetStatus);
            target.FeedbackEligibility.Should().Be(expectedFeedbackEligibilityStatus);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        [MoqInlineAutoData(-10, FeedbackTargetStatus.Active, FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [MoqInlineAutoData(10, FeedbackTargetStatus.Active, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        public void WhenCalling_UpdateTargetAfterFeedback_SetsCorrectEligibilityAndCalculationDate(
            int advanceDays,
            FeedbackTargetStatus startingStatus,
            FeedbackTargetStatus expectedStatus,
            FeedbackEligibilityStatus expectedEligibility,
            ApprenticeFeedbackTarget target)
        {
            // Arrange
            var dateTimeHelper = new SpecifiedTimeProvider(DateTime.UtcNow);
            target.EndDate = dateTimeHelper.Now;
            target.Status = startingStatus;
            dateTimeHelper.Advance(TimeSpan.FromDays(advanceDays));

            // Act
            target.UpdateTargetAfterFeedback(dateTimeHelper.Now);

            // Assert
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            target.FeedbackEligibility.Should().Be(expectedEligibility);
            target.Status.Should().Be(expectedStatus);
        }
    }
}
