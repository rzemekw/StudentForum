using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudentForum.BusinessLogic;
using StudentForumAspCore.Hubs;
using StudentForumAspCore.Models;

namespace StudentForumAspCore.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly UniversitiesService _universitiesService; 
        private readonly GroupsService _groupsService;
        private readonly IMapper _mapper;


        public GroupsController(UniversitiesService universitiesService, GroupsService groupsService, IMapper mapper)
        {
            _universitiesService = universitiesService;
            _groupsService = groupsService;
            _mapper = mapper;
        }

        [HttpGet("GetTopic")]
        public DetailsTopicModel GetTopic(int id)
        {
            var dataTopic = _groupsService.GetTopic(id);
            if (dataTopic == null)
                return null;

            var groupId = dataTopic.GroupId;
            if (!_groupsService.IsMember(groupId, User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return null;

            _groupsService.UpdateLastVisitedGroup(User.FindFirstValue(ClaimTypes.NameIdentifier), groupId);
            return _mapper.Map<DetailsTopicModel>(dataTopic);
        }

        [HttpGet("GetAnswers")]
        public List<TopicAnswerModel> GetAnswers(int topicId)
        {
            var dataTopic = _groupsService.GetTopic(topicId);
            if (dataTopic == null)
                return null;

            var groupId = dataTopic.GroupId;
            if (!_groupsService.IsMember(groupId, User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return null;

            _groupsService.UpdateLastVisitedTopic(User.FindFirstValue(ClaimTypes.NameIdentifier), topicId);

            return _mapper.Map<List<TopicAnswerModel>>(_groupsService.GetTopicAnswers(topicId));
        }

        [HttpGet("GetAnswer")]
        public TopicAnswerModel GetAnswer(int id)
        {
            var answer = _groupsService.GetTopicAnswer(id);
            if (answer == null)
                return null;

            var topic = _groupsService.GetTopic(answer.TopicId);
            if (!_groupsService.IsMember(topic.GroupId, User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return null;

            _groupsService.UpdateLastVisitedTopic(User.FindFirstValue(ClaimTypes.NameIdentifier), topic.Id);

            return _mapper.Map<TopicAnswerModel>(answer);
        }

        [HttpGet("GetUsersUniversitiesWithGroups")]
        public List<MenuUniversityGroupsModel> GetUsersUniversitiesWithGroups()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var xd = _mapper.Map<List<MenuUniversityGroupsModel>>(_groupsService.GetUserUniversityGroups(userId));
            return xd;
        }

        [HttpGet("NewTopicAvailable")]
        public bool NewTopicAvailable(int groupId)
        {
            return _groupsService.NewTopicAvailable(groupId, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("GetTopicsGroupId")]
        public int? GetTopicsGroupId(int topicId)
        {
            return _groupsService.GetTopic(topicId)?.GroupId;
        }
    }
}
