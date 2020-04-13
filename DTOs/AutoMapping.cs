using AutoMapper;
using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CarCategoryDTO, CarCategory>().ForMember(x=>x.Name, opt=>opt.MapFrom(src => src.Name)); 
            CreateMap<CarCategory, CarCategoryDTO>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<CarCategoryOnAddDTO, CarCategory>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
