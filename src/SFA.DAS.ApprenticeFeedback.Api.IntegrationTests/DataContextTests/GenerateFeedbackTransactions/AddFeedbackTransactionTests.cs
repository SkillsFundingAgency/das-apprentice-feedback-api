using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Api.IntegrationTests.DataContextTests.GenerateFeedbackTransactions
{
    public class AddFeedbackTransactionTests : GenerateFeedbackTransactionTestsBase
    {
        [Test, TestCaseSource(nameof(SingleApprenticeshipTestCases))]
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

        [Test, TestCaseSource(nameof(SingleFoundationApprenticeshipTestCases))]
        public async Task GenerateFeedbackTransaction_SingleApprenticeship_CreateFeedbackTransactionForFoundation(FeedbackTransactionTestData apprenticeship)
        {
            using (var fixture = new GenerateFeedbackTransactionTestsFixture().WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA0"))
            {
                var results = await fixture.GenerateFeedbackTransactions(apprenticeship.CurrentDate);
                await fixture.VerifyExpectedTemplates(apprenticeship);
                await fixture.VerifyFeedbackTransactionNotExists(new Models.FeedbackTransactionModel()
                {
                    Id = null,
                    ApprenticeFeedbackTargetId = apprenticeship.ApprenticeFeedbackTargetId,
                    EmailAddress = null,
                    FirstName = null,
                    TemplateId = null,
                    CreatedOn = apprenticeship.CurrentDate,
                    SendAfter = apprenticeship.CurrentDate,
                    SentDate = apprenticeship.CurrentDate,
                    TemplateName = "AppWelcome",
                    IsSuppressed = false
                });
                await fixture.VerifyFeedbackTransactionNotExists(new Models.FeedbackTransactionModel()
                {
                    Id = null,
                    ApprenticeFeedbackTargetId = apprenticeship.ApprenticeFeedbackTargetId,
                    EmailAddress = null,
                    FirstName = null,
                    TemplateId = null,
                    CreatedOn = apprenticeship.CurrentDate,
                    SendAfter = apprenticeship.CurrentDate,
                    SentDate = apprenticeship.CurrentDate,
                    TemplateName = "AppStart",
                    IsSuppressed = false
                });
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
        public async Task GenerateFeedbackTransaction_MultipleFoundationApprenticeships_CreatesFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeshipOne = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6);


            var apprenticeshipTwo = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1002)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 1)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);

            var apprenticeshipThree = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-3), 9, Guid.NewGuid(), 1003)
                       .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                       .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                       .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                       .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                       .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);

            var apprenticeshipFoundation = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1004)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 1)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);


            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ApprenticeshipId, apprenticeshipOne.StartDate, apprenticeshipOne.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA0")
                .WithApprenticeFeedbackTarget(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ApprenticeshipId, apprenticeshipTwo.StartDate, apprenticeshipTwo.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA0")
                .WithApprenticeFeedbackTarget(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ApprenticeshipId, apprenticeshipThree.StartDate, apprenticeshipThree.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA0")
                .WithApprenticeFeedbackTarget(apprenticeshipFoundation.ApprenticeFeedbackTargetId, apprenticeshipFoundation.ApprenticeshipId, apprenticeshipFoundation.StartDate, apprenticeshipFoundation.EndDate, FeedbackTargetStatus.Unknown, false, false, null, "FA0"))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                await fixture.VerifyExpectedTemplates(apprenticeshipOne);
                await fixture.VerifyExpectedTemplates(apprenticeshipTwo);
                await fixture.VerifyExpectedTemplates(apprenticeshipThree);
                await fixture.VerifyExpectedTemplates(apprenticeshipFoundation);

                results.VerifyCount(apprenticeshipOne.ExpectedTemplates.Count + apprenticeshipTwo.ExpectedTemplates.Count + apprenticeshipThree.ExpectedTemplates.Count + apprenticeshipFoundation.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipOne.ApprenticeFeedbackTargetId, apprenticeshipOne.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipTwo.ApprenticeFeedbackTargetId, apprenticeshipTwo.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipThree.ApprenticeFeedbackTargetId, apprenticeshipThree.ExpectedTemplates.Count);
                await results.VerifyFeedbackTransactionRowCount(apprenticeshipFoundation.ApprenticeFeedbackTargetId, apprenticeshipFoundation.ExpectedTemplates.Count);
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
        public async Task GenerateFeedbackTransaction_SingleFoundationApprenticeship_WithExistingFeedbackTransactions_DoesNotRecreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 0, 0)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 0, 3)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppPreEpa", 0, 0, 6)
                        .WithExistingTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 0, 6);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId, apprenticeship.ApprenticeshipId, apprenticeship.StartDate, apprenticeship.EndDate,FeedbackTargetStatus.Unknown,  false, false, null, "FA")
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


        [Test]
        public async Task GenerateFeedbackTransaction_SingleFoundationApprenticeship_WhenWithdrawn_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    FeedbackTargetStatus.Unknown,
                    true,false, null, "FA0"))
            {
                var results = await fixture.GenerateFeedbackTransactions(currentDate);

                results.VerifyCount(0);
                await results.VerifyFeedbackTransactionRowCount(apprenticeship.ApprenticeFeedbackTargetId, 0);
            }
        }

        [Test]
        public async Task GenerateFeedbackTransaction_SingleFoundationApprenticeship_WithTransfer_DoesNotCreateFeedbackTransaction()
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
                    currentDate,"FA"))
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

        [TestCase(FeedbackTargetStatus.Unknown)]
        [TestCase(FeedbackTargetStatus.NotYetActive)]
        [TestCase(FeedbackTargetStatus.Active)]
        public async Task GenerateFeedbackTransaction_SingleFoundationApprenticeship_WithNotComplete_DoesCreateFeedbackTransaction(FeedbackTargetStatus feedbackTargetStatus)
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1001)
                .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 1)
                .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEpa", 0, 6)
                .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    feedbackTargetStatus,false,false, null, "FA"))
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

        [Test]
        public async Task GenerateFeedbackTransaction_SingleFoundationApprenticeship_WithComplete_DoesNotCreateFeedbackTransaction()
        {
            var currentDate = new DateTime(2000, 01, 01);

            var apprenticeship = new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001);

            using (var fixture = new GenerateFeedbackTransactionTestsFixture()
                .WithApprenticeFeedbackTarget(apprenticeship.ApprenticeFeedbackTargetId,
                    apprenticeship.ApprenticeshipId,
                    apprenticeship.StartDate,
                    apprenticeship.EndDate,
                    FeedbackTargetStatus.Complete, false, false, null, "FA"))
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
                    .SetArgDisplayNames("OneMonthFuture", "RunForThreeMonths");
                
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6))
                    .SetArgDisplayNames("OneMonthFuture", "RunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("OneMonthFuture", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 18, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthEighteen", 0, 18))
                    .SetArgDisplayNames("OneMonthFuture", "RunForEigheenMonths");
                    
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(2000, 02, 01), 24, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthEighteen", 0, 18))
                    .SetArgDisplayNames("OneMonthFuture", "RunForTwentyFourMonths");
                    
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
                    .SetArgDisplayNames("OneMonthFuture", "RunForThirtySixMonths");

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
                    .SetArgDisplayNames("OneMonthFuture", "RunForFortyEightMonths");
                    
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
                    .SetArgDisplayNames("OneMonthFuture", "RunForSixtyMonths");

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
                    .SetArgDisplayNames("OneMonthFuture", "RunForSeventyTwoMonths");
                    
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 12, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppStart", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("OneMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 11, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("TwoMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 10, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("ThreeMonthsPast", "RunForTwelveMonths");
                    
                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 09, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("FourMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 08, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("FiveMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 07, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("AppPreEpa", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("SixMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 06, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("SevenMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 05, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("EightMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 04, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthNine", 0, 9)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("NineMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 03, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("TenMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 02, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("AppMonthTwelve", 0, 12))
                    .SetArgDisplayNames("ElevenMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1999, 01, 01), 12, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("AppWelcome", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppPreEpa", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("AppMonthTwelve", 0))
                    .SetArgDisplayNames("TwelveMonthsPast", "RunForTwelveMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(new DateTime(2000, 01, 01), new DateTime(1998, 12, 01), 12, Guid.NewGuid(), 1001))
                    .SetArgDisplayNames("ThirteenMonthsPast", "RunForTwelveMonths");
            }
        }
        public static IEnumerable<TestCaseData> SingleFoundationApprenticeshipTestCases
        {
            get
            {
                var currentDate = new DateTime(2000, 01, 01);

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate, 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 1)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("StartToday", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 1)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("OneMonthFuture", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 6, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6))
                    .SetArgDisplayNames("OneMonthFuture", "RunForSixMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(1), 3, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationStart", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppPreEPA", 0, 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3))
                    .SetArgDisplayNames("OneMonthFuture", "RunForThreeMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-1), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("OneMonthPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-2), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterMonthsBeforeEnd("FoundationAppPreEPA", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("TwoMonthsPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-3), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthThree", 0, 3)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("ThreeMonthsPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-4), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("FourMonthsPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-5), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("FiveMonthsPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-6), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSix", 0, 6)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("SixMonthsPast", "RunForNineMonths");

                yield return new TestCaseData(
                    new FeedbackTransactionTestData(currentDate, currentDate.AddMonths(-7), 9, Guid.NewGuid(), 1001)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationStart", 0)
                        .WithExpectedTemplateSendAfterCurrentDate("FoundationAppPreEPA", 0)
                        .WithExpectedTemplateSendAfterMonthsAfterStart("FoundationAppMonthSeven", 0, 7))
                    .SetArgDisplayNames("SevenMonthsPast", "RunForNineMonths");
            }
        }
    }
}
