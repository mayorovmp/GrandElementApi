using AutoMapper;
using GrandElementApi.Data;
using System.Collections.Generic;

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
                    bool res;
                    if (src.Vat.HasValue)
                        res = src.Vat.Value == 1;
                    else
                        res = false;
                    return res;}));

            CreateMap<CarOnAddDTO, Car>().ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    return src.Vat ? 1 : 0; ;
                }));
            CreateMap<CarDTO, Car>().ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    return src.Vat ? 1 : 0;
                }));

            CreateMap<DeliveryAddress, DeliveryAddressDTO>().ForMember(dest=>dest.Contacts, opt=>opt.MapFrom(src=>src.Contacts));
            CreateMap<DeliveryAddressOnAddDTO, DeliveryAddress>();
            CreateMap<DeliveryAddressDTO, DeliveryAddress>();

            CreateMap<DeliveryContact, DeliveryContactDTO>();
            CreateMap<DeliveryContactOnAddDTO, DeliveryContact>();
            CreateMap<DeliveryContactDTO, DeliveryContact>(); 

            CreateMap<ClientOnEditDTO, Client>();
            CreateMap<ClientOnAddDTO, Client>();
            CreateMap<ClientDTO, Client>();
            CreateMap<Client, ClientDTO>();

            CreateMap<Supplier, SupplierDTO>().ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    bool res;
                    if (src.Vat.HasValue)
                        res = src.Vat.Value == 1;
                    else
                        res = false;
                    return res;
                }));
            CreateMap<SupplierProduct, SupplierProductDTO>()
                .ForMember(dest=>dest.Id, opt=>opt.MapFrom(src=>src.ProductId))
                .ForMember(dest=>dest.Name, opt=>opt.MapFrom(src=>src.Product.Name));
            CreateMap<SupplierDTO, Supplier>()
            .ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    return src.Vat ? 1 : 0;
                }));

            CreateMap<SupplierOnAddDTO, Supplier>()
                .ForMember(x => x.Vat,
                    opt => opt.MapFrom((src, _) => {
                        return src.Vat ? 1 : 0;
                    }));
            CreateMap<SupplierOnEditDTO, Supplier>()
                .ForMember(x => x.Vat,
                opt => opt.MapFrom((src, _) => {
                    return src.Vat ? 1 : 0;
                })); 

            CreateMap<SupplierProductDTO, SupplierProduct>()
                .ForMember(dest=>dest.ProductId, opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest=>dest.Id, opt=> opt.MapFrom(src=>0));

            CreateMap<Request, RequestDTO>()
                .ForMember(
                    dest => dest.IsLong,
                    opt => opt.MapFrom(
                        (src, _) => { return src.IsLong == 1; }
                    )
                )
                .ForMember(dest=>dest.Status, 
                    opt=>opt.MapFrom(
                        (src, _) => {
                            return src.Status switch
                            {
                                RequestStatus.Active => "Активн.",
                                RequestStatus.Completed => "Заверш.",
                                _ => "Не определен.",
                            };
                        }))
                .ForMember(d=>d.CarVat, opt=>opt.MapFrom(
                    (src, _) =>
                    {
                        bool? res;
                        if (!src.CarVat.HasValue)
                            res = null;
                        else
                            res = src.CarVat.Value == 1 ? true : false;
                        return res;
                    }
                ))
                .ForMember(d => d.SupplierVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          bool? res;
                          if (!src.SupplierVat.HasValue)
                              res = null;
                          else
                              res = src.SupplierVat.Value == 1 ? true : false;
                          return res;
                      }
                  ))
                .ForMember(
                    dest => dest.StatusId, opt => opt.MapFrom(src => src.Status));

            CreateMap<RequestOnAddDTO, Request>()
                .ForMember(
                    dest => dest.IsLong,
                    opt => opt.MapFrom(
                        (src, _) => { return src.IsLong ? 1 : 0; }
                    )
                )
                .ForMember(
                    dest => dest.Client,
                    opt => opt.MapFrom(
                        (src, _) => { Client c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.CarCategory,
                    opt => opt.MapFrom(
                        (src, _) => { CarCategory c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Car,
                    opt => opt.MapFrom(
                        (src, _) => { Car c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Product,
                    opt => opt.MapFrom(
                        (src, _) => { Product c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Supplier,
                    opt => opt.MapFrom(
                        (src, _) => { Supplier c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.DeliveryAddress,
                    opt => opt.MapFrom(
                        (src, _) => { DeliveryAddress c = null; return c; }
                    )
                )

                .ForMember(
                    dest=>dest.Status, 
                    opt=>opt.MapFrom(
                        src=>src.StatusId
                    )
            )
                .ForMember(d => d.CarVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.CarVat.HasValue)
                              res = null;
                          else
                            res = src.CarVat.Value ? 1 : 0;
                          return res;
                      }
                  ))
                .ForMember(d => d.CarVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.CarVat.HasValue)
                              res = null;
                          else
                            res = src.CarVat.Value ? 1 : 0;
                          return res;
                      }
                  ))
                .ForMember(d => d.SupplierVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.SupplierVat.HasValue)
                              res = null;
                          else
                            res = src.SupplierVat.Value ? 1 : 0;
                          return res;
                      }
                  ));

            CreateMap<RequestOnEditDTO, Request>()
                .ForMember(
                    dest => dest.IsLong,
                    opt => opt.MapFrom(
                        (src, _) => { return src.IsLong ? 1 : 0; }
                    )
                )
                .ForMember(
                    dest => dest.RowStatus,
                    opt => opt.MapFrom(
                        (src, _) => { return RowStatus.Active; }
                    )
                )
                .ForMember(
                    dest => dest.Client,
                    opt => opt.MapFrom(
                        (src, _) => { Client c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.CarCategory,
                    opt => opt.MapFrom(
                        (src, _) => { CarCategory c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Car,
                    opt => opt.MapFrom(
                        (src, _) => { Car c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Product,
                    opt => opt.MapFrom(
                        (src, _) => { Product c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Supplier,
                    opt => opt.MapFrom(
                        (src, _) => { Supplier c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.DeliveryAddress,
                    opt => opt.MapFrom(
                        (src, _) => { DeliveryAddress c = null; return c; }
                    )
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(
                        src => src.StatusId
                    )
                )
                .ForMember(d => d.CarVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.CarVat.HasValue)
                              res = null;
                          else
                              res = src.CarVat.Value ? 1 : 0;
                          return res;
                      }
                 ))
                .ForMember(d => d.CarVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.CarVat.HasValue)
                              res = null;
                          else
                              res = src.CarVat.Value ? 1 : 0;
                          return res;
                      }
                  ))
                .ForMember(d => d.SupplierVat, opt => opt.MapFrom(
                      (src, _) =>
                      {
                          int? res;
                          if (!src.SupplierVat.HasValue)
                              res = null;
                          else
                              res = src.SupplierVat.Value ? 1 : 0;
                          return res;
                      }
                  )); 


        }
    }
}