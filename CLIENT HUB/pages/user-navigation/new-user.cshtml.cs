using BPOI_HUB.model.admin;
using BPOI_HUB.model.menu;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BPOI_HUB.pages.user_management
{
    public class NewUserPageModel : PageModel
    {
        public class NewUserModel
        {

            [BindProperty]
            public string? FirstName { get; set; }

            [BindProperty]
            public string? LastName { get; set; }

            [BindProperty]
            public string? MiddleName { get; set; }

            [BindProperty]
            public string? Email { get; set; }

            [BindProperty]
            public string? Department { get; set; }

        }

        public void OnGet()
        {

            UserManager um = new();

            um.IsSignedIn(HttpContext);

           
        }

        public IActionResult OnPost(NewUserModel newUserModel)
        {
            UserManager um = new();

            try
            {
                newUserModel.MiddleName ??= "";

                int result = um.RegisterUser(HttpContext.Session.GetString("username"),
                                newUserModel.FirstName.ToLower(),
                                newUserModel.LastName.ToLower(),
                                newUserModel.MiddleName.ToLower(),
                                newUserModel.Email.ToLower(),
                                newUserModel.Department.ToLower());


                if (result > 0)
                {
                    string[] allowed_clients = Array.Empty<string>();
                    int[] allowed_menus = Array.Empty<int>();
                    int[] allowed_apps = Array.Empty<int>();

                    List<string> links = new();

                    um.GetAllowedClientsMenu(HttpContext.Session.GetString("username"), ref allowed_clients, ref allowed_menus, ref allowed_apps, ref links);

                    HttpContext.Session.SetString("allowed_menu_ids", JsonSerializer.Serialize(allowed_menus));
                    HttpContext.Session.SetString("allowed_app_ids", JsonSerializer.Serialize(allowed_apps));
                    HttpContext.Session.SetString("allowed_clients", JsonSerializer.Serialize(allowed_clients));
                    HttpContext.Session.SetString("allowed_menus_apps", JsonSerializer.Serialize(links.ToArray()));


                    return RedirectToPage("/index");
                }
                else
                {
                    TempData["error_message"] = "Error Registering New User.";
                    return Page();
                }


            }
            catch(Exception ex)
            {
                TempData["error_message"] = ex.Message;
                return Page();
            }

        }

       

    }
}
