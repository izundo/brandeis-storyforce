using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryForce.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IStoryFileService _storyFileService;
        private readonly ISubmissionService _submissionService;
        public SearchController(IStoryFileService storyFileService, ISubmissionService submissionService)
        {           
            this._storyFileService = storyFileService;
            this._submissionService = submissionService;            
        }
        
        //[HttpPost]        
        //public async Task<IActionResult> Post(Search search)
        //{   
        //    //var submission = _submissionService.GetBySubmittedByInputValueAsync(search.Value);
        //    var storyFile = _storyFileService.GetByStoryFileByInputValueAsync(search.Value);           
        //    return Ok(storyFile);
        //}
        //public class Search
        //{
        //    public string Value { get; set; }
           
        //}
    }
}
