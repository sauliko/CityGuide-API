using CityGuide.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CityGuide.Repositories
{
    public class AdvertisementRepository
    {
        private readonly string _filePath;

        public AdvertisementRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<Advertisement> GetAll()
        {
            if (!File.Exists(_filePath))
                return new List<Advertisement>();

            string json = File.ReadAllText(_filePath);

            return JsonConvert.DeserializeObject<List<Advertisement>>(json)
                   ?? new List<Advertisement>();
        }

        public List<string> GetCategories()
        {
            var categories = new List<string> { "Electronics", "Vehicles", "Real Estate", "Services", "Fashion", "Home & Garden", "Sports & Outdoors", "Toys & Games", "Health & Beauty" };
            return categories;  
        }



        public List<Advertisement> Search(string searchTerm)
        {
            var advertisements = GetAll();
            if (string.IsNullOrEmpty(searchTerm))
                return advertisements;
            advertisements = advertisements.Where(a => a.Title.ToLower().Contains(searchTerm.ToLower()) || a.Description.ToLower().Contains(searchTerm.ToLower()) || a.Category.ToLower().Contains(searchTerm.ToLower()) || a.Address.ToLower().Contains(searchTerm.ToLower())).ToList();
            return advertisements;
        }

        public int Save(Advertisement advertisement, int userId)
        {
            var advertisements = GetAll();
            advertisement.Id = advertisements.Count > 0 ? advertisements.Max(a => a.Id) + 1 : 1;
            advertisement.UserId = userId;
            advertisements.Add(advertisement);
            string json = JsonConvert.SerializeObject(advertisements, Formatting.Indented);
            File.WriteAllText(_filePath, json);
            return advertisement.Id;
        }

        public int Edit(Advertisement advertisement, int userId)
        {
            var advertisements = GetAll();
            var existingAd = advertisements.FirstOrDefault(a => a.Id == advertisement.Id && a.UserId == userId);
            if (existingAd != null)
            {
                existingAd.Title = advertisement.Title;
                existingAd.Description = advertisement.Description;
                existingAd.Category = advertisement.Category;
                existingAd.Price = advertisement.Price;
                existingAd.Address = advertisement.Address;
                existingAd.UserId = advertisement.UserId;
                existingAd.Lat = advertisement.Lat;
                existingAd.Lon = advertisement.Lon;
                existingAd.ImageUrl = advertisement.ImageUrl;
                string json = JsonConvert.SerializeObject(advertisements, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                return existingAd.Id;
            }
            return 0; 
        }

        public int Delete(int id,int userId)
        {
            var advertisements = GetAll();
            var adToDelete = advertisements.FirstOrDefault(a => a.Id == id && a.UserId == userId);
            if (adToDelete != null)
            {
                advertisements.Remove(adToDelete);
                string json = JsonConvert.SerializeObject(advertisements, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                return adToDelete.Id;
            }
            return 0;
        }

    }
}