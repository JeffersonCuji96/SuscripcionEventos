using BL.Models;

namespace BL.Repositories
{
    public interface IPersonaRepository: IGenericRepository<Persona> 
    {
        void UpdatePerson(Persona persona);
        void UpdatePhoto(string foto, long id);
        string GetPathPhoto(long id);
        bool CheckPhone(string phone);
    }
}
