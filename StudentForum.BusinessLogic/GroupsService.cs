using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using StudentForum.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentForum.BusinessLogic
{
    public class GroupsService
    {
        private readonly ForumContext _context;

        public GroupsService(ForumContext context)
        {
            _context = context;
        }

        public List<Group> GetUsersGroups(string userId)
        {
            return _context.UserGroups.Where(ug => ug.UserId == userId).Select(g => g.Group).ToList();
        }

        public List<int> GetUsersGroupsIds(string userId)
        {
            return _context.UserGroups.Where(ug => ug.UserId == userId).Select(g => g.GroupId).ToList();
        }

        public Group GetGroup(int id)
        {
            return _context.Groups.Where(g => g.Id == id).FirstOrDefault();
        }

        public Group GetGroupWithTopics(int id)
        {
            var group = _context.Groups.Where(g => g.Id == id).FirstOrDefault();

            if (group == null)
                return null;

            var topics = _context.Topics.Where(t => t.GroupId == id).Include(t => t.Author).OrderByDescending(t => t.Date).ToList();
            group.Topics = topics;
            return group;
        }

        public GroupWithGroupedTopics GetGroupWithGroupedTopics(string userId, int groupId)
        {
            var userGroup = _context.UserGroups
                .Where(ug => ug.UserId == userId && ug.GroupId == groupId)
                .Include(ug => ug.Group).FirstOrDefault();
            if (userGroup == null)
                return null;

            var unseenTopics = _context.Topics
                .Where(t => t.GroupId == groupId && t.Date > userGroup.LastVisited)
                .OrderByDescending(t => t.Date)
                .Include(t => t.Author)
                .ToList();

            var otherTopicsQuery = from t in _context.Topics.Include(t => t.Author)
                                   join utd in _context.UserTopicDates.Where(u => u.UserId == userId) on t.Id equals utd.TopicId into utd
                                   from subUtd in utd.DefaultIfEmpty()
                                   where t.GroupId == groupId
                                   where t.Date <= userGroup.LastVisited
                                   orderby t.Date descending
                                   select new
                                   {
                                       Topic = t,
                                       LastVisited = subUtd.LastVisited,
                                       LastUpdated = _context.Answers.Where(a => a.TopicId == t.Id).Select(a => a.Date).Max()
                                   };

            var otherTopics = otherTopicsQuery.AsEnumerable();

            var topicsWithSeenAnswers = otherTopics.Where(o => o.LastVisited >= o.LastUpdated).Select(t => t.Topic).ToList();
            var topicsWithUnSeenAnswers = otherTopics.Where(o => o.LastVisited < o.LastUpdated).Select(t => t.Topic).ToList();


            return new GroupWithGroupedTopics
            {
                Id = userGroup.GroupId,
                Name = userGroup.Group.Name,
                UnseenTopics = unseenTopics,
                TopicsWithNewAnswers = topicsWithUnSeenAnswers,
                OldTopics = topicsWithSeenAnswers
            };
        }

        public List<Topic> GetUnseenTopics(string userId)
        {
            var unseenTopics = from t in _context.Topics
                               join ug in _context.UserGroups on t.GroupId equals ug.GroupId
                               where ug.UserId == userId
                               where t.Date > ug.LastVisited
                               orderby t.Date descending
                               select t;

            return unseenTopics.Include(t => t.Group).Include(t => t.Author).ToList();
        }

        public List<Topic> GetSeenTopicsWithNewAnswers(string userId)
        {
            var result = from t in _context.Topics
                         join ug in _context.UserGroups on t.GroupId equals ug.GroupId
                         join utd in _context.UserTopicDates.Where(u => u.UserId == userId) on t.Id equals utd.TopicId into utd
                         from subUtd in utd.DefaultIfEmpty()
                         where ug.UserId == userId
                         where t.Date <= ug.LastVisited
                         where _context.Answers.Where(a => a.TopicId == t.Id).Max(a => a.Date) > subUtd.LastVisited
                         orderby t.Date descending
                         select t;

            return result.Include(t => t.Group).Include(t => t.Author).ToList();
        }

        public void CreateGroup(string adminId, string name, int? universityId, string password)
        {
            var hasher = new PasswordHasher();
            Group group;

            group = new Group
            {
                AdminId = adminId,
                Name = name,
                UniversityId = universityId
            };


            group.UserGroups = new List<UserGroup>
            {
                new UserGroup
                {
                    UserId = adminId,
                    Group = group
                }
            };

            if (password != null)
            {
                group.PasswordSalt = hasher.CreateSalt();
                group.PasswordHash = hasher.CreatePassworHash(password, group.PasswordSalt);
            }

            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        public bool CheckPassword(int groupId, string password)
        {
            var group = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault();
            if (group == null)
                return false;

            var passwordHash = group.PasswordHash;
            var passwordSalt = group.PasswordSalt;

            if (passwordHash == null)
                return true;

            return new PasswordHasher().ComparePassword(password, passwordHash, passwordSalt);
        }

        public void AddUserToGroup(string userId, int groupId)
        {
            var uId = _context.Groups.Where(g => g.Id == groupId).Select(g => g.UniversityId).FirstOrDefault();

            if(uId.HasValue)
            {
                if(!_context.UserUniversities.Where(uu => uu.UniversityId == uId.Value && uu.UserId == userId).Any())
                {
                    _context.UserUniversities.Add(new UserUniversity { UserId = userId, UniversityId = uId.Value });
                }
            }

            var userGroup = new UserGroup { GroupId = groupId, UserId = userId, LastVisited = DateTime.Now };
            _context.UserGroups.Add(userGroup);
            _context.UserTopicDates
                .AddRange(_context.Topics.Where(t => t.GroupId == groupId)
                .Select(t => new UserTopicDate { TopicId = t.Id, UserId = userId, LastVisited = DateTime.Now }));

            _context.SaveChanges();
        }

        public void RemoveUserFromGroup(string userId, int groupId)
        {
            var group = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault();
            if (group == null)
                return;
            if (group.AdminId == userId)
                return;

            var userGroup = _context.UserGroups.Where(ug => ug.UserId == userId && ug.GroupId == groupId).FirstOrDefault();
            if (userGroup != null)
            {
                _context.UserGroups.Remove(userGroup);

                var utds = from utd in _context.UserTopicDates
                           join t in _context.Topics.Where(t => t.GroupId == groupId) on utd.TopicId equals t.Id
                           where utd.UserId == userId
                           select utd;

                _context.UserTopicDates.RemoveRange(utds);

                _context.SaveChanges();
            }
        }

        public Topic CreateTopic(string userId, int groupId, string name)
        {
            var topic = new Topic
            {
                UserId = userId,
                Date = DateTime.Now,
                GroupId = groupId,
                Name = name
            };
            _context.Topics.Add(topic);
            _context.SaveChanges();

            return topic;
        }

        public Topic GetTopic(int id)
        {
            return _context.Topics.Where(t => t.Id == id).Include(t => t.Author).Include(t => t.Group).FirstOrDefault();
        }

        public List<TopicAnswer> GetTopicAnswers(int topicId)
        {
            return _context.Answers.Where(a => a.TopicId == topicId).OrderByDescending(a => a.Date).ToList();
        }

        public TopicAnswer CreateAnswer(string userId, int topicId, string content)
        {
            var answer = new TopicAnswer
            {
                UserId = userId,
                Date = DateTime.Now,
                Content = content,
                TopicId = topicId
            };
            _context.Answers.Add(answer);
            _context.SaveChanges();

            return answer;
        }

        public TopicAnswer GetTopicAnswer(int id)
        {
            return _context.Answers.Where(a => a.Id == id).FirstOrDefault();
        }

        public void UpdateLastVisitedTopic(string userId, int topicId)
        {
            var utDate = _context.UserTopicDates.Where(utd => utd.UserId == userId && utd.TopicId == topicId).FirstOrDefault();
            if (utDate == null)
            {
                utDate = new UserTopicDate
                {
                    UserId = userId,
                    TopicId = topicId
                };
                _context.UserTopicDates.Add(utDate);
            }

            utDate.LastVisited = DateTime.Now;
            _context.SaveChanges();
        }

        public void UpdateLastVisitedGroup(string userId, int groupId)
        {
            var userGroup = _context.UserGroups.Where(ug => ug.UserId == userId && ug.GroupId == groupId).FirstOrDefault();
            if (userGroup == null)
                return;

            userGroup.LastVisited = DateTime.Now;
            _context.SaveChanges();
        }

        public List<UniversityGroups> GetUserUniversityGroups(string userId)
        {
            var universityGroupsQuery = from uu in _context.UserUniversities
                                        join u in _context.Universities on uu.UniversityId equals u.Id
                                        join g in
                                        (
                                            from g in _context.Groups
                                            join ug in _context.UserGroups on g.Id equals ug.GroupId
                                            where ug.UserId == userId
                                            select new
                                            {
                                                Group = g,
                                                NewTopicAvailable = _context.Topics.Where(t => t.GroupId == g.Id).Max(t => t.Date) > ug.LastVisited,
                                                NewAnswerAvailable = (from t in _context.Topics
                                                                      join utd in _context.UserTopicDates on t.Id equals utd.TopicId
                                                                      where t.GroupId == g.Id
                                                                      where utd.UserId == userId
                                                                      where utd.LastVisited < _context.Answers.Where(a => a.TopicId == t.Id).Max(a => a.Date)
                                                                      select t)
                                                                     .Any()
                                            }
                                        ) on u.Id equals g.Group.UniversityId into groups
                                        from g in groups.DefaultIfEmpty()
                                        where uu.UserId == userId
                                        select new { u, g.Group, g.NewAnswerAvailable, g.NewTopicAvailable };

            var universityGroups = universityGroupsQuery.AsEnumerable().GroupBy(ug => ug.u)
                .Select(ug => new UniversityGroups
                {
                    UniversityId = ug.Key.Id,
                    UniversityName = ug.Key.Name,
                    Groups = ug.SkipWhile(ugg => ugg.Group == null)
                    .Select(ugg => new GroupWithNotifications
                    {
                        Id = ugg.Group.Id,
                        Name = ugg.Group.Name,
                        NewAnswerAvailable = ugg.NewAnswerAvailable,
                        NewTopicAvailable = ugg.NewTopicAvailable
                    }).ToList()
                })
                .ToList();

            var otherGroupsQuery = from g in _context.Groups
                                   join ug in _context.UserGroups on g.Id equals ug.GroupId
                                   where ug.UserId == userId
                                   where g.UniversityId == null
                                   select new
                                   {
                                       Group = g,
                                       NewTopicAvailable = _context.Topics.Where(t => t.GroupId == g.Id).Max(t => t.Date) > ug.LastVisited,
                                       NewAnswerAvailable = (from t in _context.Topics
                                                             join utd in _context.UserTopicDates on t.Id equals utd.TopicId
                                                             where t.GroupId == g.Id
                                                             where utd.UserId == userId
                                                             where utd.LastVisited < _context.Answers.Where(a => a.TopicId == t.Id).Max(a => a.Date)
                                                             select t)
                                                            .Any()
                                   };

            universityGroups.Add(new UniversityGroups
            {
                Groups = otherGroupsQuery.AsEnumerable().Select(g => new GroupWithNotifications
                {
                    Id = g.Group.Id,
                    Name = g.Group.Name,
                    NewAnswerAvailable = g.NewAnswerAvailable,
                    NewTopicAvailable = g.NewTopicAvailable
                }).ToList()
            });

            return universityGroups;
        }

        /// <summary>
        /// Indicates that User of userId is a member of Group of groupId 
        /// </summary>
        public bool IsMember(int groupId, string userId)
        {
            return _context.UserGroups.Where(ug => ug.UserId == userId && ug.GroupId == groupId).Any();
        }

        public bool PasswordRequired(int groupId)
        {
            return _context.Groups.Where(g => g.Id == groupId && g.PasswordHash != null).Any();
        }

        public bool GroupExists(int groupId)
        {
            return _context.Groups.Where(g => g.Id == groupId).Any();
        }

        public bool NewTopicAvailable(int groupId, string userId)
        {
            var query = from ug in _context.UserGroups
                        where ug.UserId == userId && ug.GroupId == groupId
                        where _context.Topics.Where(t => t.GroupId == groupId).Max(t => t.Date) > ug.LastVisited
                        select ug;

            return query.Any();
        }

        public bool NewAnswerAvailable(int groupId, string userId)
        {
            var query = from ug in _context.UserGroups
                        join t in _context.Topics on ug.GroupId equals t.GroupId
                        join utd in _context.UserTopicDates on t.Id equals utd.TopicId
                        where ug.UserId == userId && ug.GroupId == groupId
                        where utd.UserId == userId
                        where utd.LastVisited < _context.Answers.Where(a => a.TopicId == t.Id).Max(a => a.Date)
                        select t;

            return query.Any();
        }
    }
}
