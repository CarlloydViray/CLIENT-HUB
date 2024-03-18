using BPOI_HUB.model.database;
using Newtonsoft.Json;
using System.Data;

namespace BPOI_HUB.model.admin
{
	public class RoleManager
	{

		public void AddRole(string name, string[] clients, string[] menus, string[] apps, bool is_admin)
		{

			POSTGRE_DB pg = new();

			pg.Connect();

			pg.Insert("roles", new string[] { "\"Name\"", "\"AllowedClients\"", "\"AllowedMenus\"", "\"AllowedApps\"", "\"DateCreated\"", "\"IsAdmin\"" },
										   new string[] { name, JsonConvert.SerializeObject(clients), JsonConvert.SerializeObject(menus), JsonConvert.SerializeObject(apps), ":date_created", ":editable" }, new object[] { DateTime.Now, is_admin });

			pg.Disconnect();
		}

		public void DeleteRole(int id)
		{
			POSTGRE_DB pg = new();

			pg.Connect();

			pg.Delete("roles", "\"Id\" = :id", new object[] { id });

			pg.Disconnect();

		}

        public void UpdateRole(int id, string[] clients, string[] menus, string[] apps)
        {
            POSTGRE_DB pg = new();

            Dictionary<string, object> data = new(){
                ["\"AllowedClients\""] = JsonConvert.SerializeObject(clients),
                ["\"AllowedMenus\""]   = JsonConvert.SerializeObject(menus),
                ["\"AllowedApps\""]    = JsonConvert.SerializeObject(apps)
            };

            pg.Connect();

			pg.Update("roles", "\"Id\" = :id", data, new object[] { id });

            pg.Disconnect();

        }

		public void GetAllRoles(ref List<Role> roleList)
		{
			POSTGRE_DB pg = new();

			pg.Connect();

			DataTable result = pg.Select("roles", "\"IsAdmin\" = :is_admin", new object[] { false }, "\"Id\" ASC");

			pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                Role r = new() 
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Name = row["Name"].ToString(),
                    AllowedClients = JsonConvert.DeserializeObject<string[]>(row["AllowedClients"].ToString()),
                    AllowedMenus = JsonConvert.DeserializeObject<int[]>(row["AllowedMenus"].ToString()),
                    AllowedApps = JsonConvert.DeserializeObject<int[]>(row["AllowedApps"].ToString()),
                    IsAdmin = (bool)row["IsAdmin"]
                };

                roleList.Add(r);
			}


		}

        public void GetAllUserRoles(ref List<Role> roleList)
        {
            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("roles", null, null, "\"Id\" ASC");

            pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                Role r = new()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Name = row["Name"].ToString(),
                    AllowedClients = JsonConvert.DeserializeObject<string[]>(row["AllowedClients"].ToString()),
                    AllowedMenus = JsonConvert.DeserializeObject<int[]>(row["AllowedMenus"].ToString()),
                    AllowedApps = JsonConvert.DeserializeObject<int[]>(row["AllowedApps"].ToString()),
                    IsAdmin = (bool)row["IsAdmin"]
                };

                roleList.Add(r);
            }


        }

        public void GetAllAdminRoles(ref List<Role> roleList)
        {
            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("roles", "\"IsAdmin\" = :is_admin", new object[] { true }, "\"Id\" ASC");

            pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                Role r = new()
                {
                    Id = int.Parse(row["id"].ToString()),
                    Name = row["name"].ToString(),
                    AllowedClients = JsonConvert.DeserializeObject<string[]>(row["AllowedClients"].ToString()),
                    AllowedMenus = JsonConvert.DeserializeObject<int[]>(row["AllowedMenus"].ToString()),
                    AllowedApps = JsonConvert.DeserializeObject<int[]>(row["AllowedApps"].ToString()),
                    IsAdmin = (bool)row["IsAdmin"]
                };

                roleList.Add(r);
            }


        }

        public void GetRoleById(ref Role r, int id)
		{

            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("roles", "\"Id\" = :id", new object[] { id }, "\"Id\" ASC");

            pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                r.Id = int.Parse(row["Id"].ToString());
                r.Name = row["Name"].ToString();

                r.AllowedClients = JsonConvert.DeserializeObject<string[]>(row["AllowedClients"].ToString());
				r.AllowedMenus   = JsonConvert.DeserializeObject<int[]>(row["AllowedMenus"].ToString());
                r.AllowedApps    = JsonConvert.DeserializeObject<int[]>(row["AllowedApps"].ToString());

            }
        }

		public bool IsAdmin(int id)
		{
			bool is_admin = false;

			POSTGRE_DB pg = new();

			pg.Connect();

			DataTable result = pg.Select("roles", "\"Id\" = :id", new object[] { id }, "\"Id\" ASC", new string[] { "\"IsAdmin\"" });

            pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                is_admin = (bool)row["IsAdmin"];

            }


			return is_admin;
        }

        public bool IsSuperAdmin(int id)
        {
            bool is_super_admin = false;

            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("roles", "\"Id\" = :id", new object[] { id }, "\"Id\" ASC", new string[] { "\"Name\"" });

            pg.Disconnect();

            foreach (DataRow row in result.Rows)
            {
                if (row["Name"].ToString() == "super admin")
                    is_super_admin = true;

            }


            return is_super_admin;
        }
    }
}
