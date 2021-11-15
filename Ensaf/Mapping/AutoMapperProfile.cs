using AutoMapper;
using Ensaf.Domain.Models;
using Ensaf.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensaf.Mapping
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponse>();

            CreateMap<User, AuthenticateResponse>().ForMember(x => x.Roles,o =>o.MapFrom(src => src.UserRoles.Select(r => r.Role.Name)));

            CreateMap<RegisterRequest, User>();

            CreateMap<CreateUserRequest, User>();

            CreateMap<UpdateUserRequest, User>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Roles" && src.Roles == null) return false;

                        return true;
                    }
                ));
        }
    }
}
