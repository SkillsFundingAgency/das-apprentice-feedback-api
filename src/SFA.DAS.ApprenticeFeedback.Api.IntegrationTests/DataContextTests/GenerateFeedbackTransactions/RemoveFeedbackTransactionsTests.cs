using NUnit.Framework;
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

        [Test]
        public async Task GenerateFeedbackTransaction_MultipleApprenticeships_RemovesOldSuperceededFeedbackTransactions()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeId = Guid.NewGuid();

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -2, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -2, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -2, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -2, 6);

            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1002)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate)
                .WithExistingFeedbackTransactions(apprenticeshipOne)
                .WithExistingFeedbackTransactions(apprenticeshipTwo))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await results.VerifyExpectedTemplates(apprenticeshipOne);
                await results.VerifyExpectedTemplates(apprenticeshipTwo);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_MultipleApprenticeships_RemovesNewSuperceededFeedbackTransactions()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeId = Guid.NewGuid();

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);
            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1002)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate)
                .WithExistingFeedbackTransactions(apprenticeshipOne)
                .WithExistingFeedbackTransactions(apprenticeshipTwo))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await results.VerifyExpectedTemplates(apprenticeshipOne);
                await results.VerifyExpectedTemplates(apprenticeshipTwo);

                results.VerifyCount(apprenticeshipTwo.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ExpectedTemplates.Count);
            }
        }

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
