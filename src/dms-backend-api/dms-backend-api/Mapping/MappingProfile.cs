
using AutoMapper;
using dms_backend_api.Domain.Identity;
using dms_backend_api.ExternalModel.Authenticate;
using dms_backend_api.ExternalModel.Identity;

namespace dms_backend_api.Mapping
{

    public partial class MappingProfile : Profile
    {
        //https://docs.automapper.org/en/stable/index.html

        public MappingProfile()
        {
            CreateMap<RegisterUserModelDTO, ApplicationUser>();
            CreateMap<AddRoleModelDTO, ApplicationRole>();
        }
    }
}
