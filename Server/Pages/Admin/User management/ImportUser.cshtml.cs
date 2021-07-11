using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using StoryForce.Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StoryForce.Server.Pages.Admin
{
    public class ImportUserModel : PageModel
    {
        private readonly IPeopleService _peopleService;

        public ImportUserModel(IPeopleService peopleService)
        {
            this._peopleService = peopleService;
        }

        public IList<ListCellNull> ListErr { get; set; }
        public async Task<ActionResult> OnPostAsync(IFormFile file)
        {
            var listNull = new List<ListCellNull>();
            var list = new List<StoryForce.Shared.Models.Person>();
            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;
                        for (int row = 2; row <= rowcount; row++)
                        {
                            for (int col = 1; col <= colCount; col++)
                            {
                                if (worksheet.Cells[row, col].Value == null)
                                {
                                    listNull.Add(new ListCellNull
                                    {
                                        Value = "Row: " + row + " column: " + col + " has null value!"
                                    });
                                }
                            }
                        }

                        if (listNull.Count > 0)
                        {
                            ListErr = listNull;
                            return Page();
                        }
                        else
                        {
                            for (int row = 2; row <= rowcount; row++)
                            {
                                list.Add(new StoryForce.Shared.Models.Person
                                {
                                    Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                    Email = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                    ClassOfYear = Int32.Parse(worksheet.Cells[row, 3].Value.ToString()),
                                    PhoneNumber = worksheet.Cells[row, 4].Value.ToString().Trim()                                   
                                });
                            }
                            foreach (var item in list)
                            {
                                var per = await _peopleService.GetByEmailAsync(item.Email);
                                if (per != null)
                                {
                                    per.Name = item.Name;
                                    per.Email = item.Email;
                                    per.ClassOfYear = item.ClassOfYear;
                                    per.PhoneNumber = item.PhoneNumber;                                    
                                    await _peopleService.UpdateAsync(per.Id, per);
                                }
                                else
                                {
                                    await _peopleService.CreateAsync(item);
                                }
                            }
                        }
                        return Redirect("/usermanagement");
                    }
                }
            }
            return Page();
        }

        public class ListCellNull
        {
            public string Value { get; set; }
        }
    }
}
