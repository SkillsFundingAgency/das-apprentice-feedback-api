using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Handlers;
using SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.Services;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class AddFeedbackTransactionTests : TestBase
    {
        [TestCaseSource(nameof(SingleApprenticeshipTestCases))]
        public async Task GenerateFeedbackTransactions_SingleApprenticeships(EngagementEmailTestData apprenticeship)
        {
            using (var fixture = new AddFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(apprenticeship.CurrentDate);

                await VerifyEngagementEmails(apprenticeship, results);

                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ExpectedTemplates.Count);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_MultipleApprenticeships()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeshipOne = new EngagementEmailTestData(currentDate, 1, 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6);

            var apprenticeshipTwo = new EngagementEmailTestData(currentDate, 1, 9, Guid.NewGuid(), 1002)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9);

            var apprenticeshipThree = new EngagementEmailTestData(currentDate, -3, 9, Guid.NewGuid(), 1003)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppMonthThree")
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9);

            using (var fixture = new AddFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate)
                .WithApprenticeFeedbackTarget(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ApprenticeshipId, apprenticeshipThree.StartDate, apprenticeshipThree.EndDate))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await VerifyEngagementEmails(apprenticeshipOne, results);
                await VerifyEngagementEmails(apprenticeshipTwo, results);
                await VerifyEngagementEmails(apprenticeshipThree, results);

                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ExpectedTemplates.Count + apprenticeshipTwo.ExpectedTemplates.Count + apprenticeshipThree.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ExpectedTemplates.Count);
            }
        }

        public static IEnumerable<TestCaseData> SingleApprenticeshipTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 3, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 3))
                    .SetName("StartOneMonthsInFutureAndRunForThreeMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6))
                    .SetName("StartOneMonthsInFutureAndRunForSixMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartOneMonthsInFutureAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 18, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthEighteen", 18))
                    .SetName("StartOneMonthsInFutureAndRunForEigheenMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 24, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthEighteen", 18))
                    .SetName("StartOneMonthsInFutureAndRunForTwentyFourMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 36, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 24)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 36))
                    .SetName("StartOneMonthsInFutureAndRunForThirtySixMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 48, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 24)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 36)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 48))
                    .SetName("StartOneMonthsInFutureAndRunForFortyEightMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 60, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 24)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 36)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 48)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 60))
                    .SetName("StartOneMonthsInFutureAndRunForSixtyMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), 1, 72, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtMonthsAfterStart("AppStart", 0)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 12)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 24)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 36)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 48)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 60)
                        .WithExpectedTemplateAtMonthsAfterStart("AppAnnual", 72))
                    .SetName("StartOneMonthsInFutureAndRunForSeventyTwoMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -1, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppStart")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartOneMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -2, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartTwoMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -3, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthThree", 3)
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartThreeMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -4, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartFourMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -5, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartFiveMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -6, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtMonthsBeforeEnd("AppPreEpa", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthSix", 6)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartSixMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -7, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartSevenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -8, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartEightMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -9, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthNine", 9)
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartNineMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -10, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartTenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -11, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtMonthsAfterStart("AppMonthTwelve", 12))
                    .SetName("StartElevenMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -12, 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateAtCurrentDate("AppWelcome")
                        .WithExpectedTemplateAtCurrentDate("AppPreEpa")
                        .WithExpectedTemplateAtCurrentDate("AppMonthTwelve"))
                    .SetName("StartTwelveMonthsInPastAndRunForTwelveMonths");

                yield return new TestCaseData(
                    new EngagementEmailTestData(new DateTime(2000, 01, 01), -13, 12, Guid.NewGuid(), 1001))
                    .SetName("StartThirteenMonthsInPastAndRunForTwelveMonths");
            }
        }

        public class EngagementEmailTestData
        {
            public Guid ApprenticeFeedbackTargetId { get; set; }
            public long ApprenticeshipId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CurrentDate { get; set; }
            public List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime SendAfterDate)> ExpectedTemplates { get; } 
                = new List<(Guid ApprenticeFeedbackTargetId, string TemplateName, DateTime SendAfterDate)>();

            public EngagementEmailTestData(DateTime currentDate, int startMonths, int lengthMonths, Guid apprenticeFeedbackTargetId, long apprenticeshipId)
            {
                CurrentDate = currentDate;
                StartDate = currentDate.AddMonths(startMonths);
                EndDate = StartDate.AddMonths(lengthMonths);
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId;
                ApprenticeshipId = apprenticeshipId;
            }

            public EngagementEmailTestData WithExpectedTemplateAtMonthsAfterStart(string templateName, int months)
            {
                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, StartDate.AddMonths(months)));
                return this;
            }

            public EngagementEmailTestData WithExpectedTemplateAtCurrentDate(string templateName)
            {
                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, CurrentDate));
                return this;
            }

            public EngagementEmailTestData WithExpectedTemplateAtMonthsBeforeEnd(string templateName, int months)
            {
                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, EndDate.AddMonths(-months)));
                return this;
            }

            public EngagementEmailTestData WithExpectedTemplate(string templateName, DateTime sendAfterDate)
            {
                ExpectedTemplates.Add((ApprenticeFeedbackTargetId, templateName, sendAfterDate));
                return this;
            }
        }

        private static async Task VerifyEngagementEmails(EngagementEmailTestData apprenticeship, AddFeedbackTransactionTestsFixture results)
        {
            foreach (var expectedTemplate in apprenticeship.ExpectedTemplates)
            {
                await results.VerifyFeedbackTransactionExists(
                FeedbackTransactionHandler.Create(
                    null,
                    apprenticeship.ApprenticeFeedbackTargetId,
                    null,
                    null,
                    null,
                    null,
                    expectedTemplate.SendAfterDate,
                    null,
                    expectedTemplate.TemplateName,
                    false));
            }
        }

        private class AddFeedbackTransactionTestsFixture : FixtureBase<AddFeedbackTransactionTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly IFeedbackTransactionContext _feedbackTransactionContext;

            public int _feedbackTransactionSentDateAgeDays = 90;
            public int _count;


            public AddFeedbackTransactionTestsFixture()
            {
                _feedbackTransactionContext = _databaseService.TestContext;
            }

            public async Task<AddFeedbackTransactionTestsFixture> GenerateFeedbackTransactions(DateTime dateTimeUtc)
            {
                var result = await _feedbackTransactionContext.GenerateFeedbackTransactionsAsync(_feedbackTransactionSentDateAgeDays, dateTimeUtc);
                _count = result.Count;
                return this;
            }

            public AddFeedbackTransactionTestsFixture VerifyCount(int count)
            {
                _count.Should().Be(count);
                return this;
            }
        }
    }
}
