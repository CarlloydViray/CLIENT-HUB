using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPOI_HUB.pages.user_management
{
    public class LogOutPageModel : PageModel
    {
        public void OnGet()
        {
            HttpContext.Session.Clear();
            RedirectToPage("/user-management/login");
        }
    }
}
