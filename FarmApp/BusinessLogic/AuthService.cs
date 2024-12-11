using DataAccessLayer;
using System;

namespace BusinessLogic
{
    public class AuthService
    {
        private UserRepository _userRepository;

        public AuthService(string connectionString)
        {
            _userRepository = new UserRepository(connectionString);
        }

        public bool Register(string email, string password)
        {
            // Використання UserRepository для реєстрації користувача
            return _userRepository.RegisterUser(email, password);
        }

        public bool Login(string email, string password)
        {
            // Використання UserRepository для входу користувача
            return _userRepository.LoginUser(email, password);
        }

        public int GetUserId(string email)
        {
            // Використання UserRepository для отримання ID користувача
            return _userRepository.GetUserId(email);
        }
    }
}



