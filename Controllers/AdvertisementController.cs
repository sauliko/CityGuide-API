using CityGuide.Models;
using CityGuide.Repositories;
using CityGuide.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace CityGuide.Controllers
{
    public class AdvertisementController : ApiController
    {
        private readonly AdvertisementService _service;
        private readonly TokenService _tokenService;

        public AdvertisementController()
        {
            string filePath =
                System.Web.Hosting.HostingEnvironment.MapPath(
                    "~/App_Data/advertisements.json");

            var repository = new AdvertisementRepository(filePath);

            _service = new AdvertisementService(repository);
            _tokenService = new TokenService();
        }

        [HttpGet]
        public IHttpActionResult GetAllAds()
        {
            var advertisements = _service.GetAllAdvertisements();

            return Ok(advertisements);
        }

        [HttpGet]
        public IHttpActionResult GetAdvertisementsByUserId(int userId)
        {
            var advertisements = _service.GetAdvertisementsByUserId(userId);
            return Ok(advertisements);
        }

        [HttpGet]
        public IHttpActionResult GetAdvertisementByCategory(string category)
        {
            var advertisements = _service.GetAllAdvertisementByCategory(category);
            if (advertisements == null)
                return NotFound();
            return Ok(advertisements);
        }

        [HttpGet]
        public IHttpActionResult GetAdvertisementById(int id)
        {
            var advertisement = _service.GetAdvertisementById(id);
            if (advertisement == null)
                return NotFound();
            return Ok(advertisement);
        }

        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            var categories = _service.GetAllCategories();
            if (categories == null || !categories.Any())
                return NotFound();
            return Ok(categories);
        }

        [HttpGet]
        public IHttpActionResult SearchAdvertisements(string searchTerm)
        {
            var advertisements = _service.SearchAdvertisements(searchTerm);
            return Ok(advertisements);
        }


        [HttpPost]
        [JwtAuthorize]
        public IHttpActionResult CreateAdvertisement([FromBody] Advertisement advertisement)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();
            if (advertisement == null)
                return BadRequest("Advertisement data is required.");
            advertisement.UserId = userId.Value;
            int result = _service.SaveAdvertisement(advertisement, advertisement.UserId);
            switch (result)
            {
                case -1:
                    return BadRequest("Title, Description, and Category are required.");
                case -2:
                    return BadRequest("Price cannot be negative.");
                case -3:
                    return BadRequest("UserId must be greater than zero.");
                case -4:
                    return BadRequest("Address is required.");
                case 0:
                    return BadRequest("Failed to save the advertisement.");
                default:
                    return Ok(advertisement);
            }
        }

        [HttpPut]
        [JwtAuthorize]
        public IHttpActionResult UpdateAdvertisement([FromBody] Advertisement advertisement)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();
            if (advertisement == null)
                return BadRequest("Advertisement data is required.");
            advertisement.UserId = userId.Value;
            int result = _service.EditAdvertisement(advertisement, advertisement.UserId);
            switch (result)
            {
                case -1:
                    return BadRequest("Invalid advertisement ID.");
                case -2:
                    return BadRequest("Title, Description, and Category are required.");
                case -3:
                    return BadRequest("Price cannot be negative.");
                case -4:
                    return BadRequest("UserId must be greater than zero.");
                case -5:
                    return BadRequest("Address is required.");
                case 0:
                    return BadRequest("Failed to update the advertisement.");
                default:
                    return Ok(advertisement);
            }
        }

        [HttpDelete]
        [JwtAuthorize]
        public IHttpActionResult DeleteAdvertisement(int id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized();
            if (id <= 0)
                return BadRequest("Invalid advertisement ID.");
            int result = _service.DeleteAdvertisement(id, userId.Value);
            if (result == 0)
                return NotFound();
            return Ok($"Advertisement with ID {id} has been deleted.");
        }

        private int? GetCurrentUserId()
        {
            var identity = User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null || !int.TryParse(claim.Value, out int userId))
                return null;

            return userId;
        }

        [HttpGet]
        public IHttpActionResult GetNearbyAdvertisements(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90 ||
                longitude < -180 || longitude > 180)
                return BadRequest("Invalid coordinates.");

            var advertisements = _service.GetNearbyAdvertisements(latitude, longitude);

            return Ok(advertisements);
        }

        [HttpPost]
        [JwtAuthorize]
        public IHttpActionResult UploadImage([FromBody] ImageUploadRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.File))
                return BadRequest("Image is required.");

            var imageUrl = _service.UploadImage(request);

            if (imageUrl == "-1")
                return BadRequest("Invalid image format.");

            if (imageUrl == "-2")
                return BadRequest("Image size exceeds the limit.");

            if (string.IsNullOrEmpty(imageUrl))
                return BadRequest("Failed to upload the image.");

            return Ok(new { ImageUrl = imageUrl });
        }

    }
}