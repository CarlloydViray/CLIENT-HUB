using BPOI_HUB.modules.windows;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using BPOI_HUB.model.admin;
using BPOI_HUB.model.menu;
using BPOI_HUB.model.debug;
using BPOI_HUB.pages.api.menu;
using MathNet.Numerics.Interpolation;
using NPOI.OpenXmlFormats.Dml.Chart;

namespace BPOI_HUB.pages.user_management
{
    public class UserPassModel
    {
        [BindProperty]       
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }
    }


    public class LoginModel : PageModel
    {


        public LoginModel()
        {

        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(UserPassModel userPass)
        {
            //try
            //{

            //    UserManager um = new();

            //    string[] allowed_clients = Array.Empty<string>();
            //int[] allowed_menus = Array.Empty<int>();
            //int[] allowed_apps = Array.Empty<int>();

            //List<string> links = new();

            //um.UpdateLastLogin("BYPASS ACCOUNT");
            //um.GetAllowedClientsMenu("BYPASS ACCOUNT", ref allowed_clients, ref allowed_menus, ref allowed_apps, ref links);

            //HttpContext.Session.SetString("allowed_menu_ids", JsonSerializer.Serialize(allowed_menus));
            //HttpContext.Session.SetString("allowed_app_ids", JsonSerializer.Serialize(allowed_apps));
            //HttpContext.Session.SetString("allowed_clients", JsonSerializer.Serialize(allowed_clients));
            //HttpContext.Session.SetString("allowed_menus_apps", JsonSerializer.Serialize(links.ToArray()));


            //return RedirectToPage("/Index");
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = ex.Message;
            //    return Page();
            //}
            try
            {

                if(userPass.Username == null || userPass.Password == null)
                {
                    TempData["ErrorMessage"] = "Invalid Username/Password";
                    return Page();
                }

                if (WindowsCredential.IsValid(userPass.Username, userPass.Password, "MANILA"))
                {

                    HttpContext.Session.SetString("username", userPass.Username);

                    UserManager um = new();

                    //if (um.UserExist(userPass.Username.ToLower()))
                    //{
                        string[] allowed_clients = Array.Empty<string>();
                        int[] allowed_menus = Array.Empty<int>();
                        int[] allowed_apps = Array.Empty<int>();

                        List<string> links = new();

                        um.UpdateLastLogin(userPass.Username);
                        um.GetAllowedClientsMenu(userPass.Username, ref allowed_clients, ref allowed_menus, ref allowed_apps, ref links);

                        HttpContext.Session.SetString("allowed_menu_ids", JsonSerializer.Serialize(allowed_menus));
                        HttpContext.Session.SetString("allowed_app_ids", JsonSerializer.Serialize(allowed_apps));
                        HttpContext.Session.SetString("allowed_clients", JsonSerializer.Serialize(allowed_clients));
                        HttpContext.Session.SetString("allowed_menus_apps", JsonSerializer.Serialize(links.ToArray()));


                        return RedirectToPage("/Index");
                    //}
                    //else
                    //    return RedirectToPage("/user-navigation/new-user");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Username/Password";
                    return Page();
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Page();
            }
        }




    }
}
