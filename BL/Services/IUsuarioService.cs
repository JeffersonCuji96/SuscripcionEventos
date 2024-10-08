﻿using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IUsuarioService:IGenericService<Usuario>
    {
        Tuple<AccessViewModel, int> Login(Usuario usuario);
        void InsertUserPerson(Usuario usuario);
        bool CheckPassword(UserPasswordViewModel userPassViewModel);
        bool CheckEmail(string email);
        void UpdateEmail(UserEmailViewModel userEmailViewModel);
        void UpdateClave(UserPasswordViewModel userPassViewModel);
        void RecoveryAccess(UserEmailViewModel userEmailViewModel, DateTime date);
        bool CheckToken(TokenValidViewModel tokenValidViewModel, DateTime currentDate);
        bool ChangeClave(TokenPasswordViewModel tokenPassViewModel);
        bool ConfirmEmail(TokenValidViewModel tokenValidViewModel);
        bool VerifyStatusUser(long id);
    }
}
