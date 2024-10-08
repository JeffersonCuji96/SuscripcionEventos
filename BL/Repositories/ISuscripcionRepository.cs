﻿using BL.DTO;
using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface ISuscripcionRepository : IGenericRepository<Suscripcion>
    {
        long SuscribeEvent(long idSuscripcion, bool tipo);
        Tuple<long,int> CheckSuscribeUser(SuscribeCheckViewModel suscribe);
        string CheckDateEventSuscription(EventCheckViewModel eventViewModel);
        IEnumerable<EventoSuscripcionViewModel> GetSuscriptionsByUser(long idUsuario);
    }
}
