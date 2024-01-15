using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.UnitTests.Queries
{
    public class WhenRequestingFeedbackTargetsForUpdate
    {
        #region Test Apprentice Feedback Targets

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_NotStarted(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(1),
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_UnknownStart(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = null,
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_Completed(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Complete
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_NotCompletedOrWithdrawn(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active
            };
        }


        private static Domain.Entities.ApprenticeFeedbackTarget AFT_EligibilityCalculatedRecently(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-2),
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_NoFeedback(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-100),
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_FeedbackGivenRecently(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-100),
                ApprenticeFeedbackResults = new List<Domain.Entities.ApprenticeFeedbackResult>()
                {
                    new Domain.Entities.ApprenticeFeedbackResult()
                    {
                        DateTimeCompleted = DateTime.UtcNow.AddDays(-2)
                    }
                }
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_EligibleToGiveFeedback(Guid apprenticeId, long apprenticeshipId)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ApprenticeId = apprenticeId,
                ApprenticeshipId = apprenticeshipId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-100),
                ApprenticeFeedbackResults = new List<Domain.Entities.ApprenticeFeedbackResult>()
                {
                    new Domain.Entities.ApprenticeFeedbackResult()
                    {
                        DateTimeCompleted = DateTime.UtcNow.AddDays(-95)
                    }
                }
            };
        }

        #endregion Test Apprentice Feedback Targets

        #region IQueryableExtensionTests

        public class IQueryableExtensionTests
        {
            public class HasStartedOrUnknown
            {
                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipNotStarted_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper
                    )
                {
                    context.Add(AFT_NotStarted(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .HasStartedOrUnknown(dateTimeHelper)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStartUnknown_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper
                    )
                {
                    context.Add(AFT_UnknownStart(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .HasStartedOrUnknown(dateTimeHelper)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStarted_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper
                    )
                {
                    context.Add(AFT_Completed(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .HasStartedOrUnknown(dateTimeHelper)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class StatusNotCompleted
            {
                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStatusCompleted_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
                    )
                {
                    context.Add(AFT_Completed(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .StatusNotCompleted()
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStatusNotCompletedOrWithdrawn_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
                    )
                {
                    context.Add(AFT_NotCompletedOrWithdrawn(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .StatusNotCompleted()
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class FeedbackEligibilityNotCalculatedRecently
            {
                [Test]
                [AutoMoqData]
                public void When_EligibilityCalculatedRecently_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_EligibilityCalculatedRecently(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    appSettings.EligibilityCalculationThrottleDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .FeedbackEligibilityNotCalculatedRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_EligibilityNotCalculatedRecently_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_FeedbackGivenRecently(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    appSettings.EligibilityCalculationThrottleDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .FeedbackEligibilityNotCalculatedRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class NotGivenFeedbackRecently
            {
                [Test]
                [AutoMoqData]
                public void When_NoFeedback_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_NoFeedback(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    appSettings.RecentDenyPeriodDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .NotGivenFeedbackRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }

                [Test]
                [AutoMoqData]
                public void When_GivenFeedbackRecently_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_FeedbackGivenRecently(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    appSettings.RecentDenyPeriodDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .NotGivenFeedbackRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_NotGivenFeedbackRecently_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_EligibleToGiveFeedback(Guid.NewGuid(), 1));
                    context.SaveChanges();

                    appSettings.RecentDenyPeriodDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .NotGivenFeedbackRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }
        }

        #endregion IQueryableExtensionTests

        #region Test Cases

        // Create the test data object
        static List<Domain.Entities.ApprenticeFeedbackTarget> testCase4 = 
            new List<Domain.Entities.ApprenticeFeedbackTarget>
            {
                AFT_EligibleToGiveFeedback(Guid.NewGuid(), 4)
            };

        static TestCaseData[] FeedbackTargetTestData =
        {
            // Test case 0 - Apprenticeship has not started
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_NotStarted(Guid.NewGuid(), 0)
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 1 - Apprenticeship has completed
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_Completed(Guid.NewGuid(), 1)
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 2 - Eligilibity was calculated recently
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_EligibilityCalculatedRecently(Guid.NewGuid(), 2)
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 3 - Feedback given recently
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_FeedbackGivenRecently(Guid.NewGuid(), 3)
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 4 - Eligible for feedback
            new TestCaseData(testCase4,
                1  // batch size
            ).Returns(
                testCase4
            )
        };

        #endregion Test Cases

        [Test]
        [AutoMoqInlineAutoData(0)]
        [AutoMoqInlineAutoData(1)]
        [AutoMoqInlineAutoData(2)]
        [AutoMoqInlineAutoData(3)]
        [AutoMoqInlineAutoData(4)]
        public async Task ThenFeedbackTargetsAreReturned(
            int testCaseIndex,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
            )
        {
            // Arrange

            context.ApprenticeFeedbackTargets.AddRange((List<Domain.Entities.ApprenticeFeedbackTarget>)FeedbackTargetTestData[testCaseIndex].Arguments[0]);
            context.SaveChanges();

            var query = new GetApprenticeFeedbackTargetsForUpdateQuery() { BatchSize = (int)FeedbackTargetTestData[testCaseIndex].Arguments[1] };
            var handler = GetHandler(context);

            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            var expectedResult = (IEnumerable<Domain.Entities.ApprenticeFeedbackTarget>)FeedbackTargetTestData[testCaseIndex].ExpectedResult;
            
            result.ApprenticeFeedbackTargets.Select(aft => aft.ApprenticeId).Should().BeEquivalentTo(expectedResult.Select(aft => aft.ApprenticeId));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public async Task Then_TheNumberOf_FeedbackTargetsReturned_EqualsTheSpecifiedBatchSize(int batchSize)
        {
            // Arrange
            var context = ApprenticeFeedbackDataContextBuilder.GetApprenticeFeedbackDataContext();

            // Note that .NET 6 will NOT try to add these items in the order specified.
            context.ApprenticeFeedbackTargets.AddRange(new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_EligibleToGiveFeedback(Guid.NewGuid(), 5),
                    AFT_EligibleToGiveFeedback(Guid.NewGuid(), 6),
                    AFT_EligibleToGiveFeedback(Guid.NewGuid(), 7),
                    AFT_EligibleToGiveFeedback(Guid.NewGuid(), 8),
                    AFT_EligibleToGiveFeedback(Guid.NewGuid(), 9),
                });
            context.SaveChanges();

            var query = new GetApprenticeFeedbackTargetsForUpdateQuery() { BatchSize = batchSize };

            var handler = new GetApprenticeFeedbackTargetsForUpdateQueryHandler(context,
                new ApplicationSettings()
                {
                    RecentDenyPeriodDays = 7,
                    EligibilityCalculationThrottleDays = 7
                },
                new UtcTimeProvider());

            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert

            var expectedApprentices = context.ApprenticeFeedbackTargets.Take(batchSize)
                                                                     .Select(x => x.ApprenticeId);

            var receivedApprentices = result.ApprenticeFeedbackTargets.Select(aft => aft.ApprenticeId);

            receivedApprentices.Should().BeEquivalentTo(expectedApprentices);
        }

        private static GetApprenticeFeedbackTargetsForUpdateQueryHandler GetHandler(ApprenticeFeedbackDataContext context)
        {
            return new GetApprenticeFeedbackTargetsForUpdateQueryHandler(context,
                new ApplicationSettings()
                {
                    RecentDenyPeriodDays = 7,
                    EligibilityCalculationThrottleDays = 7
                },
                new UtcTimeProvider());
        }
    }
}
