using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface IPersonaRepository: IGenericRepository<Persona> 
    {
        void UpdatePerson(Persona persona);
        void UpdatePhoto(FilePhotoViewModel filePhotoViewModel);
        string GetPathPhoto(long id);
        bool CheckPhone(string phone);
    }
}
