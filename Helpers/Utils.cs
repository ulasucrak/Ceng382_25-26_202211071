using Newtonsoft.Json;

namespace RazorPages.Helpers
{
    public class Utils
    {
        private static Utils _instance;

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Utils();
                }
                return _instance;
            }
        }

        public string ExportToJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}