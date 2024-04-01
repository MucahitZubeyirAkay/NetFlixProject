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
		}
	}
}

