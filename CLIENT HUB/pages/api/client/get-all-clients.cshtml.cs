using BPOI_HUB.model.client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BPOI_HUB.pages.api.clients
{
    public class GetAllClientsPageModel : PageModel
    {
        
        public List<Client> clientList = new();
        public IActionResult OnGet()
        {
            ClientManager cm = new();

            cm.GetAllClientsCodeName(ref clientList);

            var json = JsonConvert.SerializeObject(clientList);

            return new JsonResult(json);
        }
    }
}
