using AutoMapper;
using BlogApplication.DTO;
using BlogApplication.Repository.Entities;

namespace BlogApplication.Service.AutoMapper
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<BlogDTO, BlogEntity>()
                .ForMember(dest => dest.PK, opt => opt.MapFrom(sourceMember => AppConstants.BLOG_PARTITION_KEY))
                .ForMember(dest => dest.SK, opt => opt.MapFrom(source => $"{AppConstants.BLOG_PARTITION_KEY}{AppConstants.DELIMITER}{source.BlogId}".ToUpper()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(source => source.CreatedDate.ToString(AppConstants.ISO_8601_DATE_FORMAT)))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(source => $"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}{source.AuthorId}".ToUpper()))
                .ForMember(dest => dest.GSI1PK, opt => opt.MapFrom(source => $"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}{source.AuthorId}".ToUpper()))
                .ForMember(dest => dest.GSI1SK, opt => opt.MapFrom(source => source.CreatedDate.ToString(AppConstants.ISO_8601_DATE_FORMAT)))
                .ForMember(dest => dest.GSI2PK, opt => opt.MapFrom(source => AppConstants.BLOG_PARTITION_KEY))
                .ForMember(dest => dest.GSI2SK, opt => opt.MapFrom(source => source.CreatedDate.ToString(AppConstants.ISO_8601_DATE_FORMAT)));

            CreateMap<AuthorDTO, AuthorEntity>()
               .ForMember(dest => dest.PK, opt => opt.MapFrom(sourceMember => AppConstants.AUTHOR_PARTITION_KEY))
               .ForMember(dest => dest.SK, opt => opt.MapFrom(source => $"{AppConstants.AUTHOR_PARTITION_KEY}{AppConstants.DELIMITER}{source.AuthorId}".ToUpper()))
               .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(source => source.AuthorEmail.ToLower()));

        }
    }
}
