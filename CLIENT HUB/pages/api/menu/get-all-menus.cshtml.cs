using BPOI_HUB.model.menu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.menus
{
    public class GetAllMenuPageModel : PageModel
    {
        public List<MenuItem> menuList = new();

        public IActionResult OnGet()
        {

            MenuManager mm = new();

            mm.GetMainMenus(ref menuList, "main menu", null, new string[] { "\"MenuId\"", "\"Name\""});

            var json = JsonConvert.SerializeObject(menuList);

            return new JsonResult(json);


        }
    }
}
