using Hangfire.Dashboard;

namespace dms_backend_api.Infrastracture.Middlewares
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public HangfireAuthorizationFilter()
        {
        }
        public bool Authorize(DashboardContext currentContext)
        {
            var httpContext = AspNetCoreDashboardContextExtensions.GetHttpContext(currentContext);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return httpContext.User.Identity.IsAuthenticated;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
