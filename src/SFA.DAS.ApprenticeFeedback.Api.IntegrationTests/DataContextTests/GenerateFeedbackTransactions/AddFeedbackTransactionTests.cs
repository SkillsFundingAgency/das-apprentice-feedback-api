using FluentAssertions;
using NLog.LayoutRenderers;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class AddFeedbackTransactionTests : GenerateFeedbackTransactionTestsBase
    {
        [TestCaseSource(nameof(SingleApprenticeshipTestCases))]
        public async Task GenerateFeedbackTransactions_SingleApprenticeship_CreatesFeedbackTransactions(FeedbackTransactionTestData apprenticeship)
        {
            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(apprenticeship.CurrentDate);

                await results.VerifyExpectedTemplates(apprenticeship);

                results.VerifyCount(apprenticeship.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_MultipleApprenticeships_CreatesFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3, null)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6, null);

            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1002)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3, null)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine",0 , 9, null);

            var apprenticeshipThree = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-3), 9, Guid.NewGuid(), 1003)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0, null)
                        .WithExpectedTemplateSendAfterCurrentDate("AppMonthThree", 0, null)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9, null);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ApprenticeshipId, apprenticeshipThree.StartDate, apprenticeshipThree.EndDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await fixture.VerifyExpectedTemplates(apprenticeshipOne);
                await fixture.VerifyExpectedTemplates(apprenticeshipTwo);
                await fixture.VerifyExpectedTemplates(apprenticeshipThree);

                results.VerifyCount(apprenticeshipOne.ExpectedTemplates.Count + apprenticeshipTwo.ExpectedTemplates.Count + apprenticeshipThree.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithExistingFeedbackTransactions_DoesNotRecreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 0, 3)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppPreEpa", 0, 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 0, 6);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate)
                .WithExistingFeedbackTransactions(apprenticeship))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate.AddDays(1));

                await results.VerifyExpectedTemplates(apprenticeship);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExistingTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WhenWithdrawn_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    FeedbackTargetStatus.Unknown,
                    true))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, 0);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithTransfer_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, 
                    apprenticeship.ApprenticeshipId, 
                    apprenticeship.StartDate, 
                    apprenticeship.EndDate,
                    FeedbackTargetStatus.Unknown,
                    false,
                    true,
                    currentDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, 0);
            }
        }

        [TestCase(FeedbackTargetStatus.Unknown)]
        [TestCase(FeedbackTargetStatus.NotYetActive)]
        [TestCase(FeedbackTargetStatus.Active)]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithNotComplete_DoesCreateFeedbackTransaction(FeedbackTargetStatus feedbackTargetStatus)
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6);


            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    feedbackTargetStatus))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await results.VerifyExpectedTemplates(apprenticeship);

                results.VerifyCount(apprenticeship.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithComplete_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    FeedbackTargetStatus.Complete))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, 0);
            }
        }

        public static IEnumerable<TestCaseData> SingleApprenticeshipTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 3, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 3))
                    .SetName("StartOneMonthsInFutureAndRunForThreeMonths");
                
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6))
                    .SetName("StartOneMonthsInFutureAndRunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartOneMonthsInFutureAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 18, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthEighteen", 0, 18))
                    .SetName("StartOneMonthsInFutureAndRunForEigheenMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 24, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthEighteen", 0, 18))
                    .SetName("StartOneMonthsInFutureAndRunForTwentyFourMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 36, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 24)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 36))
                    .SetName("StartOneMonthsInFutureAndRunForThirtySixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 48, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 24)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 36)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 48))
                    .SetName("StartOneMonthsInFutureAndRunForFortyEightMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 60, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 24)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 36)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 48)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 60))
                    .SetName("StartOneMonthsInFutureAndRunForSixtyMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 72, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 24)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 36)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 48)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 60)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppAnnual", 0, 72))
                    .SetName("StartOneMonthsInFutureAndRunForSeventyTwoMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppStart", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartOneMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 11, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartTwoMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 10, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartThreeMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 09, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartFourMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 08, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartFiveMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 07, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartSixMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 06, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartSevenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 05, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartEightMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 04, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartNineMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 03, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartTenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 02, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetName("StartElevenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 01, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppMonthTwelve", 0))
                    .SetName("StartTwelveMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1998, 12, 01), 12, Guid.NewGuid(), 1001))
                    .SetName("StartThirteenMonthsInPastAndRunForTwelveMonths");
            }
        }
    }
}
