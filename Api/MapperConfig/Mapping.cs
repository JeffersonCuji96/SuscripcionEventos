using AutoMapper;
using BL.DTO;
using BL.Models;

namespace SuscripcionEventosApi.MapperConfig
{
    public class Mapping : Profile
    {
        /// <summary>
        /// Mapeo de datos
        /// </summary>
        /// <remarks>
        /// Se definen los mapeos entre diferentes tipos de clases utilizando el método CreateMap de AutoMapper. 
        /// Cada llamada a CreateMap especifica un mapeo entre una clase de origen y una clase de destino.
        /// </remarks>
        public Mapping()
        {
            CreateMap<Usuario, AccesoDTO>();
            CreateMap<AccesoDTO, Usuario>();

            CreateMap<Persona, PersonaDTO>();
            CreateMap<PersonaDTO, Persona>();

            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.ImageBase64, opt => opt.Ignore())
                .ForMember(x => x.Clave, opt => opt.Ignore());
            CreateMap<UsuarioDTO, Usuario>();

            CreateMap<Evento,EventoDTO>()
                .ForMember(e => e.ImageBase64, o => o.MapFrom(m => m.Foto));
            CreateMap<EventoDTO, Evento>();

            CreateMap<Categoria, CategoriaDTO>();
            CreateMap<CategoriaDTO, Categoria>();

            CreateMap<Suscripcion, SuscripcionDTO>();
            CreateMap<SuscripcionDTO, Suscripcion>();
        }

    }
}
