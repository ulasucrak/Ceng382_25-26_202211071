using System.Text.Json;
using System.Reflection;

namespace ClassInfoRazorPages.Helpers
{
    public class Utils
    {
        private static Utils? _instance;
        public static Utils Instance => _instance ??= new Utils();

        private Utils() { }

        public string ExportToJson<T>(List<T> data, List<string> selectedColumns)
        {
            if (selectedColumns == null || selectedColumns.Count == 0)
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var filteredList = data.Select(item =>
            {
                var dict = new Dictionary<string, object?>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (selectedColumns.Contains(prop.Name))
                        dict[prop.Name] = prop.GetValue(item);
                }

                return dict;
            }).ToList();

            return JsonSerializer.Serialize(filteredList, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
