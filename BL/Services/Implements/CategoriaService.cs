using BL.Models;
using BL.Repositories;

namespace BL.Services.Implements
{
    public class CategoriaService : GenericService<Categoria>, ICategoriaService
    {
        private readonly ICategoriaRepository categoriaRepository;
        public CategoriaService(ICategoriaRepository categoriaRepository) : base(categoriaRepository)
        {
            this.categoriaRepository = categoriaRepository;
        }
    }
}
