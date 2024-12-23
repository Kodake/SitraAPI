using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Entities.AdministrativeTools;

namespace BackEnd.Profiles.AdministrativeTools
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
