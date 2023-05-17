using AutoMapper;
using BlogApplication.DTO;
using BlogApplication.Repository.Entities;
using System;
using System.Globalization;

namespace BlogApplication.Service.AutoMapper
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile()
        {
            CreateMap<BlogEntity, BlogDTO>()
              .ForMember(dest => dest.BlogId, opt => opt.MapFrom(sourceMember => sourceMember.SK.Replace($"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}", string.Empty)))
              .ForMember(dest => dest.Title, opt => opt.MapFrom(source => source.Title))
              .ForMember(dest => dest.Content, opt => opt.MapFrom(source => source.Content))
              .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(source => source.AuthorId.Replace($"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}", string.Empty)))
              .ForMember(dest => dest.Published, opt => opt.MapFrom(source => source.Published))
              .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(source => DateTime.ParseExact(source.CreatedDate, AppConstants.ISO_8601_DATE_FORMAT, CultureInfo.InvariantCulture)));

            CreateMap<AuthorEntity, AuthorDTO>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(sourceMember => sourceMember.SK.Replace($"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}", string.Empty)));
        }
    }
}
