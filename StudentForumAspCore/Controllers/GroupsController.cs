using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using StudentForum.BusinessLogic;
using StudentForumAspCore.Hubs;
using StudentForumAspCore.Models;

namespace StudentForumAspCore.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly GroupsService _groupsService;
        private readonly UniversitiesService _universitiesService;
        private readonly UsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IHubContext<GroupsHub> _groupsHubContext;

        public GroupsController(ILogger<GroupsController> logger, GroupsService groupsService,
                                UniversitiesService universitiesService, IMapper mapper,
                                IHubContext<GroupsHub> groupsHubContext, UsersService usersService)
        {
            _logger = logger;
            _groupsService = groupsService;
            _universitiesService = universitiesService;
            _mapper = mapper;
            _groupsHubContext = groupsHubContext;
            _usersService = usersService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new CreateGroupModel();
            model.Universities = _universitiesService.GetUserUniversities(userId)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

            return PartialView("_CreatePartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateGroupModel model)
        {
            if (model == null)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                return Ok(Json(ModelState
                    .Where(s => s.Value.Errors.Any())
                    .Select(s => new { Key = s.Key, Error = s.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault() })));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int? universityId = null;
            string password = null;

            if (model.RequirePassword)
                password = model.Password;
            if (model.RequireUniversity)
            {
                universityId = model.UniversityId;

                if (!_universitiesService.IsMember(model.UniversityId.Value, userId))
                    return BadRequest(Json("You are not a member of a selected university"));
            }

            _groupsService.CreateGroup(userId, model.Name, universityId, password);

            return Ok(Json(new { Redirect = Url.Action("Index", "Home") }));
        }

        public IActionResult Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_groupsService.IsMember(id, userId))
            {
                var user = _usersService.GetUser(userId);
                ViewBag.Name = user.FirstName + " " + user.LastName;
                ViewBag.UserId = userId;
                var model = _mapper.Map<DetailsGroupModel>(_groupsService.GetGroupWithGroupedTopics(userId, id));

                _groupsService.UpdateLastVisitedGroup(userId, id);

                return View(model);
            }

            var group = _mapper.Map<JoinGroupModel>(_groupsService.GetGroup(id));
            if (group != null)
                return View("Join", group);

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinGroupModel model)
        {
            if (model == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Join", model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!_groupsService.GroupExists(model.Id) || _groupsService.IsMember(model.Id, userId))
                return BadRequest();

            //Check if RequirePassword wasnt chnged by a user
            if (!model.RequirePassword && _groupsService.PasswordRequired(model.Id))
                return BadRequest();

            if(model.RequirePassword)
            {
                if (!_groupsService.CheckPassword(model.Id, model.Password))
                {
                    ModelState.AddModelError("Password", "Wrong password");
                    return View("Join", model);
                }

                _groupsService.AddUserToGroup(userId, model.Id);
                return RedirectToAction("Details", new { id = model.Id });
            }

            _groupsService.AddUserToGroup(userId, model.Id);

            return RedirectToAction("Details", new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTopic(CreateTopicModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid || !_groupsService.IsMember(model.GroupId, userId))
            {
                return BadRequest();
            }

            var topic = _mapper.Map<DetailsTopicModel>(_groupsService.CreateTopic(userId, model.GroupId, model.Name));

            await _groupsHubContext.Clients.Group(model.GroupId.ToString()).SendAsync("TopicCreated", model.GroupId, topic.Id, userId);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(CreateTopicAnswerModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var topic = _groupsService.GetTopic(model.TopicId);

            if (topic == null)
                return BadRequest();

            if (!ModelState.IsValid || !_groupsService.IsMember(topic.GroupId, userId))
            {
                return BadRequest();
            }

            var answer = _groupsService.CreateAnswer(userId, model.TopicId, model.Content);

            //_groupsService.UpdateLastVisitedTopic(userId, topic.Id);

            await _groupsHubContext.Clients.Group(topic.GroupId.ToString()).SendAsync("AnswerCreated", topic.GroupId, topic.Id, answer.Id, userId);

            return Ok();
        }
    }
}
