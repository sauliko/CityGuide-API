using CityGuide.Models;
using CityGuide.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CityGuide.Repositories
{
    public class UserRepository
    {
        private readonly string _filePath;

        public UserRepository(string filePath)
        {
            _filePath = filePath;
        }

        public int Register(User user)
        {
            var users = GetAll();
            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            user.Password = EncryptionAndDecryption.EnCrypt(user.Password, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");
            users.Add(user);
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            return user.Id;
        }

        public List<User> GetAll()
        {
            if (!File.Exists(_filePath))
                return new List<User>();
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<User>>(json)
                   ?? new List<User>();
        }

        public int Login(string username, string password)
        {
            var users = GetAll();
            password = EncryptionAndDecryption.EnCrypt(password, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            return user?.Id ?? 0;
        }

        public string GetUsernameById(int userId)
        {
            var users = GetAll();
            var user = users.FirstOrDefault(u => u.Id == userId);
            return user?.Username;
        }
    }
}