using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IPersonaService:IGenericService<Persona>
    {
        void UpdatePerson(Persona persona);
        void UpdatePhoto(FilePhotoViewModel filePhotoViewModel);
        string GetPathPhoto(long id);
        bool CheckPhone(string phone);
    }
}
