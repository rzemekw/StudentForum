using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentForum.BusinessLogic
{
    public class CountriesService
    {
        private readonly ForumContext _context;

        public CountriesService(ForumContext context)
        {
            _context = context;
        }

        public List<Country> GetCountries()
        {
            return _context.Countries.OrderBy(c=> c.Name).ToList();
        }
    }
}
