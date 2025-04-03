using System.ComponentModel.DataAnnotations;

namespace ClassInfoRazorPages.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Student Count must be greater than 0")]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}
