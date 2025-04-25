using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassInfoRazorPages.Models;
using System.Text.Json;

namespace ClassInfoRazorPages.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public IActionResult OnPost()
{
    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
    if (!System.IO.File.Exists(filePath))
    {
        ErrorMessage = "User data file not found.";
        return Page();
    }

    var json = System.IO.File.ReadAllText(filePath);
    var users = JsonSerializer.Deserialize<List<User>>(json);

    var user = users?.FirstOrDefault(u =>
        u.Username == Input.Username &&
        u.Password == Input.Password &&
        u.IsActive);

    if (user == null)
    {
        ErrorMessage = "Incorrect username or password.";
        return Page();
    }

    string token = Guid.NewGuid().ToString();
    HttpContext.Session.SetString("username", user.Username);
    HttpContext.Session.SetString("token", token);
    // ❗ SessionId artık yazmıyoruz! (Burası hatalıydı)
    // HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

    CookieOptions options = new()
    {
        Expires = DateTimeOffset.Now.AddMinutes(30),
        HttpOnly = true,
        Secure = false, // Localhostta test için Secure'ı false yap (önemli)
        SameSite = SameSiteMode.Strict
    };
    Response.Cookies.Append("username", user.Username, options);
    Response.Cookies.Append("token", token, options);
    // ❗ session_id cookie'si artık set edilmiyor!

    return Redirect("/Index");
}

    }
}
