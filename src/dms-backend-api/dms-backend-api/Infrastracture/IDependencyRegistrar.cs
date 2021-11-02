
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dms_backend_api.Infrastracture
{
    public interface IDependencyRegistrar
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}