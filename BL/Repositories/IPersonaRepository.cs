using BL.Models;

namespace BL.Repositories
{
    public interface IPersonaRepository: IGenericRepository<Persona> 
    {
        void UpdatePerson(Persona persona);
        void UpdatePhoto(string foto, long id);
    }
}
