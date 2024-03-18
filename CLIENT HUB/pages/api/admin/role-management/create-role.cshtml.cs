using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPOI_HUB.pages.api.admin
{
    public class CreateRoleModel : PageModel
    {

        public class Role
        {
            public string? Name { get; set; }

            public string[]? AllowedClients { get; set; }

            public string[]? AllowedMenus { get; set; }

            public string[]? AllowedApps { get; set; }

        }

        public IActionResult OnPost([FromBody] Role role)
        {

            RoleManager rm = new();
            string output;

            try
            {
                rm.AddRole(role.Name, role.AllowedClients, role.AllowedMenus, role.AllowedApps, false);

                output = "success";
            }
            catch (Exception ex)
            {
                output = "ERROR : " + ex.Message;
            }

            var data = new { message = output };

            return new JsonResult(data);

        }
    }
}
