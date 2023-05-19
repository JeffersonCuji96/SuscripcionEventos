using BL.Models;
using BL.ViewModels;
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
        /// <summary>
        /// Método para actualizar datos personales del usuario
        /// </summary>
        /// <remarks>
        /// Se excluye la foto ya que la actualización de imagen del usuario
        /// se realiza aparte
        /// </remarks>
        /// <param name="persona"></param>
        public void UpdatePerson(Persona persona)
        {
            testContext.Entry(persona).State = EntityState.Modified;
            testContext.Entry(persona).Property(x => x.Foto).IsModified = false;
            testContext.SaveChanges();
        }

        /// <summary>
        /// Método para actualizar la foto del usuario
        /// </summary>
        /// <param name="filePhotoViewModel"></param>
        public void UpdatePhoto(FilePhotoViewModel filePhotoViewModel)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Persona SET Foto = @foto WHERE Id = @id",
                new SqlParameter("@id", filePhotoViewModel.Id),
                new SqlParameter("@foto", filePhotoViewModel.Photo));
        }

        /// <summary>
        /// Método para obtener la ruta de la imagen del usuario
        /// </summary>
        /// <remarks>
        /// Se usa un procedimiento almacenado donde se envía como parámetro de entrada el id y un parámetro de salida para retornar el resultado.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para verificar si existe el teléfono
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public bool CheckPhone(string phone)
        {
            return testContext.Personas.Any(x => x.Telefono == phone);
        }
    }
}
