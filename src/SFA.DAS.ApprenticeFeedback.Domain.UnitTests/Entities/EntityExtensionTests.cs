using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Domain.UnitTests.Entities
{
    public class EntityExtensionTests
    {
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
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Unknown, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, 0)]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnStatus(FeedbackTargetStatus status, int expectedCount)
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Status = (int)FeedbackTargetStatus.Unknown);
            targets.First().Status = (int)status;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count().Should().Be(expectedCount);
        }

        [Test]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Unknown, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_Complete, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooSoon, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Active, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooSoon, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.NotYetActive, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 1)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFeedbackRecently, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooSoon, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterPausing, 0)]
        [RecursiveMoqInlineAutoData(FeedbackTargetStatus.Complete, FeedbackEligibilityStatus.Deny_TooLateAfterWithdrawing, 0)]
        public void WhenCalling_FilterForEligibleActiveApprenticeFeedbackTargets_FiltersOnEligibility(FeedbackTargetStatus status, FeedbackEligibilityStatus eligibility, int expectedCount)
        {
            // Arrange
            var targets = CreateValidApprenticeFeedbackTargets();

            targets.ForEach(t => t.Status = (int)FeedbackTargetStatus.Unknown);
            targets.ForEach(t => t.FeedbackEligibility = (int)FeedbackEligibilityStatus.Unknown);
            var individualTarget = targets.First();
            individualTarget.Status = (int)status;
            individualTarget.FeedbackEligibility = (int)eligibility;

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

            targets.ForEach(t => t.Status = (int)FeedbackTargetStatus.NotYetActive);
            targets.ForEach(t => t.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooSoon);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);

            individualTarget.Status = (int)FeedbackTargetStatus.Active;
            individualTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;

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

            targets.ForEach(t => t.Status = (int)FeedbackTargetStatus.NotYetActive);
            targets.ForEach(t => t.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooSoon);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);

            individualTarget.Status = (int)FeedbackTargetStatus.Active;
            individualTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
            individualTarget.StartDate = DateTime.UtcNow;

            lastTarget.StartDate = DateTime.UtcNow.AddDays(-1);
            lastTarget.Status = (int)FeedbackTargetStatus.Active;
            lastTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;

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

            targets.ForEach(t => t.Status = (int)FeedbackTargetStatus.Complete);
            targets.ForEach(t => t.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_Complete);
            targets.ForEach(t => t.Ukprn = individualTarget.Ukprn);

            individualTarget.Status = (int)FeedbackTargetStatus.NotYetActive;
            individualTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_HasGivenFinalFeedback;
            individualTarget.StartDate = DateTime.UtcNow;

            lastTarget.StartDate = DateTime.UtcNow.AddDays(-1);
            lastTarget.Status = (int)FeedbackTargetStatus.NotYetActive;
            lastTarget.FeedbackEligibility = (int)FeedbackEligibilityStatus.Deny_TooLateAfterPausing;

            // Act
            targets = targets.FilterForEligibleActiveApprenticeFeedbackTargets().ToList();

            // Assert
            targets.Count.Should().Be(1);
            targets.First().Should().BeEquivalentTo(individualTarget);
        }

        private List<Domain.Entities.ApprenticeFeedbackTarget> CreateValidApprenticeFeedbackTargets()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var ukprns = fixture.CreateMany<long>(10);
            var targets = new List<Domain.Entities.ApprenticeFeedbackTarget>();
            foreach (var ukprn in ukprns)
            {
                var target = fixture.Create<Domain.Entities.ApprenticeFeedbackTarget>();

                target.Status = (int)FeedbackTargetStatus.Active;
                target.FeedbackEligibility = (int)FeedbackEligibilityStatus.Allow;
                target.Ukprn = ukprn;
                targets.Add(target);
            }

            return targets;
        }

    }
}
