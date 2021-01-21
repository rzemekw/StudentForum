using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentForum.BusinessLogic
{
    public class UsersService
    {
        private readonly ForumContext _context;

        public UsersService(ForumContext context)
        {
            _context = context;
        }

        public void AddUser(string id, string firstName, string lastName)
        {
            _context.Users.Add(
                new User
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                });
            _context.SaveChanges();
        }

        public User GetUser(string id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }
    }
}
