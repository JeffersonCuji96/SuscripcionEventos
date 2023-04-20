using BL.DTO;
using BL.Models;
using BL.Repositories;
using BL.ViewModels;

namespace BL.Services.Implements
{
    public class SuscripcionService : GenericService<Suscripcion>, ISuscripcionService
    {
        private readonly ISuscripcionRepository suscripcionRepository;
        public SuscripcionService(ISuscripcionRepository suscripcionRepository) : base(suscripcionRepository)
        {
            this.suscripcionRepository = suscripcionRepository;
        }
        public long SuscribeEvent(long idSuscripcion, bool tipo)
        {
            return suscripcionRepository.SuscribeEvent(idSuscripcion, tipo);
        }
        public Tuple<long, int> CheckSuscribeUser(SuscribeCheckViewModel suscribe)
        {
            return suscripcionRepository.CheckSuscribeUser(suscribe);
        }
        public string CheckDateEventSuscription(EventCheckViewModel eventViewModel)
        {
            return suscripcionRepository.CheckDateEventSuscription(eventViewModel);
        }
    }
}
