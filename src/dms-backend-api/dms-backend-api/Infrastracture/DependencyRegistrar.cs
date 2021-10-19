
using AutoMapper.Data;
using dms_backend_api.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace dms_backend_api.Infrastracture
{

    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services)
        {
            /*Mapping*/
            services.AddAutoMapper(mc =>
            {
                mc.AddDataReaderMapping();
                mc.AddProfile(new MappingProfile());
            });


            /*Validator*/

            /*Services*/
        }
    }
}
