using ClassInfoRazorPages.Models;

namespace ClassInfoRazorPages.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchoolDbContext context)
        {
            if (context.Classes.Any())
                return; // Zaten veri var, tekrar eklemeye gerek yok

            var classes = new List<Class>();

            for (int i = 1; i <= 100; i++)
            {
                classes.Add(new Class
                {
                    Name = $"Class {i}",
                    PersonCount = (i % 40) + 1,
                    Description = $"Auto-generated description for Class {i}",
                    IsActive = i % 2 == 0
                });
            }

            context.Classes.AddRange(classes);
            context.SaveChanges();
        }
    }
}
