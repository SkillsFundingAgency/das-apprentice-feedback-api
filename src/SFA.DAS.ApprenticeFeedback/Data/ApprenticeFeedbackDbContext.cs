using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Data.Models;

namespace SFA.DAS.ApprenticeFeedback.Data
{
    public class ApprenticeFeedbackDbContext : DbContext
    {
        public ApprenticeFeedbackDbContext(DbContextOptions<ApprenticeFeedbackDbContext> options)
        {

        }

        public virtual DbSet<Models.ApprenticeFeedback> ApprenticeFeedbackRecords { get; set; }
        public virtual DbSet<Standard> Standards { get; set; }
    }
}
