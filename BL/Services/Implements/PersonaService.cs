using BL.Models;
using BL.Repositories;

namespace BL.Services.Implements
{
    public class PersonaService : GenericService<Persona>, IPersonaService
    {
        private readonly IPersonaRepository personaRepository;
        public PersonaService(IPersonaRepository personaRepository) : base(personaRepository)
        {
            this.personaRepository = personaRepository;
        }
    }
}
