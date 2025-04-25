using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClassInfoRazorPages.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();

            // Cookie'leri temizle
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            // Login sayfasına yönlendir
            return RedirectToPage("/Login");
        }
    }
}
