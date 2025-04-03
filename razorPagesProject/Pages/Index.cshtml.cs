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

        public List<ClassInformationModel> ClassList => classList;

        public void OnGet()
        {
            // Nothing to do here
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

            return RedirectToPage();
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

            return RedirectToPage();
        }
    }
}
