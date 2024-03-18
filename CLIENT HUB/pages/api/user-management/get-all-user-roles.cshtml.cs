using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.user_management
{
    public class GetAllUserRolePageModel : PageModel
    {
        public List<User> userList = new();
        public IActionResult OnGet()
        {
            UserManager um = new();

            um.GetAllUserRoles(ref userList);

            var json = JsonConvert.SerializeObject(userList);

            return new JsonResult(json);
        }
    }
}
