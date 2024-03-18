using BPOI_HUB.modules.core;
using BPOI_HUB.model.database;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BPOI_HUB.model.debug;

namespace BPOI_HUB.pages
{
    public class IndexModel : PageModel
    {


        public User user;

        public POSTGRE_DB pg;
        

        public IActionResult OnGet()
        {

            if (HttpContext.Session.Keys.Any())
            {

                user = new()
                {
                    UserName = HttpContext.Session.GetString("username")
                };


                UserManager um = new();

                um.GetUserData(ref user);


                return Page();
            }
            else
            {

                user = new User();
            }



            return null;
        }
    }
}