using CityGuide.Models;
using CityGuide.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace CityGuide.Services
{
    public class AdvertisementService
    {
        private readonly AdvertisementRepository _repository;

        public AdvertisementService(AdvertisementRepository repository)
        {
            _repository = repository;
        }

        public List<Advertisement> GetAllAdvertisements()
        {
            return _repository.GetAll();
        }

        public List<Advertisement> GetAdvertisementsByUserId(int userId)
        {
            if (userId == 0)
                return new List<Advertisement>();
            var adById = _repository.GetAll();
            adById = adById.Where(a => a.UserId == userId).ToList();
            return adById;
        }

        public List<Advertisement> GetAllAdvertisementByCategory(string category)
        {
            var adByCategory = _repository.GetAll();
            if (!string.IsNullOrWhiteSpace(category)) {
                adByCategory = adByCategory.Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return adByCategory;
        }

        public Advertisement GetAdvertisementById(int id)
        {
            if (id <= 0)
                return null;
            var advertisements = _repository.GetAll();
            return advertisements.FirstOrDefault(a => a.Id == id);
        }

        public List<string> GetAllCategories()
        {
            return _repository.GetCategories();
        }

        public List<Advertisement> SearchAdvertisements(string searchTerm)
        {
            return _repository.Search(searchTerm);
        }

        public int SaveAdvertisement(Advertisement advertisement, int userId)
        {
            if (advertisement == null)
                return 0;
            if (string.IsNullOrEmpty(advertisement.Title) || string.IsNullOrEmpty(advertisement.Description) || string.IsNullOrEmpty(advertisement.Category))
                return -1;
            if (advertisement.Price < 0)
                return -2;
            if (userId <= 0)
                return -3;
            if (string.IsNullOrEmpty(advertisement.Address))
                return -4;
            return _repository.Save(advertisement, userId);
        }

        public int EditAdvertisement(Advertisement advertisement, int userId)
        {
            if (advertisement == null)
                return 0;
            if (advertisement.Id <= 0)
                return -1;
            if (string.IsNullOrEmpty(advertisement.Title) || string.IsNullOrEmpty(advertisement.Description) || string.IsNullOrEmpty(advertisement.Category))
                return -2;
            if (advertisement.Price < 0)
                return -3;
            if (userId <= 0)
                return -4;
            if (string.IsNullOrEmpty(advertisement.Address))
                return -5;
            return _repository.Edit(advertisement, userId);
        }

        public int DeleteAdvertisement(int id, int userId)
        {
            if (id <= 0)
                return 0;
            return _repository.Delete(id, userId);
        }

        private double CalculateDistance(double lat1, double lon1,double lat2, double lon2)
        {
            const double earthRadius = 6371;

            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) *
                Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        public List<Advertisement> GetNearbyAdvertisements(double latitude, double longitude)
        {
            const double radiusKm = 3;

            var advertisements = _repository.GetAll();

            return advertisements.Where(a => a.Lat.HasValue && a.Lon.HasValue).Where(a => CalculateDistance(latitude,longitude,a.Lat.Value,a.Lon.Value) <= radiusKm).ToList();
        }

        public string UploadImage(ImageUploadRequest request)
        {
            try
            {
                var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    return "-1";

                var bytes = Convert.FromBase64String(request.File);

                if (bytes.Length > 3 * 1024 * 1024)
                    return "-2";

                var uploadPath = WebConfigurationManager.AppSettings["ImageUploadPath"];
                var physicalPath = HostingEnvironment.MapPath(uploadPath);

                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);

                var fileName = Guid.NewGuid().ToString("N") + extension;

                File.WriteAllBytes(Path.Combine(physicalPath, fileName), bytes);

                return "/Images/" + fileName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}