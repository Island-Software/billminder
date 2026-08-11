using System.Linq;
using AutoMapper;
using Paybills.API.Application.DTOs.Bill;
using Paybills.API.Application.DTOs.BillType;
using Paybills.API.Application.DTOs.Receiving;
using Paybills.API.Application.DTOs.ReceivingType;
using Paybills.API.Application.DTOs.User;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, UserDto>();
            CreateMap<AppUser, UserEditDto>();
            CreateMap<Bill, BillDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => (from user in src.Users
                                               select user.UserName)));
            CreateMap<BillType, BillTypeDto>();
            CreateMap<Receiving, ReceivingDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => (from user in src.Users
                                               select user.UserName)));
            CreateMap<ReceivingType, ReceivingTypeDto>();
            CreateMap<ReceivingTypeDto, ReceivingType>();

            CreateMap<ReceivingRegisterDto, Receiving>()
                .ForMember(dest => dest.ReceivingType,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Users,
                    opt => opt.Ignore());
        }
    }
}