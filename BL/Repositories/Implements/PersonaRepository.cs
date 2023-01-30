using BL.Models;

namespace BL.Repositories.Implements
{
    public class PersonaRepository : GenericRepository<Persona>, IPersonaRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        public PersonaRepository(DbSuscripcionEventosContext testContext) : base(testContext)
        {
            this.testContext = testContext;
        }
    }
}
