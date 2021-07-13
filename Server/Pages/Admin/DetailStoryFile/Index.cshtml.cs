using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;
using StoryForce.Shared.ViewModels;

namespace StoryForce.Server.Pages.Admin
{
    [Authorize]
    public class StoryFileDetail : PageModel
    {
        private readonly IStoryFileService _storyFileService;
        private readonly IPeopleService _peopleService;
        private readonly ISubmissionService _submissionService;
        private readonly IEventService _eventService;

        public StoryFileDetail(IStoryFileService storyFileService, IPeopleService peopleService, ISubmissionService submissionService, IEventService eventService)
        {
            this._storyFileService = storyFileService;
            this._peopleService = peopleService;
            this._eventService = eventService;
            StoryFile = new();
            this._submissionService = submissionService;
        }

        [BindProperty]
        public StoryForce.Shared.Models.StoryFile StoryFile { get; set; }

        [BindProperty]
        public List<StoryForce.Shared.Models.Event> Event { get; set; }

        [BindProperty]
        public List<StoryForce.Shared.Models.Person> Staff { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            StoryFile = await this._storyFileService.GetAsync(id);
            Event = await this._eventService.GetAsync();
            var staff = await this._peopleService.GetAsync();
            Staff = staff.Where(m => m.Type == PersonType.Staff).ToList();
            return Page();
        }

        [BindProperty]
        public StoryForce.Shared.Models.StoryFile File { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Event = await this._eventService.GetAsync();
            var staff = await this._peopleService.GetAsync();
            Staff = staff.Where(m => m.Type == PersonType.Staff).ToList();
            if (File.Id > 0)
            {
                var file = await _storyFileService.GetAsync(File.Id);
                file.Description = StoryFile.Description;
                var user = await this._peopleService.GetAsync(file.SubmittedById);
                if(user != null)
                {
                    user.Name = StoryFile.SubmittedBy.Name;
                    user.Email = StoryFile.SubmittedBy.Email;
                    await _peopleService.UpdateAsync(user.Id, user);
                }
                if (StoryFile.Event.Id > 0)
                {
                    //var even = await this._eventService.GetAsync(StoryFile.Event.Id);
                    file.EventId = StoryFile.Event.Id;
                }
                else
                {
                    file.Event = StoryFile.Event;
                }

                if (StoryFile.DownloadUrl != null)
                {
                    file.DownloadUrl = StoryFile.DownloadUrl;
                }

                if (StoryFile.RequestedBy.Id > 0)
                {
                    file.RequestedById = StoryFile.RequestedBy.Id;
                }
                else
                {
                    file.RequestedById = 0;
                }
                await _storyFileService.UpdateAsync(file.Id, file);
            }
            return Page();
        }

        [BindProperty]
        public IList<StoryForce.Shared.Models.Person> RequestByPerson { get; set; }
    }
}
