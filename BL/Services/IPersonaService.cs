using BL.Models;

namespace BL.Services
{
    public interface IPersonaService:IGenericService<Persona>
    {
        void UpdatePerson(Persona persona);
        void UpdatePhoto(string foto, long id);
        string GetPathPhoto(long id);
        bool CheckPhone(string phone);
    }
}
