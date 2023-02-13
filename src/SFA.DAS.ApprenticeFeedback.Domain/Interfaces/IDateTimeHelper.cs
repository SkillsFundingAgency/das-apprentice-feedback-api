using System;

namespace SFA.DAS.ApprenticeFeedback.Domain.Interfaces
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
    }

    public class UtcTimeProvider : IDateTimeHelper
    {
        public DateTime Now => DateTime.UtcNow;
    }

    public class SpecifiedTimeProvider : IDateTimeHelper
    {
        public DateTime Now { get; set; }

        public SpecifiedTimeProvider(DateTime time)
        {
            Now = time;
        }

        public void Advance(TimeSpan timeSpan)
        {
            Now = Now.Add(timeSpan);
        }
    }
}
