using CityGuide.Models;
using CityGuide.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace CityGuide.Services
{
    public class UserService
    {
        public readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public int Register(User user)
        {
            if (user == null)
                return 0;
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return -1;
            if(_repository.GetAll().Any(u => u.Username == user.Username))
                return -2;
            if(_repository.GetAll().Any(u => u.Email == user.Email))
                return -3;
            return _repository.Register(user);
        }

        public int Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return 0;
            var result = _repository.Login(username, password);
            return result;
        }

        public string GetUsernameById(int userId)
        {
            if (userId <= 0)
                return null;
            return _repository.GetUsernameById(userId);
        }

    }
}