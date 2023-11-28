using FluentAssertions;
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
    public class RemoveFeedbackTransactionsTests : GenerateFeedbackTransactionTestsBase
    {
        [TestCaseSource(nameof(SingleApprenticeshipTestCases))]
        public async Task GenerateFeedbackTransactions_SingleApprenticeship_WhenWithdrawn_RemovesFeedbackTransactions(FeedbackTransactionTestData apprenticeship)
        {
            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    apprenticeship.FeedbackTargetStatus,
                    apprenticeship.Withdrawn)
                .WithExistingFeedbackTransactions(apprenticeship))
            {
                var results = await fixture.GenerateFeedbackTransactions(apprenticeship.CurrentDate);

                // the templates should have been removed
                await fixture.VerifyExpectedTemplates(apprenticeship);

                fixture.VerifyCount(0);
                await fixture.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        /*[Test]
        public async Task GenerateFeedbackTransaction_MultipleApprenticeships_CreatesFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 6, null);

            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, 1, 9, Guid.NewGuid(), 1002)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", 9, null);

            var apprenticeshipThree = new FeedbackTransactionTestData(currentDate, -3, 9, Guid.NewGuid(), 1003)
                        .WithExistingTemplateAtCurrentDate("AppWelcome", null)
                        .WithExistingTemplateAtCurrentDate("AppMonthThree")
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", 9, null);

            using (var fixture = new RemoveFeedbackTransactionFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ApprenticeshipId, apprenticeshipThree.StartDate, apprenticeshipThree.EndDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await VerifyEngagementEmails(apprenticeshipOne, results);
                await VerifyEngagementEmails(apprenticeshipTwo, results);
                await VerifyEngagementEmails(apprenticeshipThree, results);

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

            var apprenticeship = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 6, null);

            using (var fixture = new RemoveFeedbackTransactionFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate)
                .WithFeedbackTransactions(apprenticeship))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate.AddDays(1));

                await VerifyEngagementEmails(apprenticeship, results);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WhenWithdrawn_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001);

            using (var fixture = new RemoveFeedbackTransactionFixture()
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

            var apprenticeship = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001);

            using (var fixture = new RemoveFeedbackTransactionFixture()
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
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithNotComplete_DoesNotCreateFeedbackTransaction(FeedbackTargetStatus feedbackTargetStatus)
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001)
                .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, null)
                .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 3, null)
                .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 6, null)
                .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 6, null);


            using (var fixture = new RemoveFeedbackTransactionFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    feedbackTargetStatus))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await VerifyEngagementEmails(apprenticeship, results);

                results.VerifyCount(apprenticeship.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_WithComplete_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, 1, 6, Guid.NewGuid(), 1001);

            using (var fixture = new RemoveFeedbackTransactionFixture()
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
        }*/

        public static IEnumerable<TestCaseData> SingleApprenticeshipTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 3, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 3, null))
                    .SetName("RemoveWithdrawnStartOneMonthsInFutureAndRunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 01, 01), 3, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 3, null))
                    .SetName("RemoveCompleteStartOneMonthsInFutureAndRunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6))
                    .SetName("RemoveWithdrawnStartOneMonthsInPastAndRunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6))
                    .SetName("RemoveCompleteStartOneMonthsInPastAndRunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 10, 01), 12, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3))
                    .SetName("RemoveWithdrawnStartThreeMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 10, 01), 12, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3))
                    .SetName("RemoveCompleteStartThreeMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 07, 01), 18, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthEighteen", -1, 18, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6))
                    .SetName("RemoveWithdrawnStartSixMonthsInPastAndRunForEigheenMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 07, 01), 18, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthEighteen", -1, 18, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6))
                    .SetName("RemoveCompleteStartSixMonthsInPastAndRunForEigheenMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 09, 01), new DateTime(1999, 07, 01), 12, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExistingTemplateSendAfterSpecifiedDate("AppPreEpa", -1, new DateTime(2000, 07, 01), 0) // assume the apprenticeship was 18 months when this was created
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, 9)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, 12)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthEighteen", -1, 18, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", -1, 9, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", -1, 12, 12))
                    .SetName("RemovePastEndDateStartFourteenMonthsInPastAndRunForEigheenMonthsReducedToTwelve");
            }
        }
    }
}
