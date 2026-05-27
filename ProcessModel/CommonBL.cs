using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using DataObject;
using MySql.Data.MySqlClient;


namespace ProcessModel
{
    public class CommonBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;
        public CommonDO fnStoreErrorLog(string pagename, string functionName, string Error, string UserId)
        {
            CommonDO lstComm = new CommonDO();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {

                mysqlParameters.Add(DataClass.GetParameter("@p_pagename", pagename));
                mysqlParameters.Add(DataClass.GetParameter("@p_function_name", functionName));
                mysqlParameters.Add(DataClass.GetParameter("@p_ErrorDescription", Error));
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", UserId));
                mysqlParameters.Add(DataClass.GetParameter("@p_Type", "saveError"));
                lstComm = (from ii in getDrtolistParam.getdatafromreder<CommonDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, "alpha_hrms", "sp_insert_errorlog"))
                           select ii).FirstOrDefault();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "fnStoreErrorLog", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
            return lstComm;
        }

        public List<DropDownData> dropdownusername()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindusername"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownusername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownempcode()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindempcode"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownempcode", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownComponent_Forremuneration()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindComponent_Forremuneration"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownComponent_Forremuneration", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        //public List<DropDown> dropdownsearchbyRole()
        //{
        //    List<DropDown> dropDown = new List<DropDown>();
        //    try
        //    {
        //        getDrtolist getDrtolistParam = new getDrtolist();
        //        List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //        mysqlParameters.Add(DataClass.GetParameter("@type", "getRoleSearch"));
        //        dropDown = getDrtolistParam.getdatafromreder<DropDown>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_searchby"));
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("CommonBL", "dropdownsearchbyRole", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //    return dropDown;
        //}

        public List<DropDownData> dropdownroles()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Bindrole"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindRoleAndBindUser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownroles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownusers()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Binduser"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindRoleAndBindUser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownusers", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownMenu(string type, string menuId)
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", type));
                mysqlParameters.Add(
                    DataClass.GetParameter("@p_menuid", string.IsNullOrEmpty(menuId) ? DBNull.Value : (object)menuId)
                );


                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindMenuAndSubmenu"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownSubMenu(string type, string menuId)
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", type));
                mysqlParameters.Add(DataClass.GetParameter("@p_menuid", menuId));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindMenuAndSubmenu"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownSubMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownDocuments()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();               
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindDocuments"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "dropdownDocuments", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownDesigntion()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();                //sqlParameters.Add(DataClass.GetParameter("@type", "BindGender"));
                //sqlParameters.Add(DataClass.GetParameter("@type", "BindGender"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindDesignation"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "dropdownDesigntion", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public void insertlog(string v1, string message, string v2, string v3, string v4)
        {
            try
            {
                // OPTIONAL: write to file (recommended)
                string path = @"C:\Logs\AppLog.txt";

                if (!Directory.Exists(@"C:\Logs"))
                    Directory.CreateDirectory(@"C:\Logs");

                string logText =
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " +
                    v1 + " | " +
                    message + " | " +
                    v2 + " | " +
                    v3 + " | " +
                    v4 + Environment.NewLine;

                File.AppendAllText(path, logText);
            }
            catch(Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "insertlog", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public List<CompanyLogoDO> GetCompanyLogoByUser(int userId)
        {
            List<CompanyLogoDO> listdata = new List<CompanyLogoDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_UserId", userId));

                listdata = getDrtolistParam.getdatafromreder<CompanyLogoDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_GetCompanyLogoByUserId")
                ).ToList();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "GetCompanyLogoByUser", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public List<DropDownData> dropdowCompany()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_DropdownCompanyName"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdowCompany", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdowterminationReason()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_termination_reasons"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdowterminationReason", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownassignby()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindassignby"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownassignby",
                    "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownEmpexporIntern()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> MySqlParameter = new List<MySqlParameter>();

                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(MySqlParameter, DBName, "sp_BindEmployeeExpOrIntern"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownEmpexporIntern", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        // Secondary DB dropdowns for Employee Registration flow
        public List<DropDownData> dropdownDesignationSecondary()
        {
            return GetDropDownDataFromSecondary("sp_bindDesignations");
        }

        public List<DropDownData> dropdowCompanySecondary()
        {
            return GetDropDownDataFromSecondary("Sp_BindDropdownCompany");
        }

        public List<DropDownData> dropdownassignbySecondary()
        {
            return GetDropDownDataFromSecondary("sp_bindassignby");
        }

        public List<DropDownData> dropdownEmpexporInternSecondary()
        {
            return GetDropDownDataFromSecondary("sp_BindEmployeeExpOrIntern");
        }

        private List<DropDownData> GetDropDownDataFromSecondary(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return data;
            }

            Exception mysqlEx = null;
            Exception sqlEx = null;
            try
            {
                data = GetDropDownDataFromSecondaryMySql(spName);
            }
            catch (Exception ex)
            {
                mysqlEx = ex;
            }

            if (data != null && data.Count > 0)
            {
                return data;
            }

            try
            {
                data = GetDropDownDataFromSecondarySql(spName);
            }
            catch (Exception ex)
            {
                sqlEx = ex;
            }

            if ((data == null || data.Count == 0) && (mysqlEx != null || sqlEx != null))
            {
                fnStoreErrorLog(
                    "CommonBL",
                    "GetDropDownDataFromSecondary",
                    "SP=" + spName
                    + " | MySqlError=" + (mysqlEx != null ? mysqlEx.Message : "none")
                    + " | SqlError=" + (sqlEx != null ? sqlEx.Message : "none"),
                    UserId
                );
            }

            return data;
        }

        private List<DropDownData> GetDropDownDataFromSecondaryMySql(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            string normalized = NormalizeMySqlConnectionString(Sqlconnection);
            using (MySqlConnection con = new MySqlConnection(normalized))
            using (MySqlCommand cmd = new MySqlCommand(spName, con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    data = MapDropDownData(dr);
                }
            }
            return data;
        }

        private List<DropDownData> GetDropDownDataFromSecondarySql(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            using (SqlConnection con = new SqlConnection(Sqlconnection))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    data = MapDropDownData(dr);
                }
            }
            return data;
        }

        private List<DropDownData> MapDropDownData(System.Data.IDataReader dr)
        {
            List<DropDownData> items = new List<DropDownData>();
            while (dr.Read())
            {
                string id = string.Empty;
                string text = string.Empty;

                id = GetReaderValue(dr, new[] { "Id", "id", "ID", "value", "Value", "designation_id", "company_id", "user_id", "emp_id" });
                text = GetReaderValue(dr, new[] { "Text", "text", "TEXT", "name", "Name", "designation_name", "company_name", "username", "user_fullname", "employee_type" });

                if (string.IsNullOrWhiteSpace(id) && dr.FieldCount > 0)
                {
                    id = Convert.ToString(dr[0]);
                }
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (dr.FieldCount > 1)
                    {
                        text = Convert.ToString(dr[1]);
                    }
                    else
                    {
                        text = id;
                    }
                }

                items.Add(new DropDownData
                {
                    Id = ParseInt(id),
                    Text = text
                });
            }
            return items;
        }

        private string GetReaderValue(System.Data.IDataRecord record, string[] columnNames)
        {
            foreach (string col in columnNames)
            {
                for (int i = 0; i < record.FieldCount; i++)
                {
                    if (string.Equals(record.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    {
                        object v = record[i];
                        return v == DBNull.Value ? string.Empty : Convert.ToString(v);
                    }
                }
            }
            return string.Empty;
        }

        private int ParseInt(string value)
        {
            int parsed = 0;
            int.TryParse(Convert.ToString(value), out parsed);
            return parsed;
        }

        private string NormalizeMySqlConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                if (builder.Port == 0)
                {
                    builder.Port = 3306;
                }
                return builder.ConnectionString;
            }
            catch
            {
                NameValueCollection parts = ParseConnectionString(connectionString);
                var builder = new MySqlConnectionStringBuilder();

                string server = GetConnectionValue(parts, "Server", "Data Source", "Datasource", "Host");
                string database = GetConnectionValue(parts, "Database", "Initial Catalog");
                string user = GetConnectionValue(parts, "User Id", "UserID", "uid", "User");
                string password = GetConnectionValue(parts, "Password", "Pwd");
                string portText = GetConnectionValue(parts, "Port");

                builder.Server = string.IsNullOrWhiteSpace(server) ? "localhost" : server;
                builder.Database = database ?? string.Empty;
                builder.UserID = user ?? string.Empty;
                builder.Password = password ?? string.Empty;
                builder.Port = uint.TryParse(portText, out uint parsedPort) ? parsedPort : 3306;
                builder.PersistSecurityInfo = true;
                builder.ConvertZeroDateTime = true;

                return builder.ConnectionString;
            }
        }

        private NameValueCollection ParseConnectionString(string connectionString)
        {
            NameValueCollection values = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            string[] segments = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string segment in segments)
            {
                int idx = segment.IndexOf('=');
                if (idx <= 0) continue;
                string key = segment.Substring(0, idx).Trim();
                string value = segment.Substring(idx + 1).Trim();
                values[key] = value;
            }
            return values;
        }

        private string GetConnectionValue(NameValueCollection values, params string[] keys)
        {
            foreach (string key in keys)
            {
                string value = values[key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }

        public List<DropDownData> dropdownEmployeeCode_ForRenumeration()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindEmployeeCodeFor_renumeration"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownEmployeeCode_ForRenumeration", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
    }
}
