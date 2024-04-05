using System;
using AutoMapper;
using SoftitoFlix.Models;
using SoftitoFlix.Models.Dtos;

namespace SoftitoFlix.AutoMapper
{
	public class AutoMapperConfig : Profile
	{
		public AutoMapperConfig()
		{
			CreateMap<Category, CategoryDto>().ReverseMap();
			CreateMap<Director, DirectorDto>().ReverseMap();
			CreateMap<Restriction, RestrictionDto>().ReverseMap();
			CreateMap<Media, MediaDto>().ReverseMap();
			CreateMap<Episode, EpisodeDto>().ReverseMap();
			CreateMap<ApplicationUser, UserDto>().ReverseMap().ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => DateTime.Now)).ForMember(dest => dest.Passive, opt => opt.MapFrom(src => false));
		}
	}
}

