﻿using BL.DTO;
using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface ISuscripcionService : IGenericService<Suscripcion>
    {
        long SuscribeEvent(long idSuscripcion, bool tipo);
        Tuple<long, int> CheckSuscribeUser(SuscribeCheckViewModel suscribe);
        string CheckDateEventSuscription(EventCheckViewModel eventViewModel);
        IEnumerable<EventoSuscripcionViewModel> GetSuscriptionsByUser(long idUsuario);
    }
}
