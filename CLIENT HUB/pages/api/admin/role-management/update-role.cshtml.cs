using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPOI_HUB.pages.api.admin
{
    public class UpdateRolePageModel : PageModel
    {
        public class Role
        {
            public int Id { get; set; }

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
                rm.UpdateRole(role.Id, role.AllowedClients, role.AllowedMenus, role.AllowedApps);

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
