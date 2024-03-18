using BPOI_HUB.model.admin;
using BPOI_HUB.model.menu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BPOI_HUB.pages.api.menu
{
    public class GetMainMenuPageModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMainMenuPageModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {

             JsonSerializer.Deserialize<string[]>(_httpContextAccessor.HttpContext.Session.GetString("allowed_apps_id"));

        }


    }
}
