using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentForum.BusinessLogic;
using StudentForumAspCore.Models;

namespace StudentForumAspCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GroupsService _groupsService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, GroupsService groupsService, IMapper mapper)
        {
            _logger = logger;
            _groupsService = groupsService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var unseenTopics = _mapper.Map<List<IndexTopicModel>>(_groupsService.GetUnseenTopics(userId));
                var seenTopics = _mapper.Map<List<IndexTopicModel>>(_groupsService.GetSeenTopicsWithNewAnswers(userId));

                var model = new IndexModel
                {
                    TopicsWithNewAnswers = seenTopics,
                    UnseenTopics = unseenTopics
                };

                ViewBag.UserId = userId;

                return View("IndexLoggedIn", model);
            }
            return Error();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
