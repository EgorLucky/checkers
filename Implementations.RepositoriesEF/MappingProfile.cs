using AutoMapper;

namespace Implementations.RepositoriesEF
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DomainLogic.Models.Game, Game>()
                .ReverseMap();
        }
    }
}