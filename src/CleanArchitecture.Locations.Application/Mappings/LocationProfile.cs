using AutoMapper;
using CleanArchitecture.Locations.Application.DTOs;
using CleanArchitecture.Locations.Domain.Entities;

namespace CleanArchitecture.Locations.Application.Mappings;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<Location, LocationDto>()
            .ForMember(t => t.Id, o => o.MapFrom(t => t.Id))
            .ForMember(t => t.Latitude, o => o.MapFrom(t => t.Latitude))
            .ForMember(t => t.Longitude, o => o.MapFrom(t => t.Longitude))
            .ReverseMap();
        
        CreateMap<LocationDto, Location>()
            .ForMember(t => t.Id, o => o.MapFrom(t => t.Id))
            .ForMember(t => t.Latitude, o => o.MapFrom(t => t.Latitude))
            .ForMember(t => t.Longitude, o => o.MapFrom(t => t.Longitude))
            .ReverseMap();
    }
}