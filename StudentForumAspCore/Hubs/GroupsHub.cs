using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudentForum.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentForumAspCore.Hubs
{
    [Authorize]
    public class GroupsHub: Hub
    {
        private readonly GroupsService _groupsService;

        public GroupsHub(GroupsService groupsService)
        {
            _groupsService = groupsService;
        }

        public override Task OnConnectedAsync()
        {
            var ids = _groupsService.GetUsersGroupsIds(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Task.WaitAll(ids.Select(id => Groups.AddToGroupAsync(Context.ConnectionId, id.ToString())).ToArray());
            return base.OnConnectedAsync();
        }
    }
}
