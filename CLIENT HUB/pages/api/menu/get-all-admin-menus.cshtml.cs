using BPOI_HUB.model.menu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.menu
{
    public class GetAllAdminMenusPageModel : PageModel
    {
        public List<MenuItem> menuList = new();

        public IActionResult OnGet()
        {

            MenuManager mm = new();

            mm.GetMainAdminMenus(ref menuList, null, new string[] { "\"MenuId\"", "\"Name\"" });

            var json = JsonConvert.SerializeObject(menuList);

            return new JsonResult(json);


        }
    }
}
