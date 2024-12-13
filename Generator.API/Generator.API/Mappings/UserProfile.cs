﻿using AutoMapper;
using Generator.API.DTO;

namespace Generator.API.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Domain.Users, UserDto>();
        CreateMap<UserDto, Domain.Users>();
    }
}
