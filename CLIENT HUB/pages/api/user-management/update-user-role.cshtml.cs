using BPOI_HUB.model.admin;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BPOI_HUB.pages.api.user_management
{
    public class UpdateUserRolePageModel : PageModel
    {
        public class Input
        {
            public string? Username { get; set; }

            public string[]? Roles { get; set; }
    
        }

        public IActionResult OnPost([FromBody] Input input)
        {

            UserManager um = new();
            string output;

            try
            {

                if (um.IsSuperAdmin(input.Username))
                {
                    output = "ERROR : Cannot Update Super Admin User";
                }
                else
                {
                    um.UpdateRole(input.Username, input.Roles);


                    output = "success";
                }
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
