using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Pqc.Crypto.Hqc;


namespace ProcessModel
{
    public class UserDetailsBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;
        public int Getpage(int userId, string queryString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(MySqlconnection))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand("sp_getuserIdwisepagaccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add(new MySqlParameter("p_userid", MySqlDbType.Int32) { Value = userId });
                        command.Parameters.Add(new MySqlParameter("p_pagename", MySqlDbType.VarChar, 100) { Value = queryString });

                        // Output parameter
                        MySqlParameter parameterStatus = new MySqlParameter("p_status", MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(parameterStatus);

                        // Execute
                        command.ExecuteNonQuery();

                        int status = Convert.ToInt32(parameterStatus.Value);

                        return status;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDetailsBL", "Getpage", "Exception Message: " + ex.Message + " Strace=" + ex.StackTrace, UserId);

                return -1;
            }
        }

        public List<UserDetailsDO> SaveUserDetails(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                // Keep the parameter sequence aligned with Sp_insertuser definition.
                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertUser"));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_password", user.password));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_insertedby", user.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("p_user_type", user.user_type));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_type", user.employee_type));

                // Registration insert is directed to secondary DB SP/table for current employee-only flow.
                listdata = SaveUserDetailsUsingSqlConnection(mysqlParameters);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public List<UserDetailsDO> SaveUserDetailsMainDb(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertUser"));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_password", user.password));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_insertedby", user.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("p_user_type", user.user_type));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_type", user.employee_type));

                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_insertuser")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetailsMainDb",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User save did not return any response from database."
                    }
                };
            }

            return listdata;
        }

        private bool IsSuccessResponse(List<UserDetailsDO> response)
        {
            if (response == null || response.Count == 0)
            {
                return false;
            }

            string status = Convert.ToString(response[0].Status ?? string.Empty).Trim();
            return string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase);
        }

        private List<UserDetailsDO> SaveUserDetailsUsingSqlConnection(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listdata;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
                using (MySqlCommand command = new MySqlCommand("Sp_insertuser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;

                    foreach (var parameter in mysqlParameters)
                    {
                        command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value ?? DBNull.Value);
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listdata.Add(new UserDetailsDO
                            {
                                Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Failed",
                                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetailsUsingSqlConnection",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User save did not return any response from database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetails(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                 mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
                mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

                var primaryResult = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updateuser")
                );
                if (primaryResult != null && primaryResult.Count > 0)
                {
                    listdata = primaryResult;
                }

                List<UserDetailsDO> secondaryResult = new List<UserDetailsDO>();
                try
                {
                    secondaryResult = UpdateUserDetailsUsingSqlConnection(mysqlParameters);
                }
                catch
                {
                    secondaryResult = new List<UserDetailsDO>();
                }
                bool primarySuccess = IsSuccessResponse(primaryResult);
                bool secondarySuccess = IsSuccessResponse(secondaryResult);

                if (primarySuccess && secondarySuccess)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "User updated successfully in both databases."
                        }
                    };
                }
                else if (primarySuccess && !secondarySuccess)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "User updated in primary database. Secondary database response not received."
                        }
                    };
                }
                else
                {
                    listdata = primaryResult;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "UpdateUserDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
                if (listdata == null || listdata.Count == 0)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Failed",
                            Remarks = "User update failed due to exception."
                        }
                    };
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetailsMainDb(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
                mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updateuser")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "UpdateUserDetailsMainDb",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from primary database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetailsSecondary(UserDetailsDO user)
        {
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
            mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
            mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
            mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
            mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
            mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
            mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
            mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
            mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
            mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
            mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
            mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
            mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
            mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
            mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
            mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
            mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
            mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
            mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

            List<UserDetailsDO> secondaryResult = UpdateUserDetailsUsingSqlConnection(mysqlParameters);
            if (secondaryResult == null || secondaryResult.Count == 0)
            {
                secondaryResult = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from secondary database."
                    }
                };
            }

            return secondaryResult;
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSqlConnection(List<MySqlParameter> mysqlParameters)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return new List<UserDetailsDO>();
            }

            // Secondary DB can have a different user_id for the same employee.
            // Resolve secondary user_id from employee code and use it for update.
            try
            {
                string employeeCode = Convert.ToString(
                    mysqlParameters.FirstOrDefault(p =>
                        string.Equals(p.ParameterName, "p_employee_code", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.ParameterName, "@p_employee_code", StringComparison.OrdinalIgnoreCase)
                    )?.Value
                );
                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    int secondaryUserId = GetSecondaryUserIdByEmployeeCode(employeeCode);
                    if (secondaryUserId > 0)
                    {
                        var userIdParam = mysqlParameters.FirstOrDefault(p =>
                            string.Equals(p.ParameterName, "p_user_id", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(p.ParameterName, "@p_user_id", StringComparison.OrdinalIgnoreCase)
                        );
                        if (userIdParam != null)
                        {
                            userIdParam.Value = secondaryUserId;
                        }
                    }
                }
            }
            catch
            {
                // Keep fallback execution; update may still succeed with incoming user_id.
            }

            try
            {
                // Try MySQL-compatible execution first.
                return UpdateUserDetailsUsingSecondaryMySql(mysqlParameters);
            }
            catch
            {
                // Fallback to SQL Server execution for Sqlconnection strings.
                return UpdateUserDetailsUsingSecondarySql(mysqlParameters);
            }
        }

        private int GetSecondaryUserIdByEmployeeCode(string employeeCode)
        {
            try
            {
                List<UserDetailsDO> secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);
                UserDetailsDO match = secondaryUsers
                    .FirstOrDefault(u => string.Equals(
                        (u.EmployeeCode ?? string.Empty).Trim(),
                        (employeeCode ?? string.Empty).Trim(),
                        StringComparison.OrdinalIgnoreCase));
                if (match != null && match.UserId > 0)
                {
                    return match.UserId;
                }
            }
            catch
            {
            }
            return 0;
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSecondaryMySql(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);

            using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
            using (MySqlCommand command = new MySqlCommand("Sp_updateuser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                foreach (var parameter in mysqlParameters)
                {
                    string pName = parameter.ParameterName;
                    if (string.Equals(pName, "p_employee_code", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(pName, "@p_employee_code", StringComparison.OrdinalIgnoreCase))
                    {
                        pName = "p_empcode";
                    }
                    command.Parameters.AddWithValue(pName, parameter.Value ?? DBNull.Value);
                }

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    bool hasAnyRow = false;
                    while (reader.Read())
                    {
                        hasAnyRow = true;
                        listdata.Add(new UserDetailsDO
                        {
                            Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Failed",
                            Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : string.Empty
                        });
                    }

                    if (!hasAnyRow)
                    {
                        // Some SP variants don't return Status/Remarks; treat successful execution as success.
                        listdata.Add(new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "Updated in secondary database."
                        });
                    }
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User update did not return any response from secondary database."
                    }
                };
            }

            return listdata;
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSecondarySql(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            using (SqlConnection connection = new SqlConnection(Sqlconnection))
            using (SqlCommand command = new SqlCommand("Sp_updateuser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                foreach (var parameter in mysqlParameters)
                {
                    string paramName = parameter.ParameterName;
                    if (!paramName.StartsWith("@"))
                    {
                        paramName = "@" + paramName.TrimStart('@');
                    }
                    if (string.Equals(paramName, "@p_employee_code", StringComparison.OrdinalIgnoreCase))
                    {
                        paramName = "@p_empcode";
                    }
                    command.Parameters.AddWithValue(paramName, parameter.Value ?? DBNull.Value);
                }

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    bool hasAnyRow = false;
                    while (reader.Read())
                    {
                        hasAnyRow = true;
                        string status = "Failed";
                        string remarks = string.Empty;
                        if (HasColumn(reader, "Status") && reader["Status"] != DBNull.Value)
                        {
                            status = Convert.ToString(reader["Status"]);
                        }
                        if (HasColumn(reader, "Remarks") && reader["Remarks"] != DBNull.Value)
                        {
                            remarks = Convert.ToString(reader["Remarks"]);
                        }
                        listdata.Add(new UserDetailsDO { Status = status, Remarks = remarks });
                    }

                    if (!hasAnyRow)
                    {
                        // Some SP variants don't return Status/Remarks; treat successful execution as success.
                        listdata.Add(new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "Updated in secondary database."
                        });
                    }
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User update did not return any response from secondary database."
                    }
                };
            }

            return listdata;
        }

        private bool HasColumn(IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public List<UserDetailsDO> ViewAllUsers()
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                var secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);
                users = (secondaryUsers ?? new List<UserDetailsDO>())
                    .GroupBy(u =>
                        (
                            (u.EmployeeCode ?? string.Empty).Trim().ToUpper() + "|" +
                            (u.Username ?? string.Empty).Trim().ToUpper() + "|" +
                            (u.user_mail_id ?? string.Empty).Trim().ToUpper()
                        ))
                    .Select(g => g.OrderByDescending(x => x.UserId).First())
                    .OrderByDescending(u => u.UserId)
                    .ToList();

                EnrichDesignationName(users);

                if (users == null || users.Count == 0)
                {
                    users = GetAllUsersFromConnection(MySqlconnection, false);
                    EnrichDesignationName(users);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "ViewAllUsers", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                users = GetAllUsersFallbackFromPrimary();
            }
            return users;
        }

        public List<UserDetailsDO> ViewAllUsersMainDb()
        {
            List<UserDetailsDO> users = GetAllUsersFallbackFromPrimary();
            EnrichDesignationName(users);
            return users;
        }

        private void EnrichDesignationName(List<UserDetailsDO> users)
        {
            if (users == null || users.Count == 0)
            {
                return;
            }

            try
            {
                var designationMap = GetDesignationMapFromConnections();
                if (designationMap == null || designationMap.Count == 0)
                {
                    return;
                }

                foreach (var user in users)
                {
                    if (!string.IsNullOrWhiteSpace(user.designation_name))
                    {
                        continue;
                    }

                    if (user.designation_id <= 0)
                    {
                        continue;
                    }

                    string designationText;
                    if (designationMap.TryGetValue(user.designation_id.ToString(), out designationText))
                    {
                        user.designation_name = designationText;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "EnrichDesignationName", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        private Dictionary<string, string> GetDesignationMapFromConnections()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Action<string> loadFromMySql = (connectionString) =>
            {
                if (string.IsNullOrWhiteSpace(connectionString)) return;
                string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_bindDesignation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr["Id"] != DBNull.Value ? Convert.ToString(dr["Id"]).Trim() : string.Empty;
                            string text = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]).Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(text) && !map.ContainsKey(id))
                            {
                                map[id] = text;
                            }
                        }
                    }
                }
            };

            Action<string> loadFromSql = (connectionString) =>
            {
                if (string.IsNullOrWhiteSpace(connectionString)) return;
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_bindDesignation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr["Id"] != DBNull.Value ? Convert.ToString(dr["Id"]).Trim() : string.Empty;
                            string text = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]).Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(text) && !map.ContainsKey(id))
                            {
                                map[id] = text;
                            }
                        }
                    }
                }
            };

            try { loadFromMySql(MySqlconnection); } catch { }
            try { loadFromMySql(Sqlconnection); } catch { try { loadFromSql(Sqlconnection); } catch { } }

            return map;
        }

        private List<UserDetailsDO> GetAllUsersFallbackFromPrimary()
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
                sqlParameters.Add(DataClass.GetParameter("@p_type", "GetAllUser"));
                users = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "Sp_getalluser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFallbackFromPrimary", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return users;
        }

        private List<UserDetailsDO> GetAllUsersFromConnection(string connectionString, bool isSecondarySource)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return users;
            }

            try
            {
                string spName = isSecondarySource ? "Sp_getalluser_hrms" : "Sp_getalluser";
                string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_type", "GetAllUser");
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                        {
                            for (int i = 0; i < rec.FieldCount; i++)
                            {
                                if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                                    return true;
                            }
                            return false;
                        };

                        Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                        {
                            foreach (var c in cols)
                            {
                                if (hasCol(rec, c))
                                {
                                    var v = rec[c];
                                    return v == DBNull.Value ? null : v;
                                }
                            }
                            return null;
                        };

                        while (dr.Read())
                        {
                            object userIdVal = getVal(dr, new[] { "UserId", "user_id", "id" });
                            object empCodeVal = getVal(dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                            object usernameVal = getVal(dr, new[] { "Username", "username", "user_name" });
                            object fullNameVal = getVal(dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                            object emailVal = getVal(dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                            object contactVal = getVal(dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                            object companyVal = getVal(dr, new[] { "company_id", "CompanyId", "companyid" });
                            object designationNameVal = getVal(dr, new[] { "designation_name", "DesignationName", "designation" });
                            object designationIdVal = getVal(dr, new[] { "designation_id", "DesignationId", "designationid" });
                            string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                            string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                            int designationIdParsed = 0;
                            int.TryParse(designationIdRaw, out designationIdParsed);

                            if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                            {
                                designationName = designationIdRaw;
                            }
                            if (string.IsNullOrWhiteSpace(designationName))
                            {
                                designationName = ExtractDesignationText(dr);
                            }

                            users.Add(new UserDetailsDO
                            {
                                UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                                EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                                Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                                user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                                user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                                contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                                company_id = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                                CompanyId = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                                designation_name = designationName,
                                designation_id = designationIdParsed
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFromConnection", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

                // Fallback: if this connection is actually SQL Server, try SqlClient.
                try
                {
                    users = GetAllUsersFromSqlServerConnection(connectionString, isSecondarySource);
                    if ((users == null || users.Count == 0) && isSecondarySource)
                    {
                        users = GetAllUsersFromSqlServerConnection(connectionString, false);
                    }
                }
                catch (Exception ex2)
                {
                    errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFromSqlServerConnection", "Exception Message" + ex2.Message + "Strace=" + ex2.StackTrace, UserId);
                }
            }

            return users;
        }

        private List<UserDetailsDO> GetAllUsersFromSqlServerConnection(string connectionString, bool isSecondarySource)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            string spName = isSecondarySource ? "Sp_getalluser_hrms" : "Sp_getalluser";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "GetAllUser");
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                    {
                        for (int i = 0; i < rec.FieldCount; i++)
                        {
                            if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                                return true;
                        }
                        return false;
                    };

                    Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                    {
                        foreach (var c in cols)
                        {
                            if (hasCol(rec, c))
                            {
                                var v = rec[c];
                                return v == DBNull.Value ? null : v;
                            }
                        }
                        return null;
                    };

                    while (dr.Read())
                    {
                        object userIdVal = getVal(dr, new[] { "UserId", "user_id", "id" });
                        object empCodeVal = getVal(dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                        object usernameVal = getVal(dr, new[] { "Username", "username", "user_name" });
                        object fullNameVal = getVal(dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                        object emailVal = getVal(dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                        object contactVal = getVal(dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                        object companyVal = getVal(dr, new[] { "company_id", "CompanyId", "companyid" });
                        object designationNameVal = getVal(dr, new[] { "designation_name", "DesignationName", "designation" });
                        object designationIdVal = getVal(dr, new[] { "designation_id", "DesignationId", "designationid" });
                        string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                        string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                        int designationIdParsed = 0;
                        int.TryParse(designationIdRaw, out designationIdParsed);

                        if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                        {
                            designationName = designationIdRaw;
                        }
                        if (string.IsNullOrWhiteSpace(designationName))
                        {
                            designationName = ExtractDesignationText(dr);
                        }

                        users.Add(new UserDetailsDO
                        {
                            UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                            EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                            Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                            user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                            user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                            contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                            company_id = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                            CompanyId = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                            designation_name = designationName,
                            designation_id = designationIdParsed
                        });
                    }
                }
            }
            return users;
        }

        private string ExtractDesignationText(IDataRecord record)
        {
            try
            {
                for (int i = 0; i < record.FieldCount; i++)
                {
                    string col = record.GetName(i);
                    if (string.IsNullOrWhiteSpace(col))
                    {
                        continue;
                    }

                    if (!col.ToLowerInvariant().Contains("designation"))
                    {
                        continue;
                    }

                    object raw = record[i];
                    if (raw == null || raw == DBNull.Value)
                    {
                        continue;
                    }

                    string val = Convert.ToString(raw).Trim();
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }

                    int temp;
                    if (int.TryParse(val, out temp))
                    {
                        continue;
                    }

                    return val;
                }
            }
            catch
            {
                // keep silent; caller already has normal fallbacks
            }

            return string.Empty;
        }

        private string NormalizeMySqlConnectionString(string rawConnectionString)
        {
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                return rawConnectionString;
            }

            try
            {
                // Convert mixed/legacy keys (Data Source, Initial Catalog, uid) into MySQL-compatible keys.
                var parts = rawConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var part in parts)
                {
                    int idx = part.IndexOf('=');
                    if (idx <= 0) continue;
                    string key = part.Substring(0, idx).Trim();
                    string value = part.Substring(idx + 1).Trim();
                    map[key] = value;
                }

                var builder = new MySqlConnectionStringBuilder();
                if (map.ContainsKey("Server")) builder.Server = map["Server"];
                else if (map.ContainsKey("Data Source")) builder.Server = map["Data Source"];
                else if (map.ContainsKey("Datasource")) builder.Server = map["Datasource"];

                if (map.ContainsKey("Database")) builder.Database = map["Database"];
                else if (map.ContainsKey("Initial Catalog")) builder.Database = map["Initial Catalog"];

                if (map.ContainsKey("Port") && uint.TryParse(map["Port"], out uint p)) builder.Port = p;

                if (map.ContainsKey("User Id")) builder.UserID = map["User Id"];
                else if (map.ContainsKey("UserID")) builder.UserID = map["UserID"];
                else if (map.ContainsKey("Uid")) builder.UserID = map["Uid"];
                else if (map.ContainsKey("User")) builder.UserID = map["User"];
                else if (map.ContainsKey("Username")) builder.UserID = map["Username"];

                if (map.ContainsKey("Password")) builder.Password = map["Password"];
                else if (map.ContainsKey("Pwd")) builder.Password = map["Pwd"];

                if (map.ContainsKey("Persist Security Info"))
                {
                    if (bool.TryParse(map["Persist Security Info"], out bool persist))
                    {
                        builder.PersistSecurityInfo = persist;
                    }
                }

                return builder.ConnectionString;
            }
            catch
            {
                return rawConnectionString;
            }
        }
        public List<UserDetailsDO> AdvanceSearch(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            try
            {
                sqlParameters.Add(new MySqlParameter("@p_usernameId", user.usernameId ?? (object)DBNull.Value));
                sqlParameters.Add(new MySqlParameter("@p_contact_detail", user.contact_detail ?? (object)DBNull.Value));
                sqlParameters.Add(new MySqlParameter("@p_empcodeId", user.empcodeId ?? (object)DBNull.Value));

                using (var reader = DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "sp_SearchAdvUser"))
                {
                    listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(reader);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "AdvanceSearch",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }
        public List<UserDetailsDO> GetUserDetails(int userId)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            try
            {
                string type = "";
                if (userId != 0)
                {
                    type = "GetUser";

                }
                else
                {
                    type = "GetNewEmployeeId";
                }

                sqlParameters.Add(DataClass.GetParameter("@p_type", type));
                sqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));
                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "Sp_getuser1"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetUserDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                throw ex;
            }
            return listdata;
        }

        public List<UserDetailsDO> GetUserDetailsFromSecondary(int userId, string employeeCode)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            try
            {
                // Try MySQL-compatible execution first with multiple parameter variants.
                try
                {
                    listdata = TryGetSecondaryDetailsMySql(userId, employeeCode);
                }
                catch
                {
                    // Fallback to SQL Server execution if secondary DB is SQL.
                    listdata = TryGetSecondaryDetailsSql(userId, employeeCode);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetUserDetailsFromSecondary", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        private List<UserDetailsDO> TryGetSecondaryDetailsMySql(int userId, string employeeCode)
        {
            string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
            var attempts = new List<Action<MySqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_empcode", employeeCode); },
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); }
            };

            foreach (var setup in attempts)
            {
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("Sp_getuserdetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var list = MapUserDetailsFromReader(dr);
                        if (list != null && list.Count > 0)
                        {
                            return list;
                        }
                    }
                }
            }

            return new List<UserDetailsDO>();
        }

        private List<UserDetailsDO> TryGetSecondaryDetailsSql(int userId, string employeeCode)
        {
            var attempts = new List<Action<SqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_empcode", employeeCode); },
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); }
            };

            foreach (var setup in attempts)
            {
                using (SqlConnection con = new SqlConnection(Sqlconnection))
                using (SqlCommand cmd = new SqlCommand("Sp_getuserdetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var list = MapUserDetailsFromReader(dr);
                        if (list != null && list.Count > 0)
                        {
                            return list;
                        }
                    }
                }
            }

            return new List<UserDetailsDO>();
        }

        private List<UserDetailsDO> MapUserDetailsFromReader(IDataReader dr)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            while (dr.Read())
            {
                Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                {
                    for (int i = 0; i < rec.FieldCount; i++)
                    {
                        if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    return false;
                };

                Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                {
                    foreach (var c in cols)
                    {
                        if (hasCol(rec, c))
                        {
                            var v = rec[c];
                            return v == DBNull.Value ? null : v;
                        }
                    }
                    return null;
                };

                object userIdVal = getVal((IDataRecord)dr, new[] { "UserId", "user_id", "id" });
                object empCodeVal = getVal((IDataRecord)dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                object usernameVal = getVal((IDataRecord)dr, new[] { "Username", "username", "user_name" });
                object fullNameVal = getVal((IDataRecord)dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                object emailVal = getVal((IDataRecord)dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                object contactVal = getVal((IDataRecord)dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                object companyVal = getVal((IDataRecord)dr, new[] { "company_id", "CompanyId", "companyid", "comp_id" });
                object companyNameVal = getVal((IDataRecord)dr, new[] { "company_name", "CompanyName", "comp_name" });
                object esicVal = getVal((IDataRecord)dr, new[] { "ESIC_no", "esic_no", "ESICNo", "esicno" });
                object pfVal = getVal((IDataRecord)dr, new[] { "PF_no", "pf_no", "PFNo", "pfno" });
                object deptVal = getVal((IDataRecord)dr, new[] { "department", "Department", "dept", "dept_name" });
                object branchVal = getVal((IDataRecord)dr, new[] { "branch", "Branch", "branch_name" });
                object divVal = getVal((IDataRecord)dr, new[] { "division", "Division", "div", "division_name" });
                object dojVal = getVal((IDataRecord)dr, new[] { "date_of_joining", "DateOfJoining", "joining_date", "doj" });
                object probVal = getVal((IDataRecord)dr, new[] { "probation_period_months", "ProbationPeriodMonths", "probation_period", "probation_months" });
                object mgrVal = getVal((IDataRecord)dr, new[] { "reporting_manager", "ReportingManager", "reportingmanager", "manager_id", "manager" });
                object typeVal = getVal((IDataRecord)dr, new[] { "employee_type", "EmployeeType", "employment_type", "emp_type" });
                object designationNameVal = getVal((IDataRecord)dr, new[] { "designation_name", "DesignationName", "designation" });
                object designationIdVal = getVal((IDataRecord)dr, new[] { "designation_id", "DesignationId", "designationid" });

                string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                int designationIdParsed = 0;
                int.TryParse(designationIdRaw, out designationIdParsed);
                if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                {
                    designationName = designationIdRaw;
                }

                users.Add(new UserDetailsDO
                {
                    UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                    EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                    Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                    user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                    user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                    contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                    company_id = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                    CompanyId = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                    company_name = companyNameVal != null ? Convert.ToString(companyNameVal) : string.Empty,
                    ESIC_no = esicVal != null ? Convert.ToInt32(esicVal) : 0,
                    PF_no = pfVal != null ? Convert.ToInt32(pfVal) : 0,
                    department = deptVal != null ? Convert.ToString(deptVal) : string.Empty,
                    branch = branchVal != null ? Convert.ToString(branchVal) : string.Empty,
                    division = divVal != null ? Convert.ToString(divVal) : string.Empty,
                    date_of_joining = dojVal != null ? Convert.ToDateTime(dojVal) : DateTime.MinValue,
                    probation_period_months = probVal != null ? Convert.ToInt32(probVal) : 0,
                    reporting_manager = mgrVal != null ? Convert.ToString(mgrVal) : string.Empty,
                    employee_type = typeVal != null ? Convert.ToString(typeVal) : string.Empty,
                    designation_name = designationName,
                    designation_id = designationIdParsed
                });
            }

            return users;
        }
    
        public UserDetailsDO DeactivateUser(int userId, string employeeCode = "")
        {
            int secondaryUserId = ResolveSecondaryUserIdForDelete(userId, employeeCode);
            UserDetailsDO secondaryResult = DeactivateUserByConnection(Sqlconnection, secondaryUserId, "DeactivateUserSecondary");
            if (secondaryResult == null)
            {
                return new UserDetailsDO { Status = "Failed", Remarks = "Soft delete failed in secondary database." };
            }
            return secondaryResult;
        }

        private UserDetailsDO DeactivateUserByConnection(string connectionString, int userId, string methodName)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                result.Remarks = "Connection string not configured.";
                return result;
            }

            try
            {
                result = DeactivateUserByConnectionMySql(connectionString, userId);
            }
            catch
            {
                try
                {
                    result = DeactivateUserByConnectionSql(connectionString, userId);
                }
                catch (Exception ex)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "UserDetailsBL",
                        methodName,
                        "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                        UserId
                    );
                    result.Status = "Failed";
                    result.Remarks = "Soft delete execution failed.";
                }
            }

            return result;
        }

        private UserDetailsDO DeactivateUserByConnectionMySql(string connectionString, int userId)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
            var attempts = new List<Action<MySqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_userid", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); }
            };

            foreach (var setup in attempts)
            {
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("Sp_deleteuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            result.Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : "Success";
                            result.Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString() : "User deleted successfully.";
                        }
                        else
                        {
                            result.Status = "Success";
                            result.Remarks = "User deleted successfully.";
                        }
                    }
                    if (string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private UserDetailsDO DeactivateUserByConnectionSql(string connectionString, int userId)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            var attempts = new List<Action<SqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_userid", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); }
            };

            foreach (var setup in attempts)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("Sp_deleteuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string status = "Success";
                            string remarks = "User deleted successfully.";
                            if (HasColumn(dr, "Status") && dr["Status"] != DBNull.Value)
                            {
                                status = Convert.ToString(dr["Status"]);
                            }
                            if (HasColumn(dr, "Remarks") && dr["Remarks"] != DBNull.Value)
                            {
                                remarks = Convert.ToString(dr["Remarks"]);
                            }
                            result.Status = status;
                            result.Remarks = remarks;
                        }
                        else
                        {
                            result.Status = "Success";
                            result.Remarks = "User deleted successfully.";
                        }
                    }
                    if (string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private int ResolveSecondaryUserIdForDelete(int incomingUserId, string employeeCode)
        {
            try
            {
                List<UserDetailsDO> secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);

                if (string.IsNullOrWhiteSpace(employeeCode))
                {
                    List<UserDetailsDO> primaryDetails = GetUserDetails(incomingUserId);
                    if (primaryDetails != null && primaryDetails.Count > 0)
                    {
                        employeeCode = Convert.ToString(primaryDetails[0].EmployeeCode ?? string.Empty).Trim();
                    }
                }

                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    UserDetailsDO byCode = secondaryUsers.FirstOrDefault(u =>
                        string.Equals(
                            Convert.ToString(u.EmployeeCode ?? string.Empty).Trim(),
                            employeeCode,
                            StringComparison.OrdinalIgnoreCase));
                    if (byCode != null && byCode.UserId > 0)
                    {
                        return byCode.UserId;
                    }
                }

                if (incomingUserId > 0)
                {
                    UserDetailsDO direct = secondaryUsers.FirstOrDefault(u => u.UserId == incomingUserId);
                    if (direct != null)
                    {
                        return incomingUserId;
                    }
                }
            }
            catch
            {
            }

            return incomingUserId;
        }

        public void SendUserCredentialsMail(string emailId, string password)
        {
            try
            {
                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];
                string subject = "HRMS Credentials";
                string body = $"Your Login Password for HRMS is Password: {password}";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS");
                    mail.To.Add(emailId);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDetailsBl", "SendUserCredentialsMail", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

    }
}
