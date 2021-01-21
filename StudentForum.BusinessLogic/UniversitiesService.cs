using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentForum.BusinessLogic
{
    public class UniversitiesService
    {
        private readonly ForumContext _context;

        public UniversitiesService(ForumContext context)
        {
            _context = context;
        }

        public void CreateUniversity(string countryId, string name, string website)
        {
            _context.Universities.Add(
                new University
                {
                    CountryId = countryId,
                    Name = name,
                    Website = website
                });
            _context.SaveChanges();
        }

        public void AddUserToUniversity(string userId, int universityId)
        {
            var userUniversity = new UserUniversity { UniversityId = universityId, UserId = userId };
            _context.UserUniversities.Add(userUniversity);
            _context.SaveChanges();
        }

        public List<University> GetCountryUniversities(string countryId)
        {
            return _context.Universities
                .Where(u => u.CountryId == countryId)
                .ToList();
        }

        public List<University> GetUserUniversities(string userId)
        {
            return _context.UserUniversities
                .Where(u => u.UserId == userId)
                .Select(u => u.University)
                .ToList();
        }

        public University GetUniversity(string website)
        {
            return _context.Universities
                .Where(u => u.Website == website)
                .FirstOrDefault();
        }

        public University GetUniversity(int id)
        {
            return _context.Universities
                .Where(u => u.Id == id)
                .FirstOrDefault();
        }


        /// <summary>
        /// Indicates that User of userId is a member of University of universityId 
        /// </summary>
        public bool IsMember(int universityId, string userId)
        {
            return _context.UserUniversities.Where(uu => uu.UserId == userId && uu.UniversityId == universityId).Any();
        }
    }
}
