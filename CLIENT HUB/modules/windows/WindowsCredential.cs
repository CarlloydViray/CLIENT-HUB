using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.DirectoryServices.ActiveDirectory;

using BPOI_HUB.modules.core;
using BPOI_HUB.model.database;
using System.Data;

namespace BPOI_HUB.modules.windows
{

    public struct Credentials
    {
        public string Username;
        public string Password;

    }

    public class WindowsCredential
    {
        private static readonly IConfiguration _configuration = ConfigHelper.Config;

        public static bool IsValid(string username, string password, string domain)
        {
            /*
            using PrincipalContext pc = new(ContextType.Domain, domain);
            {

                return pc.ValidateCredentials(username, password);
            }
            */

            POSTGRE_DB pg = new();

            pg.Connect(_configuration.GetValue("config_db", ""));
            DataTable result = pg.Select("accounts", "\"UserName\" = :username AND \"Password\" = :password", new string[] { username, password });
            pg.Disconnect();

            if (result.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }



        }
    }



}
