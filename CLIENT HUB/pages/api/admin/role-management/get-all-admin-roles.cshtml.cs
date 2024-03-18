using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.admin.role_management
{
    public class GetAllAdminRolesModel : PageModel
    {
        public List<Role> roleList = new();
        public IActionResult OnGet()
        {
            RoleManager rm = new();

            rm.GetAllAdminRoles(ref roleList);

            var json = JsonConvert.SerializeObject(roleList);

            return new JsonResult(json);
        }
    }
}
