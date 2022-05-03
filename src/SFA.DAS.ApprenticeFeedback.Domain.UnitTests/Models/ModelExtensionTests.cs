using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Models
{
    public class ModelExtensionTests
    {
        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnId()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Id = null);
            targets.First().Id = Guid.NewGuid();

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(1);
        }

        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnStartDate()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.StartDate = null);
            targets.First().StartDate = DateTime.UtcNow;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(1);
        }

        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnUkprn()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Ukprn = null);
            targets.First().Ukprn = 123456789;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(1);
        }

        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Unknown, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, 0)]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnStatus(FeedbackTargetStatus status, int expectedCount)
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Status = FeedbackTargetStatus.Unknown);
            targets.First().Status = status;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(expectedCount);
        }

        [Test]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Unknown, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_Complete, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooSoon, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_NotEnoughActiveApprentices, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPassing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooSoon, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_NotEnoughActiveApprentices, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterPassing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 1)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooSoon, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_NotEnoughActiveApprentices, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPassing, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 0)]
        [MoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 0)]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnEligibility(FeedbackTargetStatus status, FeedbackEligibilityStatus eligibility, int expectedCount)
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Status = FeedbackTargetStatus.Unknown);
            targets.ForEach(t => t.FeedbackEligibility = FeedbackEligibilityStatus.Unknown);
            var individualTarget = targets.First();
            individualTarget.Status = status;
            individualTarget.FeedbackEligibility = eligibility;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(expectedCount);
        }

        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_PrioritisesActiveRecords()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();
            var individualTarget = targets.First();

            targets.ForEach(t => t.Status = FeedbackTargetStatus.NotYetActive);
            targets.ForEach(t => t.FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooSoon);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);

            individualTarget.Status = FeedbackTargetStatus.Active;
            individualTarget.FeedbackEligibility = FeedbackEligibilityStatus.Allow;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count.Should().Be(1);
            targets.First().Should().BeEquivalentTo(individualTarget);
        }

        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_PrioritisesEarliestRecord()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();
            var individualTarget = targets.First();
            var lastTarget = targets.Last();

            targets.ForEach(t => t.Status = FeedbackTargetStatus.NotYetActive);
            targets.ForEach(t => t.FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooSoon);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);

            individualTarget.Status = FeedbackTargetStatus.Active;
            individualTarget.FeedbackEligibility = FeedbackEligibilityStatus.Allow;
            individualTarget.StartDate = DateTime.UtcNow;

            lastTarget.StartDate = DateTime.UtcNow.AddDays(-1);
            lastTarget.Status = FeedbackTargetStatus.Active;
            lastTarget.FeedbackEligibility = FeedbackEligibilityStatus.Allow;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count.Should().Be(1);
            targets.First().Should().BeEquivalentTo(individualTarget);
        }

        [Test, MoqAutoData]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_PrioritisesEarliestNotCompleteRecord()
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();
            var individualTarget = targets.First();
            var lastTarget = targets.Last();

            targets.ForEach(t => t.Status = FeedbackTargetStatus.Complete);
            targets.ForEach(t => t.FeedbackEligibility = FeedbackEligibilityStatus.Deny_Complete);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);
            
            individualTarget.Status = FeedbackTargetStatus.NotYetActive;
            individualTarget.FeedbackEligibility = FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
            individualTarget.StartDate = DateTime.UtcNow;

            lastTarget.StartDate = DateTime.UtcNow.AddDays(-1);
            lastTarget.Status = FeedbackTargetStatus.NotYetActive;
            lastTarget.FeedbackEligibility = FeedbackEligibilityStatus.Deny_TooLateAfterPausing;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count.Should().Be(1);
            targets.First().Should().BeEquivalentTo(individualTarget);
        }

        private List<ApprenticeFeedbackTarget> CreateValidApprenticeFeedbackTargets()
        {
            var fixture = new Fixture();
            var ukprns = fixture.CreateMany<long>(10);
            var targets = new List<ApprenticeFeedbackTarget>();
            foreach (var ukprn in ukprns)
            {
                var target = fixture.Create<ApprenticeFeedbackTarget>();

                target.Status = Enums.FeedbackTargetStatus.Active;
                target.FeedbackEligibility = Enums.FeedbackEligibilityStatus.Allow;
                target.Ukprn = ukprn;
                targets.Add(target);
            }

            return targets;
        }

    }
}
