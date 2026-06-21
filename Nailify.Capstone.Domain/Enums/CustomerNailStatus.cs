using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum CustomerNailStatus
    {
        Draft,
        PendingReview,
        Assigned,
        Reviewed,
        Quoted,
        Approved,
        Rejected
    }
}
