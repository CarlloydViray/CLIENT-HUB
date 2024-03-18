using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.SS.Formula.Functions;
using System.Text.Json.Nodes;

namespace BPOI_HUB.pages.api.admin
{

    [Consumes("application/json")]
    public class DeleteRoleModel : PageModel
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

                if (rm.IsAdmin(role.Id))
                {
                    output = "ERROR : Permission Denied. Cannot Delete Admin Role.";
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
