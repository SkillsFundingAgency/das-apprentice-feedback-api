using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate;
using SFA.DAS.ApprenticeFeedback.Data;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
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

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_NotStarted(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
                StartDate = null,
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_Completed(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Complete
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_Withdrawn(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Withdrawn
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_EligibilityCalculatedRecently(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-2),
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_NoFeedback(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                Status = (int)Domain.Models.Enums.FeedbackTargetStatus.Active,
                EligibilityCalculationDate = DateTime.UtcNow.AddDays(-100),
            };
        }

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_FeedbackGivenRecently(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
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

        private static Domain.Entities.ApprenticeFeedbackTarget AFT_EligibleToGiveFeedback(string providerName)
        {
            return new Domain.Entities.ApprenticeFeedbackTarget()
            {
                ProviderName = providerName,
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
            public class HasStarted
            {
                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipNotStarted_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper
                    )
                {
                    context.Add(AFT_NotStarted("Provider Name"));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .HasStarted(dateTimeHelper)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStarted_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper
                    )
                {
                    context.Add(AFT_Completed("Provider Name"));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .HasStarted(dateTimeHelper)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class StatusNotCompletedOrPermanentlyWithdrawn
            {            
                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStatusCompleted_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
                    )
                {
                    context.Add(AFT_Completed("Provider Name"));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .StatusNotCompletedOrPermanentlyWithdrawn()
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStatusWithdrawn_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
                    )
                {
                    context.Add(AFT_Withdrawn("Provider Name"));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .StatusNotCompletedOrPermanentlyWithdrawn()
                                    .ToList();

                    apprenticeFeedbackTargets.Should().BeEmpty();
                }

                [Test]
                [AutoMoqData]
                public void When_ApprenticeshipStatusNotCompletedOrWithdrawn_Then_ReturnApprenticeship(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
                    )
                {
                    context.Add(AFT_EligibilityCalculatedRecently("Provider Name"));
                    context.SaveChanges();

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .StatusNotCompletedOrPermanentlyWithdrawn()
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class FeedbackEligibilityNotCalulcatedRecently
            {
                [Test]
                [AutoMoqData]
                public void When_EligibilityCalculatedRecently_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_EligibilityCalculatedRecently("Provider Name"));
                    context.SaveChanges();

                    appSettings.EligibilityCalculationThrottleDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .FeedbackEligibilityNotCalulcatedRecently(dateTimeHelper, appSettings)
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
                    context.Add(AFT_FeedbackGivenRecently("Provider Name"));
                    context.SaveChanges();

                    appSettings.EligibilityCalculationThrottleDays = 7;

                    var apprenticeFeedbackTargets = ((IApprenticeFeedbackTargetContext)context)
                                    .Entities
                                        .FeedbackEligibilityNotCalulcatedRecently(dateTimeHelper, appSettings)
                                    .ToList();

                    apprenticeFeedbackTargets.Should().HaveCount(1);
                }
            }

            public class NotGivenFeedbackRecently
            {
                [Test]
                [AutoMoqData]
                public void When_NoFeedback_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_NoFeedback("Provider Name"));
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
                public void When_GivenFeedbackRecently_Then_ReturnEmpty(
                    [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context,
                    UtcTimeProvider dateTimeHelper,
                    ApplicationSettings appSettings
                    )
                {
                    context.Add(AFT_FeedbackGivenRecently("Provider Name"));
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
                    context.Add(AFT_EligibleToGiveFeedback("Provider Name"));
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

        static TestCaseData[] FeedbackTargetTestData =
        {
            // Test case 0 - Apprenticeship has not started
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_NotStarted("Test Provider 0"),
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 1 - Apprenticeship has completed
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_Completed("Test Provider 1"),
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 2 - Eligilibity was calculated recently
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_EligibilityCalculatedRecently("Test Provider 2"),
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 3 - Feedback given recently
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_FeedbackGivenRecently("Test Provider 3"),
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
            ),

            // Test case 4 - Eligible for feedback
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_EligibleToGiveFeedback("Test Provider 4"),
                },
                1  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
                {
                    AFT_EligibleToGiveFeedback("Test Provider 4")
                }
            ),

            // Test case 5 - Eligible for feedback - test batch size
            new TestCaseData(
                new List<Domain.Entities.ApprenticeFeedbackTarget>
                {
                    AFT_EligibleToGiveFeedback("Test Provider 5"),
                    AFT_EligibleToGiveFeedback("Test Provider 6"),
                    AFT_EligibleToGiveFeedback("Test Provider 7"),
                    AFT_EligibleToGiveFeedback("Test Provider 8"),
                    AFT_EligibleToGiveFeedback("Test Provider 9"),
                },
                3  // batch size
            ).Returns(
                new List<Domain.Entities.ApprenticeFeedbackTarget>()
                {
                    AFT_EligibleToGiveFeedback("Test Provider 5"),
                    AFT_EligibleToGiveFeedback("Test Provider 6"),
                    AFT_EligibleToGiveFeedback("Test Provider 7"),
                }
            )
        };

        #endregion Test Cases

        [Test]
        [AutoMoqInlineAutoData(0)]
        [AutoMoqInlineAutoData(1)]
        [AutoMoqInlineAutoData(2)]
        [AutoMoqInlineAutoData(3)]
        [AutoMoqInlineAutoData(4)]
        [AutoMoqInlineAutoData(5)]
        public async Task ThenFeedbackTargetsAreReturned(
            int testCaseIndex,
            [Frozen(Matching.ImplementedInterfaces)] ApprenticeFeedbackDataContext context
            )
        {
            // Arrange

            context.ApprenticeFeedbackTargets.AddRange((List<Domain.Entities.ApprenticeFeedbackTarget>)FeedbackTargetTestData[testCaseIndex].Arguments[0]);
            context.SaveChanges();

            var query = new GetApprenticeFeedbackTargetsForUpdateQuery() { BatchSize = (int)FeedbackTargetTestData[testCaseIndex].Arguments[1] };

            var handler = new GetApprenticeFeedbackTargetsForUpdateQueryHandler(context, new ApplicationSettings() { RecentDenyPeriodDays = 7 }, new UtcTimeProvider());
            
            // Act

            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            var expectedResult = (IEnumerable<Domain.Entities.ApprenticeFeedbackTarget>)FeedbackTargetTestData[testCaseIndex].ExpectedResult;
            result.ApprenticeFeedbackTargets.Select(aft => aft.ProviderName).Should().BeEquivalentTo(expectedResult.Select(aft => aft.ProviderName));
        }
    }
}
