using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentForum.BusinessLogic;
using StudentForumAspCore.Models;

namespace StudentForumAspCore.Controllers.API
{
    [Route("api/universities")]
    [ApiController]
    [Authorize]
    public class UniversitiesController : ControllerBase
    {
        private readonly UniversitiesService _universitiesService;
        private readonly IMapper _mapper;

        public UniversitiesController(UniversitiesService universitiesService, IMapper mapper)
        {
            _universitiesService = universitiesService;
            _mapper = mapper;
        }

        [HttpGet]
        public List<UniversityModel> Get(string countryId)
        {
            return _mapper.Map<List<UniversityModel>>(_universitiesService.GetCountryUniversities(countryId));
        }
    }
}
