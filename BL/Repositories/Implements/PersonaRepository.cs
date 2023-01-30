using BL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BL.Repositories.Implements
{
    public class PersonaRepository : GenericRepository<Persona>, IPersonaRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        public PersonaRepository(DbSuscripcionEventosContext testContext) : base(testContext)
        {
            this.testContext = testContext;
        }
        public void UpdatePerson(Persona persona)
        {
            testContext.Entry(persona).State = EntityState.Modified;
            testContext.Entry(persona).Property(x => x.Foto).IsModified = false;
            testContext.SaveChanges();
        }
        public void UpdatePhoto(string foto, long id)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Persona SET Foto = @foto WHERE Id = @id",
                new SqlParameter("@id", id),
                new SqlParameter("@foto", foto));
        }
    }
}
