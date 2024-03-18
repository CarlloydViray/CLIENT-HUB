using BPOI_HUB.model.admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;


namespace BPOI_HUB.pages.api.admin
{
    public class GetAllRolesPageModel : PageModel
    {
		public List<Role> roleList = new();
		public IActionResult OnGet()
		{
			RoleManager rm = new();

			rm.GetAllRoles(ref roleList);

			var json = JsonConvert.SerializeObject(roleList);	

            return new JsonResult(json);
		}


	
	}
}
