using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ClassInfoRazorPages.Models;
using ClassInfoRazorPages.Data;
using System.ComponentModel.DataAnnotations;

namespace ClassInfoRazorPages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel ClassInput { get; set; } = new();

        public List<Class> FilteredClasses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new();

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool IsEdit => ClassInput.Id > 0;

        public async Task<IActionResult> OnGetAsync(int? editId, int? deleteId)
        {
            // 🔒 Session & Cookie validation
            var sessionUser = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieUser = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];

            if (string.IsNullOrEmpty(sessionUser) || string.IsNullOrEmpty(cookieUser) ||
                sessionUser != cookieUser || sessionToken != cookieToken)
            {
                return RedirectToPage("/Login");
            }

            // 🗑️ SOFT DELETE
            if (deleteId.HasValue)
            {
                var toSoftDelete = await _context.Classes.FindAsync(deleteId.Value);
                if (toSoftDelete != null)
                {
                    toSoftDelete.IsActive = false;
                    await _context.SaveChangesAsync();
                }
                ClassInput = new InputModel(); // Reset form
            }
            // ✏️ EDIT
            else if (editId.HasValue)
            {
                var toEdit = await _context.Classes.FindAsync(editId.Value);
                if (toEdit != null)
                {
                    ClassInput = new InputModel
                    {
                        Id = toEdit.Id,
                        ClassName = toEdit.Name,
                        StudentCount = toEdit.PersonCount,
                        Description = toEdit.Description
                    };
                }
            }

            if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = new List<string> { "ClassName", "StudentCount", "Description" };
            }

            // 🔍 FILTER + HIDE INACTIVE
            var query = _context.Classes
                .Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(SearchClassName))
                query = query.Where(c => c.Name.Contains(SearchClassName));

            // 📄 PAGINATION
            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            FilteredClasses = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (ClassInput.Id > 0)
            {
                var toUpdate = await _context.Classes.FindAsync(ClassInput.Id);
                if (toUpdate != null)
                {
                    toUpdate.Name = ClassInput.ClassName;
                    toUpdate.PersonCount = ClassInput.StudentCount;
                    toUpdate.Description = ClassInput.Description;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var newClass = new Class
                {
                    Name = ClassInput.ClassName,
                    PersonCount = ClassInput.StudentCount,
                    Description = ClassInput.Description,
                    IsActive = true
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageNumber, SearchClassName, SelectedColumns });
        }

        public async Task<IActionResult> OnPostExportJsonAsync(string SearchClassName, int PageNumber, List<string> selectedColumns)
        {
            if (selectedColumns == null || !selectedColumns.Any())
                selectedColumns = new List<string> { "ClassName", "StudentCount", "Description" };

            var query = _context.Classes
                .Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(SearchClassName))
                query = query.Where(c => c.Name.Contains(SearchClassName));

            query = query.Skip((PageNumber - 1) * PageSize)
                         .Take(PageSize);

            var exportData = await query
                .Select(c => new Dictionary<string, object>
                {
                    { "ClassName", selectedColumns.Contains("ClassName") ? c.Name : null },
                    { "StudentCount", selectedColumns.Contains("StudentCount") ? c.PersonCount : null },
                    { "Description", selectedColumns.Contains("Description") ? c.Description : null }
                })
                .Select(dict => dict
                    .Where(kv => kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value))
                .ToListAsync();

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
