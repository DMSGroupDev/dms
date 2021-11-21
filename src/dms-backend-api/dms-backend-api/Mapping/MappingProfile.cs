
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
            #region Users

            CreateMap<RegisterUserModelDTO, ApplicationUser>();
            CreateMap<UpdateUserModelDTO, ApplicationUser>();
            CreateMap<AddUserModelDTO, ApplicationUser>();

            #endregion

            #region Roles

            CreateMap<AddRoleModelDTO, ApplicationRole>();
            CreateMap<UpdateRoleModelDTO, ApplicationRole>();

            #endregion

        }
    }
}
