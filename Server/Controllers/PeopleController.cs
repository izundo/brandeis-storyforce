using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPeopleService _peopleService;
        private readonly IStoryFileService _storyFileService;
        private readonly ISubmissionService _submissionService;

        public PeopleController(IPeopleService peopleService, IStoryFileService storyFileService, ISubmissionService submissionService)
        {
            this._peopleService = peopleService;
            this._storyFileService = storyFileService;
            this._submissionService = submissionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            return await this._peopleService.GetAsync();
        }

        private async Task Seed()
        {
            /*
             *  Chloe Lewis	2025
                Claire Banks	2020
                Naomi Lewis	2023
                Gabby Lewis 	2029
                Dash Rennie	2025
                Siena Rennie	2022
                Yohan Hong	2020
                Greg Franklin-Sparks	2025
                Kyle Penczak	2022
                Greg Goldstein	2022
                Jake Horowitz	2021
                Quinn Horowitz	2029
                Gavin Horowitz 	2024
                Maya Kux	2022
                Lyla Kux	2027
                Charley Goldstein	2020
                Isaac Lendl	2025
                Lev Khersonsky	2029
                Zara Khersonsky	2027
                Micah Traeger Hirschfelder	2022
                Elias Traeger Hirschfelder	2026
                Jacob Goldberg	2022
                Sydelle Goldberg	2025

             */
            var people = new List<Person>
            {
                new Person
                {
                    Name = "Nataan Hong",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "Ezra Rosen",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "Aviv Shakked",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "David Flores",
                    ClassOfYear = 2021
                }
            };

            await this._peopleService.CreateMultipleAsync(people);
        }        

        // DELETE api/<People>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var peopleInDb = await _peopleService.GetAsync(id);
            if (peopleInDb == null)
                return BadRequest($"People with id '{id}' not found.");
            var storyFile = _storyFileService.GetBySubmittedByIdAsync(peopleInDb.Id);
            if (storyFile != null)
            {
                foreach (var story in await storyFile)
                {
                    await _storyFileService.RemoveAsync(story);
                }               
            }
            var submission = _submissionService.GetBySubmittedByIdAsync(peopleInDb.Id);
            if (submission != null)
            {
                foreach (var sub in await submission)
                {
                    await _submissionService.RemoveAsync(sub);
                }
            }
            await _peopleService.RemoveAsync(id);
            return NoContent();
        }

        //public async Task<ActionResult> ImportUser(IFormFile file)
        //{
        //    var list = new List<Person>();
        //    if (file != null)
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                var rowcount = worksheet.Dimension.Rows;
        //                for (int row = 2; row <= rowcount; row++)
        //                {
        //                    list.Add(new Person
        //                    {                                
        //                        Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
        //                        Email = worksheet.Cells[row, 2].Value.ToString().Trim(),
        //                        ClassOfYear = Int32.Parse(worksheet.Cells[row, 3].Value.ToString()),
        //                        PhoneNumber = worksheet.Cells[row, 4].Value.ToString().Trim(),
        //                        UserName = worksheet.Cells[row, 5].Value.ToString().Trim(),
        //                    });
        //                }
        //            }
        //        }
        //        foreach (var item in list)
        //        {
        //            var per = await _peopleService.GetByEmailAsync(item.Email);
        //            if(per != null)
        //            {
        //                per.Name = item.Name;
        //                per.Email = item.Email;
        //                per.ClassOfYear = item.ClassOfYear;
        //                per.PhoneNumber = item.PhoneNumber;
        //                per.UserName = item.UserName;
        //                await _peopleService.UpdateAsync(item.Id, per);
        //            }
        //            else
        //            {
        //                await _peopleService.CreateAsync(item);
        //            }                    
        //        }
        //    }
        //    return RedirectToAction("/usermanagement");
        //}
    }
}
