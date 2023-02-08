﻿using BL.Models;
using BL.Repositories;
using BL.ViewModels;

namespace BL.Services.Implements
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }
        public AccessViewModel Login(Usuario usuario)
        {
            return this.usuarioRepository.Login(usuario);
        }
        public Usuario? GetUserPersonById(long id)
        {
            return this.usuarioRepository.GetUserPersonById(id);
        }
        public void InsertUserPerson(Usuario usuario)
        {
            usuarioRepository.InsertUserPerson(usuario);
        }
        public bool CheckPassword(string password, long id)
        {
            return usuarioRepository.CheckPassword(password, id);
        }
        public bool CheckEmail(string email)
        {
            return usuarioRepository.CheckEmail(email);
        }
        public void UpdateEmail(string email, long id)
        {
            usuarioRepository.UpdateEmail(email, id);
        }
        public void UpdateClave(string password, long id)
        {
            usuarioRepository.UpdateClave(password, id);
        }
        public void RecoveryAccess(UserEmailViewModel userEmailViewModel, DateTime date)
        {
            usuarioRepository.RecoveryAccess(userEmailViewModel, date);
        }
        public bool CheckToken(DateTime currentDate,string token)
        {
            return usuarioRepository.CheckToken(currentDate, token);
        }
    }
}
