using BPOI_HUB.model.menu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.menu
{
    public class GetAllAppsPageModel : PageModel
    {
        public List<MenuItem> appList = new();
        public IActionResult OnGet()
        {

            MenuManager mm = new();

            mm.GetAllApps(ref appList, null, new string[] { "\"MenuId\"", "\"Name\"" });

            var json = JsonConvert.SerializeObject(appList);

            return new JsonResult(json);


        }
    }
}
