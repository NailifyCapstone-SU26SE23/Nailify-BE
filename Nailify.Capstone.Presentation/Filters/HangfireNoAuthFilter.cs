using Hangfire.Dashboard;

namespace Nailify.Capstone.Presentation.Filters
{
    public class HangfireNoAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Cho phép tất cả các request truy cập vào Hangfire Dashboard không cần Auth
            return true;
        }
    }
}
