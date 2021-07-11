using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StoryForce.Server.Services;
using StoryForce.Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IStoryFileService _storyFileService;
        private readonly ISubmissionService _submissionService;

        public NoteController(INoteService noteService, IStoryFileService storyFileService, ISubmissionService submissionService)
        {
            _noteService = noteService;
            this._storyFileService = storyFileService;
            this._submissionService = submissionService;
        }

        // GET: api/<Note>
        [HttpGet]
        public async Task<List<Note>> Get()
        {
            return await _noteService.GetAsync();
        }

        // GET api/<Note>/5
        [HttpGet("{id}", Name = "GetNote")]
        public async Task<ActionResult<Note>> Get(int id)
        {
            return await _noteService.GetAsync(id);
        }

        // POST api/<Note>
        [HttpPost]
        public async Task<ActionResult> Post(CreateNoteDto note)
        {
            var createdNote = new Note();
            if (note.Text == null)
            {
                return BadRequest($"Text null");
            }
            else
            {
                createdNote = await _noteService.CreateAsync(note.ToEntity());
            }            
            if (note.SubmissionId != 0)
            {
                var submission = await _submissionService.GetAsync(note.SubmissionId);
                if (submission == null)
                    return BadRequest($"Submission with id '{note.SubmissionId}' not found.");
                submission.NoteFile.Add(createdNote);
                await _submissionService.UpdateAsync(submission.Id, submission);
            }
            else 
            {
                var storyFile = await _storyFileService.GetAsync(note.StoryFileId);
                if (storyFile == null)
                    return BadRequest($"Story file with id '{note.StoryFileId}' not found.");
                storyFile.Notes.Add(createdNote);
                await _storyFileService.UpdateAsync(storyFile.Id, storyFile);
            }
           
            return CreatedAtRoute("GetNote", new { id = createdNote.Id }, createdNote);
        }

        // PUT api/<Note>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, EditNoteDto note)
        {
            var noteInDb = await _noteService.GetAsync(id);
            if (noteInDb == null || note.Id != id)
                return BadRequest($"Note with id '{id}' not found.");
            noteInDb.Text = note.Text;
            await _noteService.UpdateAsync(id, noteInDb);
            return NoContent();
        }

        // DELETE api/<Note>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var noteInDb = await _noteService.GetAsync(id);
            if (noteInDb == null)
                return BadRequest($"Note with id '{id}' not found.");
            await _noteService.RemoveAsync(id);
            return NoContent();
        }
    }

    public class CreateNoteDto
    {
        public int SubmissionId { get; set; }

        public int StoryFileId { get; set; }

        public string Text { get; set; }

        public string Username { get; set; }

        public Note ToEntity()
        {
            return new Note()
            {
                Text = this.Text,
                UserName = this.Username,
                CreatedAt = DateTime.Now,                
            };
        }
    }

    public class EditNoteDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        
    }
}
