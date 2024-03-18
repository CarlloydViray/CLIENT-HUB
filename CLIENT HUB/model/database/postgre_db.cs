using BPOI_HUB.modules.core;
using Npgsql;
using System.Data;
using System.Text.RegularExpressions;
using Match = System.Text.RegularExpressions.Match;

namespace BPOI_HUB.model.database
{
    public class POSTGRE_DB
    {
        private NpgsqlConnection conn;

        private NpgsqlTransaction connts;

        private readonly IConfiguration? _configuration;

        public POSTGRE_DB()
        {
            _configuration = ConfigHelper.Config;
        }

        public void Connect()
        {
            string cs = _configuration.GetConnectionString("postgre_cs");   

            cs = cs.Replace("@database", "hub_web");

            conn = new NpgsqlConnection(cs);
            conn.Open();

        }
        public void Connect(string database)
        {
            string cs = _configuration.GetConnectionString("postgre_cs");

            cs = cs.Replace("@database", database);
                
            conn = new NpgsqlConnection(cs);
            conn.Open();
   
        }

        public void Disconnect()
        {
            conn.Close();            
        }

        public void BeginTransaction()
        {
            connts = conn.BeginTransaction();
        }

        public void CommitTransaction() 
        {
            connts.Commit();
        }

        public void RollBackTransaction()
        {
            connts.Rollback();
        }

        public DataTable Select(string table, string? whereClause = null, object[]? param = null, string? orderbyClause = null, string[]? columns = null)
        {
            string sql;
            string columnFields;
            List<string> paramList = new();

            if (columns != null)
            {
                columnFields = String.Join(",", columns);              
            }
            else
                columnFields = "*";

            sql = "SELECT " + columnFields + " FROM " + table;

            if (whereClause != null)
            {
                sql += " WHERE " + whereClause;

                string pattern = @":\w+";

                MatchCollection matches = Regex.Matches(whereClause, pattern);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches.Cast<Match>())
                    {
                        paramList.Add(match.Value);
                    }
                }
            }

            if(orderbyClause != null)
            {
                sql += " ORDER BY " + orderbyClause;
            }

            NpgsqlCommand cmd = new(sql, conn);

            if (param != null)
            {
                int paramCount = 0;

                foreach (object paramValue in param)
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], paramValue);
                    paramCount++;
                }
            }
  
            NpgsqlDataReader reader = cmd.ExecuteReader();

            DataTable result = new();
            result.Load(reader);


            //DisplayDataTable(result);


            return result;
        }

        public DataTable ExecuteQuery(string sql, object[]? param = null)
        {
            List<string> paramList = new();
            NpgsqlCommand cmd = new(sql, conn);

            if (sql != null)
            {
                string pattern = @":\w+";

                MatchCollection matches = Regex.Matches(sql, pattern);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches.Cast<Match>())
                    {
                        paramList.Add(match.Value);
                    }
                }
            }

            if (param != null)
            {
                int paramCount = 0;

                foreach (object paramValue in param)
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], paramValue);
                    paramCount++;
                }
            }


            NpgsqlDataReader reader = cmd.ExecuteReader();

            DataTable result = new();
            result.Load(reader);


            return result;
        }

        public int ExecuteScalar(string table, string? whereClause = null, string[]? param = null)
        {
            string sql = "SELECT COUNT(*) FROM " + table;

            if (whereClause != null)
            {
                sql += " WHERE " + whereClause;
            }

            NpgsqlCommand cmd = new(sql, conn);

            if (param != null)
            {
                foreach (string paramValue in param)
                {
                    string[] paramArr = paramValue.Split("|");
                    cmd.Parameters.AddWithValue(paramArr[0], paramArr[1]);
                }
            }

            return Convert.ToInt32(cmd.ExecuteScalar());           
        }

        public int Insert(string table, string[]? columns, string[] values, object[]? param)
        {
            if (values == null)
                return 0;

            string sql = "INSERT INTO " + table;
            List<string> paramList = new();

            if(columns != null)
            {
                sql += " (" + String.Join(",", columns ) + ") ";
            }

            sql += " VALUES(";

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains(':') == false)
                    values[i] = "'" + values[i] + "'";
                else
                    paramList.Add(values[i]);
            }

            sql = sql + String.Join(",",  values) + ")";
        
            NpgsqlCommand cmd = new(sql, conn);

            if (param != null)
            {
                int paramCount = 0;

                foreach (object paramValue in param)
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], paramValue);
                    paramCount++;
                }
            }

            return cmd.ExecuteNonQuery();
        }

        public int Delete(string table, string? whereClause = null, object[]? param = null)
        {
            if (whereClause == null)
                return 0;

            string sql = "DELETE FROM " + table;
            List<string> paramList = new();


            if (whereClause != null)
            {
                sql += " WHERE " + whereClause;

                string pattern = @":\w+";

                MatchCollection matches = Regex.Matches(whereClause, pattern);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches.Cast<Match>())
                    {
                        paramList.Add(match.Value);
                    }
                }
            }


            NpgsqlCommand cmd = new(sql, conn);

            if (param != null)
            {
                
                int paramCount = 0;

                foreach (object paramValue in param)
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], paramValue);
                    paramCount++;
                }
                
            }

            return cmd.ExecuteNonQuery();
        }

        public int Update(string table, string whereClause, Dictionary<string, object> data, object[]? param)
        {

            if (data.Count <= 0)
                return 0;

            string sql = "UPDATE " + table + " SET ";
            List<string> paramList = new();

            foreach(string key in data.Keys)
            {
                if (data[key].ToString().Contains(':'))
                {
                    sql += key + " = :" + key.Replace("\"", "") + " , ";
                    paramList.Add(key.Replace("\"", ""));
                }
                else
                {
                    sql += key + " = '" + data[key].ToString() + "' , ";
                }
            }

            sql = sql[..^2];

            if (whereClause != null)
            {
                sql += " WHERE " + whereClause;

                string pattern = @":\w+";

                MatchCollection matches = Regex.Matches(whereClause, pattern);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches.Cast<Match>())
                    {
                        paramList.Add(match.Value.Replace("\"", ""));
                    }
                }
            }

            NpgsqlCommand cmd = new(sql, conn);
            int paramCount = 0;

            foreach (string key in data.Keys)
            {
                if (data[key].ToString().Contains(':'))
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], data[key]);
                    paramCount++;
                }
            }

            if (param != null)
            {              
                foreach (object paramValue in param)
                {
                    cmd.Parameters.AddWithValue(paramList[paramCount], paramValue);
                    paramCount++;
                }
            }

            return cmd.ExecuteNonQuery();
        }

        public void SetPropertyValue(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName);

            if (property != null && property.CanWrite)
            {
                // Convert the value to the property type if necessary
                var convertedValue = Convert.ChangeType(value, property.PropertyType);

                // Set the property value
                property.SetValue(obj, convertedValue);
            }
        }


    }
}
