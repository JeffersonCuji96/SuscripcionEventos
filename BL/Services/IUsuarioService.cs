﻿using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IUsuarioService:IGenericService<Usuario>
    {
        AccessViewModel Login(Usuario usuario);
        Usuario? GetUserPersonById(long id);
        void InsertUserPerson(Usuario usuario);
        bool CheckPassword(string password, long id);
        bool CheckEmail(string email);
        void UpdateEmail(string email, long id);
        void UpdateClave(string password, long id);
        void RecoveryAccess(UserEmailViewModel userEmailViewModel, DateTime date);
        bool CheckToken(DateTime currentDate,string token);
    }
}
