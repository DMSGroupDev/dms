using System.Threading.Tasks;

namespace dms_backend_api.Infrastracture.Jobs
{
    public interface IRemoveUncompletedRegistrationJob
    {
        Task ExecuteAsync();
    }
}
