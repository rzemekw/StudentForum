using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentForum.BusinessLogic;
using StudentForumAspCore.Models;

namespace StudentForumAspCore.Controllers
{
    [Authorize]
    public class UniversitiesController : Controller
    {
        private readonly CountriesService _countriesService;
        private readonly UniversitiesService _universitiesService;

        public UniversitiesController(CountriesService countriesService, UniversitiesService universitesService)
        {
            _countriesService = countriesService;
            _universitiesService = universitesService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Join()
        {
            var model = new JoinUniversityModel
            {
                Countries = _countriesService.GetCountries()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    }).ToList()
            };
            return PartialView("_JoinPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinUniversityModel model)
        {
            if (model == null)
                return BadRequest();
            if(!ModelState.IsValid)
            {
                return Ok(Json(ModelState
                    .Where(s => s.Value.Errors.Any())
                    .Select(s => new { Key = s.Key, Error = s.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault() })));
            }

            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_universitiesService.IsMember(model.UniversityId.Value, userId))
                return BadRequest(Json("You have already joined this University"));

            _universitiesService.AddUserToUniversity(userId, model.UniversityId.Value);

            return Ok(Json(new { Redirect = Url.Action("Index", "Home") }));
        }
    }
}
