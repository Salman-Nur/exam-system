using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using Mapster;

namespace ExamSystem.Application
{
    public static class ApplicationMappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Member, MemberUpdateDTO>.NewConfig()
                .Map(dest => dest.ProfilePictureUrl, src => src.ProfilePictureUri);

            TypeAdapterConfig<MemberUpdateDTO, Member>.NewConfig()
                .Map(dest => dest.UpdatedAtUtc, src => DateTime.Now)
                .Ignore(dest => dest.Id)
                .AfterMapping((src, dest) =>
                {
                    if (src.ProfilePictureUrl is not null)
                    {
                        dest.ProfilePictureUri = src.ProfilePictureUrl;
                    }
                });

            TypeAdapterConfig<Tag, TagListDTO>.NewConfig();

            TypeAdapterConfig<TagListDTO, Tag>.NewConfig();
        }
    }
}
