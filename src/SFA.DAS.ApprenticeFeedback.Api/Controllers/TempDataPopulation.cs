using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    public class TempDataPopulation
    {
        public static GetFeedbackResponse CreateFeedbackResponse(long ukprn)
        {
            var random = new Random();
            return new GetFeedbackResponse
            {
                Ukprn = ukprn,
                ProviderAttribute = new List<ApprenticeFeedbackAttribute>
                    {
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Communication",Name = "Communication 1"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Communication",Name = "Communication 2"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Organisation", Name = "Organisation 2"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Organisation", Name = "Organisation 1"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Support", Name = "Support 2"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Support", Name = "Support 1"},
                        new ApprenticeFeedbackAttribute{ Agree = random.Next(1,20), Disagree = random.Next(1,20), Category = "Communication", Name = "Communication 3"},
                    },
                ProviderRating = new List<ApprenticeFeedbackRating>
                    {
                        new ApprenticeFeedbackRating{ Rating = "Excellent", Count = random.Next(1,20)},
                        new ApprenticeFeedbackRating{ Rating = "Good", Count = random.Next(1,20)},
                        new ApprenticeFeedbackRating{ Rating = "Poor", Count = random.Next(1,20)},
                        new ApprenticeFeedbackRating{ Rating = "Very Poor", Count = random.Next(1,20)},
                    }
            };
        }
    }

    public class FetchApprenticeFeedbackRequest
    {
        public IEnumerable<long> Ukprns { get; set; }
    }

    public class GetFeedbackResponse
    {
        public long Ukprn { get; set; }
        public IEnumerable<ApprenticeFeedbackRating> ProviderRating { get; set; }
        public IEnumerable<ApprenticeFeedbackAttribute> ProviderAttribute { get; set; }
    }

    public class ApprenticeFeedbackAttribute
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Agree { get; set; }
        public int Disagree { get; set; }
    }

    public class ApprenticeFeedbackRating
    {
        public string Rating { get; set; }
        public int Count { get; set; }

    }
}

