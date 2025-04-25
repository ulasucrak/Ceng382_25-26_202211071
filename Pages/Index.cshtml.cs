using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassInfoRazorPages.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClassInfoRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> _classes = new();
        private static int nextId = 1;

        [BindProperty]
        public InputModel ClassInput { get; set; } = new();

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new(); // 🔥 New binding!

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool IsEdit => ClassInput.Id > 0;

        static IndexModel()
        {
            if (!_classes.Any())
            {
                for (int i = 1; i <= 100; i++)
                {
                    _classes.Add(new ClassInformationModel
                    {
                        Id = nextId++,
                        ClassName = $"Class {i}",
                        StudentCount = i % 30 + 1,
                        Description = $"Description for Class {i}"
                    });
                }
            }
        }

        public void OnGet(int? editId, int? deleteId)
        {
            var sessionUser = HttpContext.Session.GetString("username");
    var sessionToken = HttpContext.Session.GetString("token");

    var cookieUser = Request.Cookies["username"];
    var cookieToken = Request.Cookies["token"];

    if (string.IsNullOrEmpty(sessionUser) || 
        string.IsNullOrEmpty(cookieUser) || 
        sessionUser != cookieUser || 
        sessionToken != cookieToken)
    {
        Response.Redirect("/Login");
        return;
    }
            if (deleteId.HasValue)
            {
                var toRemove = _classes.FirstOrDefault(c => c.Id == deleteId);
                if (toRemove != null)
                    _classes.Remove(toRemove);
                ClassInput = new InputModel(); // Form reset
            }
            else if (editId.HasValue)
            {
                var toEdit = _classes.FirstOrDefault(c => c.Id == editId);
                if (toEdit != null)
                {
                    ClassInput = new InputModel
                    {
                        Id = toEdit.Id,
                        ClassName = toEdit.ClassName,
                        StudentCount = toEdit.StudentCount,
                        Description = toEdit.Description
                    };
                }
            }
            else
            {
                ClassInput = new InputModel();
            }

            if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = new List<string> { "ClassName", "StudentCount", "Description" }; // 🔥 default selected
            }

            var query = _classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchClassName))
                query = query.Where(c => c.ClassName != null && c.ClassName.Contains(SearchClassName));

            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            FilteredClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (ClassInput.Id > 0)
            {
                var toUpdate = _classes.FirstOrDefault(c => c.Id == ClassInput.Id);
                if (toUpdate != null)
                {
                    toUpdate.ClassName = ClassInput.ClassName;
                    toUpdate.StudentCount = ClassInput.StudentCount;
                    toUpdate.Description = ClassInput.Description;
                }
            }
            else
            {
                _classes.Add(new ClassInformationModel
                {
                    Id = nextId++,
                    ClassName = ClassInput.ClassName,
                    StudentCount = ClassInput.StudentCount,
                    Description = ClassInput.Description
                });
            }

            return RedirectToPage(new { PageNumber, SearchClassName, SelectedColumns });
        }

        public IActionResult OnPostExportJson(string SearchClassName, int PageNumber, List<string> selectedColumns)
        {
            if (selectedColumns == null || !selectedColumns.Any())
                selectedColumns = new List<string> { "ClassName", "StudentCount", "Description" };

            var query = _classes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchClassName))
                query = query.Where(c => c.ClassName != null && c.ClassName.Contains(SearchClassName));

            query = query.Skip((PageNumber - 1) * PageSize)
                         .Take(PageSize);

            var exportData = query
                .Select(c => new Dictionary<string, object>
                {
                    { "ClassName", selectedColumns.Contains("ClassName") && c.ClassName != null ? c.ClassName : null },
                    { "StudentCount", selectedColumns.Contains("StudentCount") && c.StudentCount > 0 ? c.StudentCount : null },
                    { "Description", selectedColumns.Contains("Description") && c.Description != null ? c.Description : null }
                })
                .Select(dict => dict
                    .Where(kv => kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value))
                .ToList();

            var json = JsonConvert.SerializeObject(exportData, Formatting.Indented);

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "export.json");
        }

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Class Name is required")]
            public string ClassName { get; set; }

            [Range(1, 100, ErrorMessage = "Student count must be between 1 and 100")]
            public int StudentCount { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }
        }
    }
}
