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

        /*
            Se usa un procedimiento almacenado para obtener el path de la foto del usuario, se envía
            como parámetro de entrada el id y un parámetro de salida para retornar el resultado.
        */
        public string GetPathPhoto(long id)
        {
            SqlParameter[] parameters = {
                new SqlParameter{ ParameterName = "@id", SqlDbType=SqlDbType.BigInt, Value = id },
                new SqlParameter{ ParameterName = "@path",SqlDbType=SqlDbType.VarChar,Size=200, Direction = ParameterDirection.Output }
            };
            testContext.Database.ExecuteSqlRaw("exec SPGetPathPhoto @id, @path OUTPUT", parameters);
            string? path = parameters[1].Value.ToString();
            return path ?? string.Empty;
        }

        public bool CheckPhone(string phone)
        {
            return testContext.Personas.Any(x => x.Telefono == phone);
        }
    }
}
