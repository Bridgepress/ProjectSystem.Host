using AutoMapper;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystem.Api.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegistrationUserRequest, User>();
        }
    }
}
