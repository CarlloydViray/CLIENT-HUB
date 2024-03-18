using BPOI_HUB.model.menu;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text.Json;

namespace BPOI_HUB.pages.menu
{
    public class AppsMenuModel : PageModel
    {
        public DataTable app_dt = new();

        public void OnGet()
        {
            MenuManager mm = new();

            int parent_id = int.Parse(Request.Query["pid"]);

            int[]? app_ids = JsonSerializer.Deserialize<int[]>(HttpContext.Session.GetString("allowed_app_ids"));


            if (app_ids == null)
                return;


            app_dt = mm.GetAllAppsByParentId(parent_id, app_ids);

            

        }
    }
}
