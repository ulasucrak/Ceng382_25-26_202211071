using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassInfoRazorPages.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClassInfoRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> classList = new List<ClassInformationModel>();
        private static int nextId = 1;

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new ClassInformationModel();

        [BindProperty(SupportsGet = true)]
        public string? FilterKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<ClassInformationTable> ClassTable { get; set; } = new List<ClassInformationTable>();

        public void OnGet()
        {
            // Veri üret (ilk çalıştırmada)
            if (!classList.Any())
            {
                for (int i = 0; i < 100; i++)
                {
                    classList.Add(new ClassInformationModel
                    {
                        Id = nextId++,
                        ClassName = $"Class {i + 1}",
                        StudentCount = 10 + i % 20,
                        Description = $"This is class {i + 1}"
                    });
                }
            }

            var filtered = classList.AsQueryable();

            if (!string.IsNullOrEmpty(FilterKeyword))
            {
                filtered = filtered.Where(x => x.ClassName != null && x.ClassName.Contains(FilterKeyword));
            }

            TotalPages = (int)System.Math.Ceiling(filtered.Count() / (double)PageSize);

            ClassTable = filtered
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new ClassInformationTable
                {
                    ClassName = x.ClassName,
                    StudentCount = x.StudentCount,
                    Description = x.Description,
                    Id = x.Id
                })
                .ToList();
        }

        public IActionResult OnPostSave()
        {
            if (!ModelState.IsValid)
                return Page();

            if (ClassInfo.Id == 0)
            {
                ClassInfo.Id = nextId++;
                classList.Add(ClassInfo);
            }
            else
            {
                var itemToUpdate = classList.FirstOrDefault(x => x.Id == ClassInfo.Id);
                if (itemToUpdate != null)
                {
                    itemToUpdate.ClassName = ClassInfo.ClassName;
                    itemToUpdate.StudentCount = ClassInfo.StudentCount;
                    itemToUpdate.Description = ClassInfo.Description;
                }
            }

            return RedirectToPage(new { FilterKeyword, PageNumber });
        }

        public IActionResult OnPostEdit(int id)
        {
            var itemToEdit = classList.FirstOrDefault(x => x.Id == id);
            if (itemToEdit != null)
            {
                ClassInfo = new ClassInformationModel
                {
                    Id = itemToEdit.Id,
                    ClassName = itemToEdit.ClassName,
                    StudentCount = itemToEdit.StudentCount,
                    Description = itemToEdit.Description
                };
            }

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var itemToDelete = classList.FirstOrDefault(x => x.Id == id);
            if (itemToDelete != null)
            {
                classList.Remove(itemToDelete);
            }

            return RedirectToPage(new { FilterKeyword, PageNumber });
        }
    }
}
