using AutoMapper;
using GrandElementApi.Data;

namespace GrandElementApi.DTOs
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CarCategoryDTO, CarCategory>().ForMember(x=>x.Name, opt=>opt.MapFrom(src => src.Name)); 
            CreateMap<CarCategory, CarCategoryDTO>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<CarCategoryOnAddDTO, CarCategory>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Product, ProductDTO>().ForMember(x=>x.Name, opt=>opt.MapFrom(src=>src.Name));
            CreateMap<ProductDTO, Product>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<ProductOnAddDTO, Product>().ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Car, CarDTO>().ForMember(x => x.Vat, 
                opt => opt.MapFrom((src, _)=> {
                    bool? res;
                    if (src.Vat.HasValue)
                        res = src.Vat.Value == 1;
                    else
                        res = null;
                    return res;}));

            CreateMap<CarOnAddDTO, Car>().ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    int? t;
                    if (!src.Vat.HasValue)
                    {
                        t = null;
                    }
                    else { 
                        t = src.Vat.Value ? 1 : 0;
                    }
                    return t;
                }));
            CreateMap<CarDTO, Car>().ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    int? t;
                    if (!src.Vat.HasValue)
                    {
                        t = null;
                    }
                    else
                    {
                        t = src.Vat.Value ? 1 : 0;
                    }
                    return t;
                }));

        }
    }
}