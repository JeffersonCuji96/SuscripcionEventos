using BL.Models;
using BL.Repositories;
using BL.ViewModels;

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
        public void UpdatePhoto(FilePhotoViewModel filePhotoViewModel)
        {
            personaRepository.UpdatePhoto(filePhotoViewModel);
        }
        public string GetPathPhoto(long id)
        {
            return personaRepository.GetPathPhoto(id);
        }
        public bool CheckPhone(string phone)
        {
            return personaRepository.CheckPhone(phone);
        }
    }
}
