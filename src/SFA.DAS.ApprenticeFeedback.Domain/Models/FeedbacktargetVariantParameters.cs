using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Domain.Models
{
    public  class FeedbackTargetVariantParameters
    {
        public bool ClearStaging { get; set; }
        public bool MergeStaging { get; set; }
        public List<FeedbackTargetVariant> FeedbackTargetVariants { get; set; }
    }
}
