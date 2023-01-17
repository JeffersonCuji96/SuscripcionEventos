using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BL.Models;

namespace BL.Repositories.Implements
{
    public class GenericRepository<TEntity>:IGenericRepository<TEntity> where TEntity:class
    {
        private readonly DbSuscripcionEventosContext testContext;
        public GenericRepository(DbSuscripcionEventosContext testContext)
        {
            this.testContext = testContext;
        }
        public void Delete(int id)
        {
            var entity = GetById(id);

            if (entity == null)
                throw new Exception("The entity is null");
            testContext.Set<TEntity>().Remove(entity);
            testContext.SaveChanges();
        }
        public IEnumerable<TEntity> GetAll()
        {
            return testContext.Set<TEntity>().ToList();
        }
        public TEntity GetById(long id)
        {
            return testContext.Set<TEntity>().Find(id);
        }
        public TEntity Insert(TEntity entity)
        {
            testContext.Set<TEntity>().Add(entity);
            testContext.SaveChanges();
            return entity;
        }
        public TEntity Update(TEntity entity)
        {
            testContext.Entry(entity).State = EntityState.Modified;
            testContext.SaveChanges();
            return entity;
        }
    }
}
