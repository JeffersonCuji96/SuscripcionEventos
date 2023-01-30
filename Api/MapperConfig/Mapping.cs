using AutoMapper;
using BL.DTO;
using BL.Models;

namespace SuscripcionEventosApi.MapperConfig
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Usuario, AccesoDTO>();
            CreateMap<AccesoDTO, Usuario>();

            CreateMap<Persona, PersonaDTO>();
            CreateMap<PersonaDTO, PersonaDTO>();
        }

    }
}
