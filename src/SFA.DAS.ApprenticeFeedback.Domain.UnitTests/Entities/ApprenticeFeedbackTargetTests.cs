using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Entities
{
    public class ApprenticeFeedbackTargetTests
    {
        [Test]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, true)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsActive(FeedbackTargetStatus status, bool IsActive, Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)status;

            // Act & Assert
            target.IsActive().Should().Be(IsActive);
        }

        [Test]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, true)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsNotYetActive(FeedbackTargetStatus status, bool NotYetActive, Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)status;

            // Act & Assert
            target.IsInactive().Should().Be(NotYetActive);
        }

        [Test]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, true)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Unknown, false)]
        public void WhenCalling_IsActive_SpecifiesIfTargetIsComplete(FeedbackTargetStatus status, bool isComplete, Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)status;

            // Act & Assert
            target.IsComplete().Should().Be(isComplete);
        }

        [Test]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Allow, true)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Unknown, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_Complete, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooSoon, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Allow, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Allow, false)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Unknown, FeedbackEligibilityStatus.Allow, false)]
        public void WhenCalling_IsActiveAndEligible_SpecifiesIfTargetIsEligibleAndActive(FeedbackTargetStatus status, FeedbackEligibilityStatus eligibility, bool isComplete, Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.FeedbackEligibility = (int)eligibility;
            target.Status = (int)status;

            // Act & Assert
            target.IsActiveAndEligible().Should().Be(isComplete);
        }

        [Test]
        [RecursiveMoqInlineAutoData(9, 10, false)]
        [RecursiveMoqInlineAutoData(10, 10, true)]
        [RecursiveMoqInlineAutoData(11, 10, true)]
        //If Start Date is same as cut off date, then we allow feedback to be given
        public void WhenCalling_HasApprenticeshipStartedForFeedback_SpecifiesIfApprenticeshipHasStarted(
            int advanceDateDays,
            int initialDenyPeriodDays,
            bool hasApprenticeshipStarted,
            Domain.Entities.ApprenticeFeedbackTarget target,
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
        [RecursiveMoqInlineAutoData(9, 10, false)]
        [RecursiveMoqInlineAutoData(10, 10, false)]
        [RecursiveMoqInlineAutoData(11, 10, true)]
        //If End Date is same as Cut off, we say apprenticeship hasn't finished, to allow feedback to be
        //Given up to the final day
        public void WhenCalling_HasApprenticeshipFinishedForFeedback_SpecifiesIfApprenticeshipHasFinished(
            int advanceDateDays,
            int finalAllowPeriodDays,
            bool hasApprenticeshipStarted,
            Domain.Entities.ApprenticeFeedbackTarget target,
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
        [RecursiveMoqInlineAutoData(9, 10, true)]
        [RecursiveMoqInlineAutoData(10, 10, false)]
        [RecursiveMoqInlineAutoData(11, 10, false)]
        //If feedback was given the same day as the cut off, we now allow it
        public void WhenCalling_HasApprenticeProvidedFeedbackRecently_SpecifiesIfFeedbackHasBeenRecent(
            int advanceDateDays,
            int recentDenyPeriodDays,
            bool hasRecentlyGivenFeedback,
            Domain.Entities.ApprenticeFeedbackTarget target,
            ApprenticeFeedbackResult appFeedbackResult,
            ApplicationSettings settings)
        {
            var now = DateTime.UtcNow;
            appFeedbackResult.DateTimeCompleted = now;
            target.ApprenticeFeedbackResults.Clear();
            target.ApprenticeFeedbackResults.Add(appFeedbackResult);
            settings.RecentDenyPeriodDays = recentDenyPeriodDays;
            var timeHelper = new SpecifiedTimeProvider(now);
            timeHelper.Advance(TimeSpan.FromDays(advanceDateDays));

            target.HasRecentlyProvidedFeedback(settings, timeHelper).Should().Be(hasRecentlyGivenFeedback);
        }

        [Test]
        [RecursiveMoqInlineAutoData(-1, false)]
        [RecursiveMoqInlineAutoData(0, true)]
        [RecursiveMoqInlineAutoData(1, true)]
        //If feedback was given the same date as the end date we classify it as final feedback
        public void WhenCalling_HasApprenticeProvidedFinalFeedback_SpecifiesIfFinalFeedbackHasBeenGiven(
            int advanceDateDays,
            bool hasGivenFinalFeedback,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.EndDate = DateTime.UtcNow;
            var lastFeedbackCompletedDate = DateTime.UtcNow.AddDays(advanceDateDays);

            // Act & Assert
            target.HasProvidedFinalFeedback(lastFeedbackCompletedDate).Should().Be(hasGivenFinalFeedback);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_ResetFeedbackTarget_SetsDefaultsOnTarget(Domain.Entities.ApprenticeFeedbackTarget target)
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
            target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Unknown);
            target.Status.Should().Be((int)FeedbackTargetStatus.Unknown);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNull_AndTargetActive_UpdatesTargetToComplete(
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)FeedbackTargetStatus.Active;
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(null, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be((int)FeedbackTargetStatus.Complete);
            target.EligibilityCalculationDate.Should().Be(now);
            target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_Complete);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNotNull_FieldsAreNotUpdated_IfTargetIsComplete(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)FeedbackTargetStatus.Complete;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.Ukprn.Should().NotBe(learner.Ukprn);
            target.ProviderName.Should().NotBe(learner.ProviderName);
            target.StandardName.Should().NotBe(learner.StandardName);
            target.StandardUId.Should().NotBe(learner.StandardUId);
            target.StartDate.Should().NotBe(learner.LearnStartDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNotNull_FieldsAreUpdated(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
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

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_SetsStoppedDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 3;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.ApprovalsStopDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsPaused_SetsPausedDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 6;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.ApprovalsPauseDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsInProgress_SetsInProgress(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 1;
            
            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.EstimatedEndDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsComplete_NoStatusOrEligibilityChanges(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)FeedbackTargetStatus.Complete;
            target.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_Complete;
            var previousCalculatedDate = target.EligibilityCalculationDate;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper.Object);

            // Assert
            target.Status.Should().Be((int)FeedbackTargetStatus.Complete);
            target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_Complete);
            target.EligibilityCalculationDate.Should().Be(previousCalculatedDate);
        }

        [Test]
        //CompletionStatus, recentFeedbackToEndDate, expected status, expected eligibility
        [RecursiveMoqInlineAutoData(1, true, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        [RecursiveMoqInlineAutoData(3, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing)]
        [RecursiveMoqInlineAutoData(6, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing)]
        [RecursiveMoqInlineAutoData(5, false, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_Complete)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForHasApprenticeFinishedForFeedback_SetsCorrectEligiblityAndStatus(
            int completionStatus,
            bool recentFeedbackToEndDate,
            FeedbackTargetStatus expectedStatus,
            FeedbackEligibilityStatus expectedEligibility,
            Learner learner,
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target,
            ApprenticeFeedbackResult appFeedbackResult)
        {
            // Arrange
            //Setup for ApprenticeFinished for Feedback
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;
            // Setup for feedback
            appFeedbackResult.DateTimeCompleted = recentFeedbackToEndDate ? now.AddDays(1) : now.AddDays(-1);
            target.ApprenticeFeedbackResults.Clear();
            target.ApprenticeFeedbackResults.Add(appFeedbackResult);
            
            //Advance time 1 day beyond the final allowed period days
            settings.FinalAllowedPeriodDays = 30;
            dateTimeHelper.Advance(TimeSpan.FromDays(31));

            // Set the learner up with expected outcome and completion status
            learner.CompletionStatus = completionStatus;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be((int)expectedStatus);
            target.FeedbackEligibility.Should().Be((int)expectedEligibility);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        //recentFeedbackToEndDate, expected eligibility
        [RecursiveMoqInlineAutoData(true, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [RecursiveMoqInlineAutoData(false, FeedbackEligibilityStatus.Allow)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedButActive_SetsCorrectEligiblityAndStatus(
            bool recentFeedbackForApprenticeship,
            FeedbackEligibilityStatus expectedEligibility,
            Learner learner,
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target,
            ApprenticeFeedbackResult appFeedbackResult)
        {
            // Arrange
            // Final Allow is 10 days, but as time not advanced it won't be finished.
            // Recent Deny set to 10
            settings.FinalAllowedPeriodDays = 10;
            settings.RecentDenyPeriodDays = 10;

            //Setup for Apprenticeship to be active but not finished
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);
            target.Status = (int)FeedbackTargetStatus.Active;

            appFeedbackResult.DateTimeCompleted = recentFeedbackForApprenticeship ? now.AddDays(-5) : now.AddDays(-15);
            target.ApprenticeFeedbackResults.Clear();
            target.ApprenticeFeedbackResults.Add(appFeedbackResult);
            
            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Set start date to now as well
            learner.LearnStartDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.FeedbackEligibility.Should().Be((int)expectedEligibility);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedNotActiveNotStarted_SetsStatusToNotYetActiveAndTooSoon(
            Learner learner,
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target)
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
            target.Status = (int)FeedbackTargetStatus.NotYetActive;

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be((int)FeedbackTargetStatus.NotYetActive);
            target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_TooSoon);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        [RecursiveMoqInlineAutoData(FeedbackEligibilityStatus.Allow, FeedbackTargetStatus.Active)]
        public void WhenCalling_UpdateAndSettingStatusAndEligibility_ForNotFinishedNotActiveButNowStarted_SetsStatusAndEligibilityCorrectly(
            FeedbackEligibilityStatus expectedFeedbackEligibilityStatus,
            FeedbackTargetStatus expectedFeedbackTargetStatus,
            Learner learner,
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target)
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
            target.Status = (int)FeedbackTargetStatus.Unknown;

            // Set all end dates to the same, we don't care which one for this test.
            learner.EstimatedEndDate = dateTimeHelper.Now;
            learner.ApprovalsPauseDate = dateTimeHelper.Now;
            learner.ApprovalsStopDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, settings, dateTimeHelper);

            // Assert
            target.Status.Should().Be((int)expectedFeedbackTargetStatus);
            target.FeedbackEligibility.Should().Be((int)expectedFeedbackEligibilityStatus);
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
        }

        [Test]
        [RecursiveMoqInlineAutoData(-10, FeedbackTargetStatus.Active, FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently)]
        [RecursiveMoqInlineAutoData(10, FeedbackTargetStatus.Active, FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback)]
        public void WhenCalling_UpdateTargetAfterFeedback_SetsCorrectEligibilityAndCalculationDate(
            int advanceDays,
            FeedbackTargetStatus startingStatus,
            FeedbackTargetStatus expectedStatus,
            FeedbackEligibilityStatus expectedEligibility,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            var dateTimeHelper = new SpecifiedTimeProvider(DateTime.UtcNow);
            target.EndDate = dateTimeHelper.Now;
            target.Status = (int)startingStatus;
            dateTimeHelper.Advance(TimeSpan.FromDays(advanceDays));

            // Act
            target.UpdateTargetAfterFeedback(dateTimeHelper.Now);

            // Assert
            target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            target.FeedbackEligibility.Should().Be((int)expectedEligibility);
            target.Status.Should().Be((int)expectedStatus);
        }
    }
}
