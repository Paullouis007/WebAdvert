using AdvertApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<AdvertModel, CreateAdvertModel>().ReverseMap();
            CreateMap<AdvertResponse, AdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertRequest, ConfirmAdvertModel>().ReverseMap();
        }
    }
}
