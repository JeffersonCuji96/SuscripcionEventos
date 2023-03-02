using BL.Common;
using BL.Models;
using Microsoft.Extensions.Options;

namespace BL.Repositories.Implements
{
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        private readonly AppSettings appSettings;
        public CategoriaRepository(DbSuscripcionEventosContext testContext, IOptions<AppSettings> appSettings) : base(testContext)
        {
            this.testContext = testContext;
            this.appSettings = appSettings.Value;
        }
    }
}
