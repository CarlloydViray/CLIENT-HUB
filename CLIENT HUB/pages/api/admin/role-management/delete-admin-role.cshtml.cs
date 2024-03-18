using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPOI_HUB.pages.api.admin.role_management
{
    [Consumes("application/json")]
    public class DeleteAdminRoleModel : PageModel
    {
        public class Role
        {
            public int Id { get; set; }
        }

        public IActionResult OnPost([FromBody] Role role)
        {

            RoleManager rm = new();
            string output;

            try
            {

                if (rm.IsSuperAdmin(role.Id))
                {
                    output = "ERROR : Permission Denied. Cannot Delete Super Admin Role.";
                }
                else
                {
                    rm.DeleteRole(role.Id);

                    output = "success";
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }

            var data = new { message = output };

            return new JsonResult(data);

        }
    }
}
