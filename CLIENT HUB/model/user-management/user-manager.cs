using BPOI_HUB.modules.core;
using BPOI_HUB.model.database;
using Newtonsoft.Json;
using System.Data;
using NPOI.SS.Formula.Functions;
using BPOI_HUB.model.admin;
using BPOI_HUB.model.menu;

namespace BPOI_HUB.model.user_management
{
    public class UserManager
    {
        
        private readonly IConfiguration _configuration;

        public UserManager()
        {

            _configuration = ConfigHelper.Config;
        }


		public bool IsSignedIn(HttpContext context)
        {
            if (!context.Session.Keys.Any())
            {

                context.Response.Redirect("/login");
                return false;
            }



            return true;
        }

        public bool UserExist(string username)
        {
			
			
			POSTGRE_DB pg = new();

            pg.Connect(_configuration.GetValue("config_db", ""));

            int count = pg.ExecuteScalar( "accounts", "\"UserName\" = @Username", new string[] { "@Username|" + username });

            pg.Disconnect();


            if (count > 0)
                return true;
            else
                return false;
        }

        public int RegisterUser(string username, string firstName, string lastName, string middleName, string email, string department)
        {
            string[] role = { "21" };

            POSTGRE_DB pg = new();

			pg.Connect(_configuration.GetValue("config_db", ""));

			int result = pg.Insert("accounts", null, new string[] { username, lastName, firstName, middleName, ":DateRegister", ":LastLogin", JsonConvert.SerializeObject(role), email, department }, new object[] { DateTime.Now, DateTime.Now });

            pg.Disconnect();

            return result;
        }
        public void GetAllowedClientsMenu(string username, ref string[] allowed_clients, ref int[] allowed_menus, ref int[] allowed_apps, ref List<string> links)
        {
            UserManager um = new();
            RoleManager rm = new();
            MenuManager mm = new();



            string[] apps_list;


            User user = new()
            {
                UserName = username
            };

            um.GetUserData(ref user);

            foreach (string role in user.Roles)
            {
                Role r = new();
                rm.GetRoleById(ref r, int.Parse(role));


                if (r.AllowedClients != null)
                    allowed_clients = allowed_clients.Concat(r.AllowedClients).ToArray();

                if (r.AllowedMenus != null)
                    allowed_menus = allowed_menus.Concat(r.AllowedMenus).ToArray();

                if (r.AllowedApps != null)
                    allowed_apps = allowed_apps.Concat(r.AllowedApps).ToArray();

            }

            allowed_clients = allowed_clients.Distinct().ToArray();
            allowed_menus = allowed_menus.Distinct().ToArray();
            allowed_apps = allowed_apps.Distinct().ToArray();


            foreach (int am in allowed_menus)
            {
                string url = mm.GetMenuLinkById(am);

                if (url.Trim() != "")
                    links.Add(url);

            }

            foreach (int ap in allowed_apps)
            {
                string url = mm.GetMenuLinkById(ap);

                if (url.Trim() != "")
                    links.Add(url);
            }

            links.Add("/");
            links.Add("/Index");
            links.Add("/core/download-file");

          

        }


        public void GetUserData(ref User user)
        {
            POSTGRE_DB pg = new();

			pg.Connect(_configuration.GetValue("config_db", ""));

			DataTable result = pg.Select("accounts", "\"UserName\" = :username", new string[] { user.UserName });

            user.FirstName  = result.Rows[0]["FirstName"].ToString();
            user.MiddleName = result.Rows[0]["MiddleName"].ToString();
            user.LastName   = result.Rows[0]["LastName"].ToString();
            user.Email      = result.Rows[0]["Email"].ToString();
            user.Department = result.Rows[0]["Department"].ToString();
            user.LastLogin = DateTime.Parse(result.Rows[0]["LastLogin"].ToString()).ToLocalTime();
            user.Roles = JsonConvert.DeserializeObject<string[]>(result.Rows[0]["Role"].ToString());

            pg.Disconnect();

        }

        public void GetAllUserRoles(ref List<User> userList)
        {
            POSTGRE_DB pg = new();

            pg.Connect(_configuration.GetValue("config_db", ""));

            DataTable result = pg.Select("accounts", null, null, null, new string[] {"\"UserName\"", "\"LastName\"", "\"FirstName\"", "\"MiddleName\"", "\"Role\""});

            foreach (DataRow row in result.Rows)
            {
                User u = new()
                {
                    UserName = row["UserName"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    Roles = JsonConvert.DeserializeObject<string[]>(row["Role"].ToString())
                };
     
                

                userList.Add(u);
            }

            pg.Disconnect();
        }

        public bool IsSuperAdmin(string username)
        {
            User u = new();
            RoleManager rm = new();
            bool IsSuperAdmin = false;

            POSTGRE_DB pg = new();

            pg.Connect(_configuration.GetValue("config_db", ""));

            DataTable result = pg.Select("accounts", "\"UserName\" = :Username", new object[] { username }, null, new string[] { "\"UserName\"", "\"Role\"" });

            foreach (DataRow row in result.Rows)
            {
                u.UserName = row["UserName"].ToString();
                u.Roles = JsonConvert.DeserializeObject<string[]>(row["Role"].ToString());
    
            }

            pg.Disconnect();

 
            foreach(string role in u.Roles)
            {
                if(rm.IsSuperAdmin(int.Parse(role)))
                    IsSuperAdmin = true;
            }


            return IsSuperAdmin;
        }

        public void UpdateLastLogin(string username)
        {
            POSTGRE_DB pg = new();

            Dictionary<string, object> data = new()
            {
                ["\"LastLogin\""] = DateTime.Now
            };

            pg.Connect(_configuration.GetValue("config_db", ""));

            pg.Update("accounts", "\"UserName\" = :username", data, new object[] { username });

            pg.Disconnect();
        }

        public void UpdateRole(string username, string[] roles)
        {
            POSTGRE_DB pg = new();

            Dictionary<string, object> data = new()
            {
                ["\"Role\""] = JsonConvert.SerializeObject(roles)
            };

            pg.Connect(_configuration.GetValue("config_db", ""));

            pg.Update("accounts", "\"UserName\" = :username", data, new object[] { username });

            pg.Disconnect();

        }

    }
}
