using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.API.Profiles.AdministrativeTools
{
    public class DeadlinesProfile : Profile
    {
        public DeadlinesProfile()
        {
            CreateMap<CreateDeadlineDTO, Deadline>();
            CreateMap<Deadline, DeadlineListDTO>();
            CreateMap<Deadline, DeadlineDTO>();
        }
    }
}

