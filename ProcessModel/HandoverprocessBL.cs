using DataObject;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcessModel
{
    public class HandoverprocessBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;

        public List<ResignationDO> GetEmployeeResignationDetails(int reportingManagerId)
        {
            List<ResignationDO> listdata = new List<ResignationDO>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listdata;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("Sp_GetEmployeeResignationDetailsHR", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            listdata = MapResignationRows(dr);
                        }
                    }
                }
            }
            catch (MySqlException exParam)
            {
                // Fallback for SP variants that take reporting manager as input.
                if (exParam.Message != null &&
                    (exParam.Message.IndexOf("expects", StringComparison.OrdinalIgnoreCase) >= 0
                    || exParam.Message.IndexOf("arguments", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    try
                    {
                        string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                        using (MySqlConnection con = new MySqlConnection(normalized))
                        using (MySqlCommand cmd = new MySqlCommand("Sp_GetEmployeeResignationDetailsHR", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (reportingManagerId > 0)
                            {
                                cmd.Parameters.AddWithValue("@p_reporting_manager_id", reportingManagerId);
                            }
                            con.Open();
                            using (MySqlDataReader dr = cmd.ExecuteReader())
                            {
                                listdata = MapResignationRows(dr);
                            }
                        }
                    }
                    catch (Exception exFallback)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog(
                            "HandoverprocessBL",
                            "GetEmployeeResignationDetails_Fallback",
                            "Exception Message=" + exFallback.Message + " Strace=" + exFallback.StackTrace,
                            UserId
                        );
                    }
                }
                else
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "HandoverprocessBL",
                        "GetEmployeeResignationDetails",
                        "Exception Message=" + exParam.Message + " Strace=" + exParam.StackTrace,
                        UserId
                    );
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetEmployeeResignationDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        private List<ResignationDO> MapResignationRows(MySqlDataReader dr)
        {
            List<ResignationDO> listdata = new List<ResignationDO>();
            while (dr.Read())
            {
                listdata.Add(new ResignationDO
                {
                    UserId = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
                    EmployeeResignationId = dr["employee_resignation_id"] != DBNull.Value ? Convert.ToInt32(dr["employee_resignation_id"]) : 0,
                    resignation_date = dr["resignation_date"] != DBNull.Value ? Convert.ToDateTime(dr["resignation_date"]) : DateTime.MinValue,
                    notice_period_days = dr["notice_period_days"] != DBNull.Value ? Convert.ToInt32(dr["notice_period_days"]) : 0,
                    last_working_date = dr["last_working_date"] != DBNull.Value ? Convert.ToDateTime(dr["last_working_date"]) : DateTime.MinValue,
                    reason = dr["reason"] != DBNull.Value ? Convert.ToString(dr["reason"]) : string.Empty,
                    hr_status = dr["status"] != DBNull.Value ? Convert.ToString(dr["status"]) : "Pending",
                    remarks = dr["remarks"] != DBNull.Value ? Convert.ToString(dr["remarks"]) : string.Empty,
                    action_date = dr["action_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["action_date"]) : null,
                    reporting_manager = dr["reporting_manager"] != DBNull.Value ? Convert.ToInt32(dr["reporting_manager"]) : 0,
                    EmployeeName = dr["emp_name"] != DBNull.Value ? Convert.ToString(dr["emp_name"]) : string.Empty,
                    EmployeeEmail = dr["email_id"] != DBNull.Value ? Convert.ToString(dr["email_id"]) : string.Empty,
                    reporting_manager_name = dr["reporting_manager_name"] != DBNull.Value ? Convert.ToString(dr["reporting_manager_name"]) : string.Empty,
                    project_status = dr["project_status"] != DBNull.Value ? Convert.ToString(dr["project_status"]) : string.Empty
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
        public List<HandoverProcessDO> SaveHandoverProcess(HandoverProcessDO obj)
        {
            List<HandoverProcessDO> list = new List<HandoverProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                //param.Add(new MySqlParameter("@type", "SaveHandover"));
                param.Add(new MySqlParameter("@p_employee_resignation_id", obj.EmployeeResignationId));
                param.Add(new MySqlParameter("@p_user_id", obj.UserId));
                param.Add(new MySqlParameter("@p_PendriveBackup", obj.PendriveBackup ? 1 : 0));
                param.Add(new MySqlParameter("@p_LaptopWithCharger", obj.LaptopWithCharger ? 1 : 0));
                param.Add(new MySqlParameter("@p_ContactDetailsShared", obj.ContactDetailsShared ? 1 : 0));
                param.Add(new MySqlParameter("@p_DiarySubmitted", obj.DiarySubmitted ? 1 : 0));
                param.Add(new MySqlParameter("@p_ID_Card", obj.IDCard ? 1 : 0));

                param.Add(new MySqlParameter("@p_HR_Remark", obj.HR_Remark));
                param.Add(new MySqlParameter("@p_inserted_by", obj.InsertedBy));

                list = getDrtolistParam.getdatafromreder<HandoverProcessDO>(
                  DataClass.GetDataReaderFromSpWithParam(param, DBName, "SP_Save_Handover_Process")
              );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverProcessBL",
                    "SaveHandoverProcess",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }
        public HandoverProcessDO GetHandoverByResignationId(int resignationId)
        {

            getDrtolist dr = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            //param.Add(new MySqlParameter("@type", "GetByResignationId"));
            param.Add(new MySqlParameter("@p_employee_resignation_id", resignationId));

            var list = dr.getdatafromreder<HandoverProcessDO>(
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_Get_Handover_Process_By_ResignationId"
                ));

            return list != null && list.Count > 0 ? list[0] : null;
        }

        public List<TerminationProcessDO> SaveEmployeeTermination(TerminationProcessDO obj)
        {
            List<TerminationProcessDO> list = new List<TerminationProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                param.Add(new MySqlParameter("@p_company_id", obj.CompanyId));
                param.Add(new MySqlParameter("@p_user_id", obj.UserId));
                param.Add(new MySqlParameter("@p_employee_code", obj.EmployeeCode));
                param.Add(new MySqlParameter("@p_termination_date", obj.TerminationDate));
                param.Add(new MySqlParameter("@p_termination_reason", obj.termination_reason ?? ""));
                param.Add(new MySqlParameter("@p_PerformanceRating", obj.PerformanceRating.HasValue ? obj.PerformanceRating.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticePeriodDays", obj.NoticePeriodDays.HasValue ? obj.NoticePeriodDays.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_TerminationLetter", string.IsNullOrEmpty(obj.TerminationLetter) ? (object)DBNull.Value : obj.TerminationLetter));
                param.Add(new MySqlParameter("@p_ResponseDeadline", obj.ResponseDeadline.HasValue ? obj.ResponseDeadline.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticeLetter", string.IsNullOrEmpty(obj.NoticeLetter) ? (object)DBNull.Value : obj.NoticeLetter));
                param.Add(new MySqlParameter("@p_inserted_by", obj.InsertedBy));


                list = getDrtolistParam.getdatafromreder<TerminationProcessDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        DBName,
                        "SP_Save_Employee_Termination"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationProcessBL",
                    "SaveEmployeeTermination",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }

        public List<UserDetailsDO> GetTerminationList(int companyId)
        {
            List<UserDetailsDO> list = new List<UserDetailsDO>();

            try
            {
                List<MySqlParameter> param = new List<MySqlParameter>();

                param.Add(new MySqlParameter("@p_company_id", companyId));

                var reader = DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_GetTerminationDetails"
                );

                while (reader.Read())
                {
                    UserDetailsDO obj = new UserDetailsDO();

                    obj.UserId = Convert.ToInt32(reader["user_id"]);
                    obj.EmployeeCode = reader["employee_code"].ToString();
                    obj.notice_status = reader["notice_status"].ToString();

                    //obj.ResponseDeadline = reader["ResponseDeadline"] == DBNull.Value
                    //    ? (DateTime?)null
                    //    : Convert.ToDateTime(reader["ResponseDeadline"]);
                    obj.TerminationDate = reader["TerminationDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["TerminationDate"]);

                    
                    list.Add(obj);
                }

                reader.Close(); // ✅ Important: close reader
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetTerminationList",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }

        public List<TerminationProcessDO> saveshowcausenotice(TerminationProcessDO obj)
        {
            List<TerminationProcessDO> list = new List<TerminationProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                param.Add(new MySqlParameter("@p_CompanyId", obj.CompanyId));
                param.Add(new MySqlParameter("@p_UserId", obj.UserId));
                param.Add(new MySqlParameter("@p_EmployeeCode", obj.EmployeeCode));
                param.Add(new MySqlParameter("@p_ResponseDeadline", obj.ResponseDeadline.HasValue ? obj.ResponseDeadline.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticeLetter", string.IsNullOrEmpty(obj.NoticeLetter) ? (object)DBNull.Value : obj.NoticeLetter));
                param.Add(new MySqlParameter("@p_InsertedBy", obj.InsertedBy));


                list = getDrtolistParam.getdatafromreder<TerminationProcessDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        DBName,
                        "SP_SaveShowCauseNotice"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationProcessBL",
                    "SaveEmployeeTermination",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }
        public string GetShowCauseStatus(string USERID)
        {
            string status = "";

            List<MySqlParameter> param = new List<MySqlParameter>();

            param.Add(new MySqlParameter("@p_user_id", USERID));

            var dr = DataClass.GetDataReaderFromSpWithParam(
                param,
                DBName,
                "SP_GetShowCauseStatus"
            );

            if (dr.Read())
            {
                status = dr["notice_status"].ToString();
            }

            return status;
        }
        public TerminationProcessDO GetTerminationByUserId(int userId)
        {
            TerminationProcessDO data = null;

            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@p_user_id", userId));

            using (var dr = DataClass.GetDataReaderFromSpWithParam(param, DBName, "SP_GetTerminationByUserId"))
            {
                if (dr.Read())
                {
                    data = new TerminationProcessDO
                    {
                        UserId = userId,
                        ResponseDeadline = dr["ResponseDeadline"] != DBNull.Value
                                           ? Convert.ToDateTime(dr["ResponseDeadline"])
                                           : (DateTime?)null
                    };
                }
            }

            return data;
        }



        public void UpdateNoticeStatus(int userId, string status)
        {
            getDrtolist dr = new getDrtolist();

            List<MySqlParameter> param = new List<MySqlParameter>();

            param.Add(new MySqlParameter("@p_user_id", userId));
            param.Add(new MySqlParameter("@p_notice_status", status));

            // Call SP (ignore result)
            dr.getdatafromreder<object>(
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_UpdateNoticeStatusByUserId"
                )
            );
        }



    }
}
