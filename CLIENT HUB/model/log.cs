using System.Numerics;
using BPOI_HUB.model.database;
using Npgsql;
using System.Data;
using System.Collections.Generic;

namespace BPOI_HUB.model
{
    public class Log
    {
        public BigInteger Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string RequestorAccount { get; set; }
        public DateTime RequestDateTime { get; set; }
        public string MachineId { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public int RetryCount { get; set; }
        public DateTime ExecutionStartTime { get; set; }
        public DateTime ExecuttionEndTime { get; set; }
        public string OutputFileUrl { get; set; }
        public string ClientCode { get; set; }
        public string ApplicationId { get; set; }
        public string SystemType { get; set; }

        private string sql = "";
        private readonly Dictionary<string, object> sqlParams = new();

        public static readonly string STATUS_PENDING = "PENDING";
        public static readonly string STATUS_INPROGRESS = "IN-PROGRESS";
        public static readonly string STATUS_COMPLETED = "COMPLETED";
        public static readonly string STATUS_EXCEPTION = "EXCEPTION";

        private static readonly List<string> pendingRequired = new()
        {
            "title",
            "body",
            "requestor_account",
            "request_date_time",
            "client_code",
            "application_id",
            "system_type",
        };

        private static readonly List<string> inProgressRequired = new()
        {
            "id",
            "title",
            "body",
            "requestor_account",
            "request_date_time",
            "client_code",
            "application_id",
            "system_type",
        };

        private static readonly List<string> completedRequired = new()
        {
            "id",
            "title",
            "body",
            "requestor_account",
            "request_date_time",
            "client_code",
            //"status",
            "execution_start_time",
            "execution_end_time",
            "application_id",
            "system_type",
        };

        private static readonly List<string> exceptionRequired = new()
        {
            "id",
            "title",
            "body",
            "requestor_account",
            "request_date_time",
            "client_code",
            //"status",
            "execution_start_time",
            "execution_end_time",
            "application_id",
            "system_type",
        };

        public Log()
        {

        }

        public static Boolean ValidateRequiredFields(string status, dynamic request)
        {
            Boolean result;
            string msg = "";

            if (status.Equals(STATUS_PENDING, StringComparison.OrdinalIgnoreCase))
                result = pendingRequired.Any() && pendingRequired.All(key => request.ContainsKey(key));
            else if (status.Equals(STATUS_INPROGRESS, StringComparison.OrdinalIgnoreCase))
                result = inProgressRequired.Any() && inProgressRequired.All(key => request.ContainsKey(key));
            else if (status.Equals(STATUS_COMPLETED, StringComparison.OrdinalIgnoreCase))
                result = completedRequired.Any() && completedRequired.All(key => request.ContainsKey(key));
            else if (status.Equals(STATUS_EXCEPTION, StringComparison.OrdinalIgnoreCase))
                result = exceptionRequired.Any() && exceptionRequired.All(key => request.ContainsKey(key));
            else
                result = false;

            return result;
        }

        public int AssignId()
        {
            //Boolean validated;
            int assigned_id = -1;

            POSTGRE_DB pg = new();

            pg.Connect();
            pg.BeginTransaction();

            BuildIdSql();

            /*y
            {
                System.IO.File.AppendAllText(errorFile, Environment.NewLine + "SQL: " + sql);

                NpgsqlCommand cmd = new NpgsqlCommand(sql, pg.conn);

                foreach (KeyValuePair<string, object> param in sqlParams)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                    System.IO.File.AppendAllText(errorFile, Environment.NewLine + "Key: " + param.Key + " Value: " + param.Value);
                }

                //cmd.ExecuteNonQuery();

                //assigned_id = Convert.ToInt32(cmd.ExecuteScalar());
                //assigned_id = (int)cmd.ExecuteScalar();
                cmd.ExecuteScalar();

                pg.commit_transaction();
                //validated = true;
            }
            catch (PostgresException e)
            {
                pg.rollback_transaction();
            }
            catch (NpgsqlException e)
            {
                pg.rollback_transaction();
            }
            catch (Exception e)
            {
                pg.rollback_transaction();
            }
            */
            pg.Disconnect();
            return assigned_id;
        }

        public Boolean UpdateStatus()
        {
            Boolean validated = false;

            POSTGRE_DB pg = new();

            pg.Connect();
            pg.BeginTransaction();

            BuildUpdateSql();

            /*try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(sql, pg.conn);

                foreach(KeyValuePair<string, object> param in sqlParams)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                cmd.ExecuteNonQuery();
                pg.commit_transaction();
                validated = true;
            }
            catch (PostgresException e)
            {
                pg.rollback_transaction();
                validated = false;
                System.IO.File.AppendAllText(errorFile, Environment.NewLine + e.Message);
            }
            catch (NpgsqlException e)
            {
                pg.rollback_transaction();
                validated = false;
                System.IO.File.AppendAllText(errorFile, Environment.NewLine + e.Message);
            }
            catch (Exception e)
            {
                pg.rollback_transaction();
                validated = false;
                System.IO.File.AppendAllText(errorFile, Environment.NewLine + e.Message);
            }*/

            pg.Disconnect();
            return validated;
        }

        private void BuildUpdateSql()
        {
            List<string> setSql = new();

            if (Title != null) {
                setSql.Add("title=@title"); 
                sqlParams.Add("title", Title);
            }
            if (Body != null) {
                setSql.Add("body=@body");
                sqlParams.Add("body", Body);
            }
            if (RequestorAccount != null) {
                setSql.Add("requestor_account=@requestor_account");
                sqlParams.Add("requestor_account", RequestorAccount);
            }
            if (RequestDateTime != null) {
                setSql.Add("request_date_time=@request_date_time");
                sqlParams.Add("request_date_time", RequestDateTime);
            }
            if (MachineId != null)
            {
                setSql.Add("machine_id=@machine_id");
                sqlParams.Add("machine_id", RequestDateTime);
            }
            if (Status != null)
            {
                setSql.Add("status=@status");
                sqlParams.Add("status", Status);
            }
            if (Details != null)
            {
                setSql.Add("details=@details");
                sqlParams.Add("details", Details);
            }
            if (RetryCount > 0)
            {
                setSql.Add("retry_count=@retry_count");
                sqlParams.Add("retry_count", RetryCount);
            }
            if (ExecutionStartTime != null) {
                setSql.Add("execution_start_time=@execution_start_time");
                sqlParams.Add("execution_start_time", ExecutionStartTime);
            }
            if (ExecuttionEndTime != null) {
                setSql.Add("execution_end_time=@execution_end_time");
                sqlParams.Add("execution_end_time", ExecuttionEndTime);
            }
            if (OutputFileUrl != null)
            {
                setSql.Add("output_file_url=@output_file_url");
                sqlParams.Add("output_file_url", OutputFileUrl);
            }
            if (ClientCode != null) {
                setSql.Add("client_code=@client_code");
                sqlParams.Add("client_code", ClientCode);
            }
            if (ApplicationId != null) {
                setSql.Add("application_id=@application_id");
                sqlParams.Add("application_id", ApplicationId);
            }
            if (SystemType != null) {
                setSql.Add("system_type=@system_type");
                sqlParams.Add("system_type", SystemType);
            }
            
            if (setSql.Count > 0)
            {
                sqlParams.Add("id", Id);
                sql = "update log set " + String.Join(",", setSql) + " WHERE id=@id";
            }
        }

        private void BuildIdSql()
        {
            List<string> columnSql = new();

            if (!string.IsNullOrEmpty(Title))
            {
                columnSql.Add("title");
                sqlParams.Add("title", Title);
            }
            if (!string.IsNullOrEmpty(Body))
            {
                columnSql.Add("body");
                sqlParams.Add("body", Body);
            }
            if (!string.IsNullOrEmpty(RequestorAccount))
            {
                columnSql.Add("requestor_account");
                sqlParams.Add("requestor_account", RequestorAccount);
            }
            if (RequestDateTime != DateTime.MinValue)
            {
                columnSql.Add("request_date_time");
                sqlParams.Add("request_date_time", RequestDateTime);
            }
            if (!string.IsNullOrEmpty(MachineId))
            {
                columnSql.Add("machine_id");
                sqlParams.Add("machine_id", MachineId);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                columnSql.Add("status");
                sqlParams.Add("status", Status);
            }
            if (!string.IsNullOrEmpty(Details))
            {
                columnSql.Add("details");
                sqlParams.Add("details", Details);
            }
            if (RetryCount > 0)
            {
                columnSql.Add("retry_count");
                sqlParams.Add("retry_count", RetryCount);
            }
            if (ExecutionStartTime != DateTime.MinValue)
            {
                columnSql.Add("execution_start_time");
                sqlParams.Add("execution_start_time", ExecutionStartTime);
            }
            if (ExecuttionEndTime != DateTime.MinValue)
            {
                columnSql.Add("execution_end_time");
                sqlParams.Add("execution_end_time", ExecuttionEndTime);
            }
            if (!string.IsNullOrEmpty(OutputFileUrl))
            {
                columnSql.Add("output_file_url");
                sqlParams.Add("output_file_url", OutputFileUrl);
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                columnSql.Add("client_code");
                sqlParams.Add("client_code", ClientCode);
            }
            if (!string.IsNullOrEmpty(ApplicationId))
            {
                columnSql.Add("application_id");
                sqlParams.Add("application_id", ApplicationId);
            }
            if (!string.IsNullOrEmpty(SystemType))
            {
                columnSql.Add("system_type");
                sqlParams.Add("system_type", SystemType);
            }

            if (columnSql.Count > 0)
            {
                string paramPrefix = "@";
                //sql = "insert into log (" + String.Join(",", columnSql) + ") VALUES (" + String.Join(",", columnSql.Select(x => paramPrefix + x)) + ");SELECT SCOPE_IDENTITY();";
                //sql = "insert into log (" + String.Join(",", columnSql) + ") values (" + String.Join(",", columnSql.Select(x => paramPrefix + x)) + ") returning id";
                //sql = "insert into log (" + String.Join(",", columnSql) + ") output Inserted.id values (" + String.Join(",", columnSql.Select(x => paramPrefix + x)) + ")";
                sql = "insert into log (" + String.Join(",", columnSql) + ") values (" + String.Join(",", columnSql.Select(x => paramPrefix + x)) + ")";
            }
        }


    }
}
