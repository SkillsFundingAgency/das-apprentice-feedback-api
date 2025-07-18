using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class RemoveFeedbackTransactionsTests : GenerateFeedbackTransactionTestsBase
    {
        [Test, TestCaseSource(nameof(SingleApprenticeshipTestCases))]
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

        [Test, TestCaseSource(nameof(SingleFoundationApprenticeshipTestCases))]
        public async Task GenerateFeedbackTransactions_SingleFoundationApprenticeship_WhenWithdrawn_RemovesFeedbackTransactions(FeedbackTransactionTestData apprenticeship)
        {
            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    apprenticeship.FeedbackTargetStatus,
                    apprenticeship.Withdrawn,false, null, "FA")
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
        public async Task GenerateFeedbackTransaction_MultipleFoundationApprenticeships_RemovesOldSuperceededFeedbackTransactions()
        {

            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeId = Guid.NewGuid();

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -2, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -2, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -2, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", -2, 6);

            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 7, Guid.NewGuid(), 1002)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);

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
        public async Task GenerateFeedbackTransaction_MultipleFoundationApprenticeships_RemovesNewSuperceededFeedbackTransactions()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeId = Guid.NewGuid();
            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);
            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 7, Guid.NewGuid(), 1002)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA")
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA")
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
                    .SetArgDisplayNames("RemoveWithdrawn","OneMonthsInFuture","RunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 01, 01), 3, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 3, null))
                    .SetArgDisplayNames("RemoveComplete", "StartOneMonthsInFuture", "RunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6))
                    .SetArgDisplayNames("RemoveWithdrawn", "StartOneMonthsInPast", "RunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("AppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", -1, 6, 6))
                    .SetArgDisplayNames("RemoveComplete", "StartOneMonthsInPast", "RunForSixMonths");

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
                    .SetArgDisplayNames("RemoveWithdrawn", "StartThreeMonthsInPast", "RunForTwelveMonths");

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
                    .SetArgDisplayNames("RemoveComplete", "StartThreeMonthsInPast", "RunForTwelveMonths");

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
                    .SetArgDisplayNames("RemoveWithdrawn", "StartSixMonthsInPast", "RunForEigheenMonths");

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
                    .SetArgDisplayNames("RemoveComplete", "StartSixMonthsInPast", "RunForEigheenMonths");

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
                    .SetArgDisplayNames("RemovePastEndDate", "StartFourteenMonthsInPast", "RunForEigheenMonthsReducedToTwelve");
            }
        }
        public static IEnumerable<TestCaseData> SingleFoundationApprenticeshipTestCases
        {
            get
            {
                var currentDate = new DateTime(2000, 01, 01);


                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate, 3, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppPreEPA", -1, 6))
                    .SetArgDisplayNames("RemoveWithdrawn", "OneMonthFuture", "RunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 01, 01), 3, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 3, null))
                    .SetArgDisplayNames("RemoveCompleted", "OneMonthFuture", "RunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6))
                    .SetArgDisplayNames("RemoveWithdrawn", "OneMonthsInPast", "RunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 6, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", -1, 6, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6))
                    .SetArgDisplayNames("RemoveCompleted", "OneMonthsInPast", "RunForSixMonths");


                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 7, Guid.NewGuid(), 1001, withdrawn: true)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", -1, 7, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6))
                    .SetArgDisplayNames("RemoveWithdrawn", "OneMonthsInPast", "RunForSevenMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 7, Guid.NewGuid(), 1001, feedbackTargetStatus: FeedbackTargetStatus.Complete)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", -1, 3, null)
                        .WithExistingTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", -1, 6, null)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", -1, 7, null)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppStart", -1, 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", -1, 6, 6))
                    .SetArgDisplayNames("RemoveCompleted", "OneMonthsInPast", "RunForSevenMonths");
            }
        }
    }
}
