
using BPOI_HUB.model.database;
using BPOI_HUB.modules.core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace BPOI_HUB.model.client
{
    public class ClientManager
    {

        public void GetAllClients(ref List<Client> clientList)
        {
            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("clients");

            pg.Disconnect();

            if (result != null)
            {

                foreach (DataRow row in result.Rows)
                {
                    Client c = new() 
                    {
                        ClientCode = row["client_code"].ToString(),
                        ClientName = row["client_name"].ToString(),

                        System = JsonConvert.DeserializeObject<Dictionary<string, string>>(row["system"].ToString()),
                        Division = row["division"].ToString()

                    };
                    clientList.Add(c);
                }
            }


        }

        public void GetAllClientsCodeName(ref List<Client> clientList)
        {
            POSTGRE_DB pg = new();

            pg.Connect();

            DataTable result = pg.Select("clients", null, null, null, new string[] { "ClientCode", "ClientName"});

            pg.Disconnect();

            if (result != null)
            {

                foreach (DataRow row in result.Rows)
                {
                    Client c = new()
                    {
                        ClientCode = row["ClientCode"].ToString(),
                        ClientName = row["ClientName"].ToString()
                    };

                    clientList.Add(c);
                }
            }


        }
    }
}
