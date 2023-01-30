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
        public void UpdatePerson(Persona persona)
        {
            personaRepository.UpdatePerson(persona);
        }
        public void UpdatePhoto(string foto, long id)
        {
            personaRepository.UpdatePhoto(foto, id);
        }
    }
}
