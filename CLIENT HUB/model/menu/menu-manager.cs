using BPOI_HUB.modules.core;
using BPOI_HUB.model.database;
using System.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.Functions;

namespace BPOI_HUB.model.menu
{
    public class MenuManager
    {
        private readonly string db;
        public MenuManager()
        {
            IConfiguration? _configuration = ConfigHelper.Config;

            this.db = _configuration.GetValue("config_db", "");

        }

        public DataTable GetMainMenus()
        {
            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = pg.Select("menus", "\"Category\" = :category", new string[] { "main menu" }, " Index ASC");

            pg.Disconnect();


            return result;
        }

        public DataTable GetMainMenus(int[] menu_ids)
        {
            POSTGRE_DB pg = new();

            pg.Connect(db);

            string sql = "SELECT * FROM menus WHERE \"MenuId\" IN (" + string.Join(",", menu_ids.Select((id, index) => $"{id}")) + ") " +
                        " OR (\"Category\" = 'main menu' AND \"IsRequired\" = true) ORDER BY \"Index\" ASC";

            DataTable result = pg.ExecuteQuery(sql);

            pg.Disconnect();


            return result;
        }

        public string[] GetMainMenuList()
        {
            List<string> list = new();
            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = pg.Select("menus", "\"Category\" = :category", new string[] { "main menu" }, " \"Index\" ASC", new string[] { "MenuId"});

            pg.Disconnect();


            foreach (DataRow row in result.Rows)
            {
                list.Add(row["MenuId"].ToString());
            }

            return list.ToArray();
        }

        public DataTable GetAdminMenus()
        {
            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = pg.Select("menus", "\"Category\" = :category", new string[] { "admin menu" }, " \"Index\" ASC");

            pg.Disconnect();


            return result;
        }

        public DataTable? GetSubMenus(int? parent)
        {
            if (parent == null)
                return null;

            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = pg.Select("menus", "\"Category\" = :category AND \"Parent\" = :parent", new object[] { "sub menu", parent }, " \"Index\" ASC");

            pg.Disconnect();


            return result;

        }

        public void GetMainMenus(ref List<MenuItem> menuList, string category, string? sub_category = null, string[]? columns = null, bool? is_required = false)
        {
            POSTGRE_DB pg = new();
            pg.Connect(db);

            DataTable result;

            if (sub_category == null)
                result = pg.Select("menus", "\"Category\" = :category AND \"IsRequired\" = :is_required", new object[] { category, is_required }, " \"Index\" ASC", columns);
            else
                result = pg.Select("menus", "\"Category\" = :category AND \"SubCategory\" = :sub_category AND \"IsRequired\" = :is_required", new object[] { category, sub_category, is_required }, " \"Index\" ASC", columns);

            pg.Disconnect();

            AddToMenuList(ref menuList, result, pg, columns);


        }
        public void GetMainAdminMenus(ref List<MenuItem> menuList, string? sub_category = null, string[]? columns = null, bool? is_required = false)
        {
            POSTGRE_DB pg = new();
            pg.Connect(db);

            DataTable result;

            if (sub_category == null)
                result = pg.Select("menus", "(\"Category\" = 'admin menu') AND \"IsRequired\" = :is_required", new object[] { is_required }, " \"Index\" ASC", columns);
            else
                result = pg.Select("menus", "(\"Category\" = 'admin menu') AND \"SubCategory\" = :sub_category AND \"IsRequired\" = :is_required", new object[] { sub_category, is_required }, " \"Index\" ASC", columns);

            pg.Disconnect();

            AddToMenuList(ref menuList, result, pg, columns);

        }

        public void GetAllApps(ref List<MenuItem> menuList, string? sub_category = null, string[]? columns = null)
        {
            POSTGRE_DB pg = new();
            pg.Connect(db);

            DataTable result;

            if (sub_category == null)
                result = pg.Select("menus", "\"Category\" = :category", new object[] { "apps" }, " \"Index\" ASC", columns);
            else
                result = pg.Select("menus", "\"Category\" = :category AND \"SubCategory\" = :sub_category", new object[] { "apps", sub_category }, " \"Index\" ASC", columns);

            pg.Disconnect();

            AddToMenuList(ref menuList, result, pg, columns);


        }

        public void GetAllAdminApps(ref List<MenuItem> menuList, string? sub_category = null, string[]? columns = null)
        {
            POSTGRE_DB pg = new();
            pg.Connect(db);

            DataTable result;

            if (sub_category == null)
                result = pg.Select("menus", "\"Category\" = :category", new object[] { "admin apps" }, " \"Index\" ASC", columns);
            else
                result = pg.Select("menus", "\"Category\" = :category AND \"SubCategory\" = :sub_category", new object[] { "admin apps", sub_category }, " \"Index\" ASC", columns);

            pg.Disconnect();

            
            AddToMenuList(ref menuList, result, pg, columns);


        }

        private void AddToMenuList(ref List<MenuItem> menuList, DataTable result, POSTGRE_DB pg, string[]? columns = null)
        {
            foreach (DataRow row in result.Rows)
            {
                MenuItem m = new();

                if (columns != null)
                {
                    foreach (string col in columns)
                    {
                        string colName = col.Replace("\"", "");
                        pg.SetPropertyValue(m, colName, row[colName]);
                    }        
                }
                else
                {
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        string colName = col.ColumnName.Replace("\"", "");
                        pg.SetPropertyValue(m, colName, row[colName]);
                    }
                }

                menuList.Add(m);
            }
        }

        public DataTable GetAllAppsByParentId(int parent_id, int[] app_ids)
        {
            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = new();

            string sql = "SELECT * FROM menus WHERE \"MenuId\" IN (" + string.Join(",", app_ids.Select((id, index) => $"{id}")) + ") " +
                        " AND \"Parent\" = :parent_id OR (\"Category\" = 'apps' AND \"IsRequired\" = true)  ORDER BY \"Index\" ASC";

            result = pg.ExecuteQuery(sql, new object[] { parent_id });

            pg.Disconnect();


            return result;
        }

        public string GetMenuLinkById(int id) 
        {
            POSTGRE_DB pg = new();

            pg.Connect(db);

            DataTable result = pg.Select("menus", "\"MenuId\" = :menu_id", new object[] { id }, " \"Index\" ASC", new string[] { "\"Link\"" });

            pg.Disconnect();


            if (result.Rows.Count > 0)
                return result.Rows[0]["Link"].ToString();
            else
                return "";

        }

        


    }
}
