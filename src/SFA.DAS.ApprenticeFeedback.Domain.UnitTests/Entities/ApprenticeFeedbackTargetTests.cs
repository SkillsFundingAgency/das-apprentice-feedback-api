﻿using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
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
            using (new AssertionScope())
            {
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
            target.UpdateApprenticeshipFeedbackTarget(null, null, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)FeedbackTargetStatus.Complete);
                target.EligibilityCalculationDate.Should().Be(now);
                target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_Complete);
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNull_AndTargetUnknown_StoresMyApprenticeshipDetails(
            MyApprenticeship myApprenticeship,
            ApplicationSettings settings,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Status = (int)FeedbackTargetStatus.Unknown;
            var now = DateTime.UtcNow;
            var dateTimeHelper = new SpecifiedTimeProvider(now);
            myApprenticeship.TrainingCode = 10.ToString();

            // Act
            target.UpdateApprenticeshipFeedbackTarget(null, myApprenticeship, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)FeedbackTargetStatus.Unknown);
                target.EligibilityCalculationDate.Should().Be(now);
                target.Ukprn.Should().Be(myApprenticeship.TrainingProviderId);
                target.ProviderName.Should().Be(myApprenticeship.TrainingProviderName);
                target.StandardUId.Should().Be(myApprenticeship.StandardUId);
                target.LarsCode.Should().Be(int.TryParse(myApprenticeship.TrainingCode, out int parsedValue) ? (int?)parsedValue : null);
                target.StartDate.Should().Be(myApprenticeship.StartDate);
                target.EndDate.Should().Be(myApprenticeship.EndDate);
            }
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
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.Ukprn.Should().NotBe(learner.Ukprn);
                target.ProviderName.Should().NotBe(learner.ProviderName);
                target.StandardName.Should().NotBe(learner.StandardName);
                target.StandardUId.Should().NotBe(learner.StandardUId);
                target.StartDate.Should().NotBe(learner.LearnStartDate);
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndLearnerIsNotNull_FieldsAreUpdated(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.Ukprn.Should().Be(learner.Ukprn);
                target.ProviderName.Should().Be(learner.ProviderName);
                target.StandardName.Should().Be(learner.StandardName);
                target.StandardUId.Should().Be(learner.StandardUId);
                target.StartDate.Should().Be(learner.LearnStartDate);
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_AndLearnActEndDateIsNotSet_SetsApprovalsStoppedDateAsEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            learner.CompletionStatus = 3;
            learner.IsTransfer = false;
            learner.LearnActEndDate = null;
            learner.ApprovalsStopDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.EndDate.Should().Be(learner.ApprovalsStopDate);
                target.Withdrawn.Should().BeTrue();
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_AndLearnActEndDateIsSet_SetsLearnActEndDateAsEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            learner.CompletionStatus = 3;
            learner.IsTransfer = false;
            learner.LearnActEndDate = learner.ApprovalsStopDate.Value.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.EndDate.Should().Be(learner.LearnActEndDate);
                target.Withdrawn.Should().BeTrue();
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_AndLearnActEndDateIsNotSet_ButIsTransfer_SetInProgress_AndEndDateIsEstimatedEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            learner.CompletionStatus = 3;
            learner.LearnActEndDate = null;
            learner.IsTransfer = true;
            learner.EstimatedEndDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.EndDate.Should().Be(learner.EstimatedEndDate);
                target.Withdrawn.Should().BeFalse();
                target.IsTransfer.Should().BeTrue();
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsStopped_AndLearnActEndDateIsSet_ButIsTransfer_SetInProgress_AndEndDateIsLearnActEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            learner.CompletionStatus = 3;
            learner.LearnActEndDate = DateTime.Now.AddDays(-7);
            learner.IsTransfer = true;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                 target.EndDate.Should().Be(learner.LearnActEndDate);
                 target.Withdrawn.Should().BeFalse();
                 target.IsTransfer.Should().BeTrue();
             }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsPaused_AndLearnActEndDateIsNotSet_SetsApprovalsPauseDateAsEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 6;
            learner.IsTransfer = false;
            learner.LearnActEndDate = null;
            learner.ApprovalsPauseDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.ApprovalsPauseDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsPaused_AndLearnActEndDateIsSet_SetsLearnActEndDateAsEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 6;
            learner.IsTransfer = false;
            learner.LearnActEndDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.LearnActEndDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsInProgress_AndLearnActEndDateIsNotSet_EndDateIsEstimatedEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 1;
            learner.LearnActEndDate = null;
            learner.EstimatedEndDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.EstimatedEndDate);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsInProgress_AndLearnActEndDateIsSet_EndDateIsLearnActEndDate(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            learner.CompletionStatus = 1;
            learner.LearnActEndDate = DateTime.Now.AddDays(-7);

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            target.EndDate.Should().Be(learner.LearnActEndDate);
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
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)FeedbackTargetStatus.Complete);
                target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_Complete);
                target.EligibilityCalculationDate.Should().Be(previousCalculatedDate);
            }
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
            learner.LearnActEndDate = dateTimeHelper.Now;
            learner.IsTransfer = false;
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
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)expectedStatus);
                target.FeedbackEligibility.Should().Be((int)expectedEligibility);
                target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            }
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
            learner.LearnActEndDate = dateTimeHelper.Now;

            // Set start date to now as well
            learner.LearnStartDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.FeedbackEligibility.Should().Be((int)expectedEligibility);
                target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            }
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
            learner.LearnActEndDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)FeedbackTargetStatus.NotYetActive);
                target.FeedbackEligibility.Should().Be((int)FeedbackEligibilityStatus.Deny_TooSoon);
                target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            }
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
            learner.LearnActEndDate = dateTimeHelper.Now;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper);

            // Assert
            using (new AssertionScope())
            {
                target.Status.Should().Be((int)expectedFeedbackTargetStatus);
                target.FeedbackEligibility.Should().Be((int)expectedFeedbackEligibilityStatus);
                target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
            }
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
            using (new AssertionScope())
            {
                target.EligibilityCalculationDate.Should().Be(dateTimeHelper.Now);
                target.FeedbackEligibility.Should().Be((int)expectedEligibility);
                target.Status.Should().Be((int)expectedStatus);
            }
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsWithdrawn_SetsWithdrawnAndSendsEmail_AddingNewTransaction(
            Learner learner,
            ApplicationSettings settings,
            Mock<IDateTimeHelper> dateTimeHelper,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            target.FeedbackTransactions = new List<Domain.Entities.FeedbackTransaction>();
            learner.CompletionStatus = 3;
            learner.IsTransfer = false;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, dateTimeHelper.Object);

            // Assert
            target.Withdrawn.Should().BeTrue();
            target.FeedbackTransactions.Should().HaveCount(1);
            target.FeedbackTransactions.Single().ApprenticeFeedbackTargetId.Should().Be(target.Id);
        }

        [Test, RecursiveMoqAutoData]
        public void WhenCalling_UpdateApprenticeshipFeedbackTarget_AndApprenticeshipIsWithdrawn_SetsWithdrawnAndSendsEmail_ReusingUnknownPendingEmailTransaction(
            Learner learner,
            ApplicationSettings settings,
            UtcTimeProvider utcTimeProvider,
            Domain.Entities.ApprenticeFeedbackTarget target)
        {
            // Arrange
            target.Withdrawn = false;
            target.FeedbackTransactions.Clear();

            var plus30Days = utcTimeProvider.Now.AddDays(30);
            var plus60Days = utcTimeProvider.Now.AddDays(60);
            var plus90Days = utcTimeProvider.Now.AddDays(90);

            var firstKnownPendingFeedbackTransaction = new Domain.Entities.FeedbackTransaction
            {
                SentDate = null,
                SendAfter = plus30Days,
                TemplateName = "FirstTemplateName",
                ApprenticeFeedbackTargetId = target.Id
            };

            var secondKnownPendingFeedbackTransaction = new Domain.Entities.FeedbackTransaction
            {
                SentDate = null,
                SendAfter = plus60Days,
                TemplateName = "SecondTemplateName",
                ApprenticeFeedbackTargetId = target.Id
            };

            var thirdKnownPendingFeedbackTransaction = new Domain.Entities.FeedbackTransaction
            {
                SentDate = null,
                SendAfter = plus90Days,
                TemplateName = "ThirdTemplateName",
                ApprenticeFeedbackTargetId = target.Id
            };

            var unknownPendingFeedbackTransaction = new Domain.Entities.FeedbackTransaction
            {
                SentDate = null,
                SendAfter = plus30Days,
                TemplateName = null,
                ApprenticeFeedbackTargetId = target.Id
            };
            
            target.FeedbackTransactions.Add(firstKnownPendingFeedbackTransaction);
            target.FeedbackTransactions.Add(unknownPendingFeedbackTransaction);
            target.FeedbackTransactions.Add(secondKnownPendingFeedbackTransaction);
            target.FeedbackTransactions.Add(thirdKnownPendingFeedbackTransaction);

            learner.CompletionStatus = 3; // Stopped
            learner.IsTransfer = false;

            // Act
            target.UpdateApprenticeshipFeedbackTarget(learner, null, settings, utcTimeProvider);

            // Assert
            using (new AssertionScope())
            {
                target.Withdrawn.Should().BeTrue();
                target.FeedbackTransactions.Should().HaveCount(4);

                var firstKnownFeedbackTransactionAfterUpdate = target.FeedbackTransactions.FirstOrDefault(p => p.TemplateName == "FirstTemplateName");
                firstKnownFeedbackTransactionAfterUpdate.SendAfter.Should().Be(plus30Days);

                var secondKnownFeedbackTransactionAfterUpdate = target.FeedbackTransactions.FirstOrDefault(p => p.TemplateName == "SecondTemplateName");
                secondKnownFeedbackTransactionAfterUpdate.SendAfter.Should().Be(plus60Days);

                var thirdKnownFeedbackTransactionAfterUpdate = target.FeedbackTransactions.FirstOrDefault(p => p.TemplateName == "ThirdTemplateName");
                thirdKnownFeedbackTransactionAfterUpdate.SendAfter.Should().Be(plus90Days);

                var unknownFeedbackTransactionAfterUpdate = target.FeedbackTransactions.FirstOrDefault(p => string.IsNullOrEmpty(p.TemplateName));
                unknownFeedbackTransactionAfterUpdate.SendAfter.Should().BeNull();
            }
        }
    }
}
