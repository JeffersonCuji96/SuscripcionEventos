using System.Collections.Generic;
using BL.Repositories;

namespace BL.Services.Implements
{
    public class GenericService<TEntity>:IGenericService<TEntity> where TEntity:class
    {
        private readonly IGenericRepository<TEntity> genericRepository;
        public GenericService(IGenericRepository<TEntity> genericRepository)
        {
            this.genericRepository = genericRepository;
        }
        public void Delete(int id)
        {
            genericRepository.Delete(id);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return genericRepository.GetAll();
        }
        public TEntity GetById(long id)
        {
            return genericRepository.GetById(id);
        }
        public TEntity Insert(TEntity entity)
        {
            return genericRepository.Insert(entity);
        }
        public TEntity Update(TEntity entity)
        {
            return genericRepository.Update(entity);
        }
    }
}
