  using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ProcessModel
{
    public class EmployeeLeaveBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;

        public List<EmployeeLeaveListDO> GetAllLeaveRequestsForHr()
        {
            List<EmployeeLeaveListDO> listdata = new List<EmployeeLeaveListDO>();
            try
            {
                List<EmployeeLeaveListDO> primaryList = new List<EmployeeLeaveListDO>();
                try
                {
                    getDrtolist getDrtolistParam = new getDrtolist();
                    primaryList = getDrtolistParam.getdatafromreder<EmployeeLeaveListDO>(
                        DataClass.GetDataReaderFromSpWithParam(null, DBName, "SP_getallleaverequest_HR")
                    );
                }
                catch
                {
                    // Ignore and continue to secondary fallback (lean_mng).
                }

                if (primaryList != null && primaryList.Count > 0)
                {
                    return primaryList;
                }

                // Fallback: read from secondary connection string when leave data lives there.
                listdata = GetLeaveRequestsFromSecondaryConnection();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeLeaveBL",
                    "GetAllLeaveRequestsForHr",
                    "Exception Message" + ex.Message + "Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        private List<EmployeeLeaveListDO> GetLeaveRequestsFromSecondaryConnection()
        {
            List<EmployeeLeaveListDO> listdata = new List<EmployeeLeaveListDO>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listdata;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("SP_getallleaverequest_HR", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        listdata = MapLeaveRows(dr);
                    }
                }
            }
            catch (Exception exMySql)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(Sqlconnection))
                    using (SqlCommand cmd = new SqlCommand("SP_getallleaverequest_HR", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listdata.Add(new EmployeeLeaveListDO
                                {
                                    leave_id = dr["leave_id"] != DBNull.Value ? Convert.ToInt32(dr["leave_id"]) : 0,
                                    emp_id = dr["emp_id"] != DBNull.Value ? Convert.ToInt32(dr["emp_id"]) : 0,
                                    emp_name = dr["emp_name"] != DBNull.Value ? Convert.ToString(dr["emp_name"]) : string.Empty,
                                    approval_status = dr["approval_status"] != DBNull.Value ? Convert.ToString(dr["approval_status"]) : string.Empty,
                                    request_date = dr["request_date"] != DBNull.Value ? Convert.ToString(dr["request_date"]) : string.Empty
                                });
                            }
                        }
                    }
                }
                catch (Exception exSql)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "EmployeeLeaveBL",
                        "GetLeaveRequestsFromSecondaryConnection",
                        "MySqlError=" + exMySql.Message + " | SqlError=" + exSql.Message,
                        UserId
                    );
                }
            }

            return listdata;
        }

        public EmployeeLeaveDetailDO GetLeaveRequestById(int leaveId)
        {
            EmployeeLeaveDetailDO detail = null;
            if (leaveId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return detail;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetemployeeLeaveRequestBy_Id", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_leaveId", leaveId);
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            detail = MapLeaveDetail(dr);
                        }
                    }
                }
            }
            catch (Exception exMySql)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(Sqlconnection))
                    using (SqlCommand cmd = new SqlCommand("SP_GetemployeeLeaveRequestBy_Id", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_leaveId", leaveId);
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                detail = MapLeaveDetail(dr);
                            }
                        }
                    }
                }
                catch (Exception exSql)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "EmployeeLeaveBL",
                        "GetLeaveRequestById",
                        "MySqlError=" + exMySql.Message + " | SqlError=" + exSql.Message,
                        UserId
                    );
                }
            }

            return detail;
        }

        private EmployeeLeaveDetailDO MapLeaveDetail(IDataRecord dr)
        {
            return new EmployeeLeaveDetailDO
            {
                leave_id = GetInt(dr, "leave_id"),
                start_date = GetString(dr, "start_date"),
                end_date = GetString(dr, "end_date"),
                leave_description = GetString(dr, "leave_description"),
                approval_status = GetInt(dr, "approval_status_id"),
                approval_status_id = GetInt(dr, "approval_status_id"),
                approval_status_text = GetString(dr, "approval_status"),
                created = GetDate(dr, "created"),
                lookupId = GetInt(dr, "lookupId"),
                leave_daytype = GetString(dr, "leave_daytype"),
                leaves_types_id = HasColumn(dr, "leaves_types_id") && dr["leaves_types_id"] != DBNull.Value ? (int?)Convert.ToInt32(dr["leaves_types_id"]) : null,
                leaves_types = GetString(dr, "leaves_types"),
                leavefromtime = GetString(dr, "leavefromtime"),
                leavetotime = GetString(dr, "leavetotime"),
                rejection_remark = GetString(dr, "rejection_remark")
            };
        }

        private bool HasColumn(IDataRecord dr, string columnName)
        {
            return GetOrdinalIgnoreCase(dr, columnName) >= 0;
        }

        private int GetOrdinalIgnoreCase(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private string GetString(IDataRecord dr, string columnName)
        {
            int ordinal = GetOrdinalIgnoreCase(dr, columnName);
            if (ordinal < 0 || dr.IsDBNull(ordinal))
            {
                return string.Empty;
            }
            return Convert.ToString(dr.GetValue(ordinal));
        }

        private int GetInt(IDataRecord dr, string columnName)
        {
            int ordinal = GetOrdinalIgnoreCase(dr, columnName);
            if (ordinal < 0 || dr.IsDBNull(ordinal))
            {
                return 0;
            }
            return Convert.ToInt32(dr.GetValue(ordinal));
        }

        private DateTime GetDate(IDataRecord dr, string columnName)
        {
            int ordinal = GetOrdinalIgnoreCase(dr, columnName);
            if (ordinal < 0 || dr.IsDBNull(ordinal))
            {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(dr.GetValue(ordinal));
        }

        private List<EmployeeLeaveListDO> MapLeaveRows(MySqlDataReader dr)
        {
            List<EmployeeLeaveListDO> listdata = new List<EmployeeLeaveListDO>();
            while (dr.Read())
            {
                listdata.Add(new EmployeeLeaveListDO
                {
                    leave_id = dr["leave_id"] != DBNull.Value ? Convert.ToInt32(dr["leave_id"]) : 0,
                    emp_id = dr["emp_id"] != DBNull.Value ? Convert.ToInt32(dr["emp_id"]) : 0,
                    emp_name = dr["emp_name"] != DBNull.Value ? Convert.ToString(dr["emp_name"]) : string.Empty,
                    approval_status = dr["approval_status"] != DBNull.Value ? Convert.ToString(dr["approval_status"]) : string.Empty,
                    request_date = dr["request_date"] != DBNull.Value ? Convert.ToString(dr["request_date"]) : string.Empty
                });
            }
            return listdata;
        }

        private string NormalizeMySqlConnectionString(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return raw;
            }

            var pairs = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in pairs)
            {
                var idx = pair.IndexOf('=');
                if (idx <= 0) continue;
                var key = pair.Substring(0, idx).Trim();
                var value = pair.Substring(idx + 1).Trim();
                dict[key] = value;
            }

            string server = dict.ContainsKey("Server") ? dict["Server"] :
                            dict.ContainsKey("Data Source") ? dict["Data Source"] : string.Empty;
            string port = dict.ContainsKey("Port") ? dict["Port"] : "3306";
            string database = dict.ContainsKey("Database") ? dict["Database"] :
                              dict.ContainsKey("Initial Catalog") ? dict["Initial Catalog"] : string.Empty;
            string user = dict.ContainsKey("User Id") ? dict["User Id"] :
                          dict.ContainsKey("uid") ? dict["uid"] :
                          dict.ContainsKey("User") ? dict["User"] : string.Empty;
            string password = dict.ContainsKey("Password") ? dict["Password"] :
                              dict.ContainsKey("pwd") ? dict["pwd"] : string.Empty;

            return string.Format(
                "Server={0};Port={1};Database={2};User={3};Password={4};Persist Security Info=True;Convert Zero Datetime=True;",
                server, port, database, user, password
            );
        }
    }
}
