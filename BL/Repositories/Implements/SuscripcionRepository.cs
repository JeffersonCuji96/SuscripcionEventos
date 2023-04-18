using BL.Common;
using BL.Models;
using BL.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace BL.Repositories.Implements
{
    public class SuscripcionRepository : GenericRepository<Suscripcion>, ISuscripcionRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        private readonly AppSettings appSettings;
        public SuscripcionRepository(DbSuscripcionEventosContext testContext, IOptions<AppSettings> appSettings) : base(testContext)
        {
            this.testContext = testContext;
            this.appSettings = appSettings.Value;
        }
        public long SuscribeEvent(long idSuscripcion, bool tipo)
        {
            if (tipo == false)
                testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 2 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            else testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 1 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            return idSuscripcion;
        }

        public Tuple<long,int> CheckSuscribeUser(SuscribeCheckViewModel suscribe)
        {
            var oSuscripcion = testContext.Suscripciones.FirstOrDefault(x => x.IdUsuario == suscribe.IdUsuario && x.IdEvento == suscribe.IdEvento);
            long id = oSuscripcion?.Id ?? 0;
            int estado = oSuscripcion?.IdEstado ?? 0;
            return Tuple.Create(id,estado);
        }

    }
}
