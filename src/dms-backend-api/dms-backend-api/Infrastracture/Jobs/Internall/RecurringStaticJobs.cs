using Hangfire;

namespace dms_backend_api.Infrastracture.Jobs.Internall
{
    public class RecurringStaticJobs : IRecurringStaticJobs
    {
        #region Fields 

        private readonly IRecurringJobManager _recurringJobManager;

        #region Jobs
        private readonly IRemoveUncompletedRegistrationJob _removeUncompletedRegistrationJob;
        #endregion

        #endregion

        #region Ctor
        public RecurringStaticJobs(IRecurringJobManager recurringJobManager, IRemoveUncompletedRegistrationJob removeUncompletedRegistrationJob)
        {
            _recurringJobManager = recurringJobManager;
            _removeUncompletedRegistrationJob = removeUncompletedRegistrationJob;
        }
        #endregion

        #region Method
        public void AddStaticRecuringJobs()
        {
            _recurringJobManager.AddOrUpdate("Remove uncompleted registration", () => _removeUncompletedRegistrationJob.ExecuteAsync(), "0 0 * * *");
        }
        #endregion
    }
}
