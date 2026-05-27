using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Org.BouncyCastle.Pqc.Crypto.Hqc;

namespace ProcessModel
{
    public class LoginBAL
    {
        protected string UserId = null;

        private string DBName = ConfigurationManager.AppSettings["DBName"];
        public LoginDO UserLogin(LoginDO login)
        {
            LoginDO lstLoginDetails = new LoginDO();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("p_Username", login.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_Password", login.Password));
                //mysqlParameters.Add(DataClass.GetParameter("p_type", "SignIn"));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_userId", MySqlDbType.VarChar, 100));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_User_name", MySqlDbType.VarChar, 200));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_User_role", MySqlDbType.VarChar, 200));
                //mysqlParameters.Add(DataClass.GetOutputParameter("p_Password", MySqlDbType.VarChar, 200));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_Result", MySqlDbType.VarChar, 100));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_Remark", MySqlDbType.VarChar, 200));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_PassResetFlag", MySqlDbType.VarChar, 100));

                lstLoginDetails = (from ii in getDrtolistParam.getdatafromreder<LoginDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, "alpha_hrms", "sp_authentication"))
                                   select ii).FirstOrDefault();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("LoginBAL", "UserLogin", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return lstLoginDetails;
        }
        //public List<LoginBO> UserLogin(LoginBO login)
        //{
        //    List<LoginBO> listdata = new List<LoginBO>();
        //    try
        //    {
        //        getDrtolist getDrtolistParam = new getDrtolist();
        //          List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //        mysqlParameters.Add(DataClass.GetParameter("@p_Username", login.Username));
        //        mysqlParameters.Add(DataClass.GetParameter("p_Password", login.Password));
        //        //mysqlParameters.Add(DataClass.GetParameter("p_type", "SignIn"));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_userId", MySqlDbType.VarChar, 100));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_User_name", MySqlDbType.VarChar, 200));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_User_role", MySqlDbType.VarChar, 200));
        //        //mysqlParameters.Add(DataClass.GetOutputParameter("p_Password", MySqlDbType.VarChar, 200));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_Result", MySqlDbType.VarChar, 100));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_Remark", MySqlDbType.VarChar, 200));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_PassResetFlag", MySqlDbType.VarChar, 100));

        //        listdata = getDrtolistParam.getdatafromreder<LoginBO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_authentication"));
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("LoginBAL", "UserLogin", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //    return listdata;
        //}
        public List<changePassDO> UserMailPass(changePassDO login)
        {
            List<changePassDO> listdata = new List<changePassDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", login.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_type", "mailmatch"));
                mysqlParameters.Add(DataClass.GetParameter("p_user_otp", login.generated_otp ?? ""));
                mysqlParameters.Add(
                    DataClass.GetParameter("p_user_id", login.userId ?? "0")
                );
                mysqlParameters.Add(DataClass.GetOutputParameter("p_user_fullname", MySqlDbType.VarChar, 100));

                mysqlParameters.Add(DataClass.GetOutputParameter("p_userId", MySqlDbType.VarChar, 100));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_result", MySqlDbType.VarChar, 200));
                var outPassFlag = DataClass.GetOutputParameter("p_pass_resetflag", MySqlDbType.VarChar, 10);
                mysqlParameters.Add(outPassFlag);


                listdata = getDrtolistParam.getdatafromreder<changePassDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updatePasswordByEmail")
                );

                login.passresetflag = int.TryParse(outPassFlag.Value?.ToString(), out int flag) ? flag : (int?)null;
                foreach (var item in listdata)
                {
                    item.passresetflag = login.passresetflag;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "LoginBAL",
                    "UserMailPass",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }
        public List<changePassDO> GenerateOtpByUserId(int userId)
        {
            List<changePassDO> listdata = new List<changePassDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("p_type", "generateOTP"));  // ✅ FIXED
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", userId));
                mysqlParameters.Add(DataClass.GetParameter("p_user_otp", "")); // Not needed for generateOTP

                // Output params
                mysqlParameters.Add(DataClass.GetOutputParameter("p_user_fullname", MySqlDbType.VarChar, 100));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_userId", MySqlDbType.VarChar, 100));
                mysqlParameters.Add(DataClass.GetOutputParameter("p_result", MySqlDbType.VarChar, 200));
                var outPassFlag = DataClass.GetOutputParameter("p_pass_resetflag", MySqlDbType.VarChar, 10);
                mysqlParameters.Add(outPassFlag);

                listdata = getDrtolistParam.getdatafromreder<changePassDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updatePasswordByEmail1")
                );

                foreach (var item in listdata)
                {
                    item.passresetflag = int.TryParse(outPassFlag.Value?.ToString(), out int flag) ? flag : (int?)null;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "LoginBAL",
                    "GenerateOtpByUserId",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }


        public void SendVerificationCodeEmail(string email, string verificationCode)
        {
            try
            {
                string subject = "HRMS Application - Verification Code";
                string body = $"<h4>Your verification code is: <b>{verificationCode}</b></h4><p>It is valid for 15 minutes.</p>";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System"); 
                    mail.To.Add(email);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true; 

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("LoginBAL", "SendVerificationCodeEmail", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public List<LoginBO> forgetPassBal(LoginBO login)
        {
            List<LoginBO> listdata = new List<LoginBO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_userId", login.UserId));
                mysqlParameters.Add(DataClass.GetParameter("@p_password", login.Password));
                listdata = getDrtolistParam.getdatafromreder<LoginBO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_forgetpass"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("LoginBAL", "forgetPassBal", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public List<LoginBO> GetUserIdByUsername(LoginBO login)
        {
            List<LoginBO> listdata = new List<LoginBO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_Username", login.Username));
                listdata = getDrtolistParam.getdatafromreder<LoginBO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_GetUserIdByUsername"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("LoginBAL", "GetUserIdByUsername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public List<LoginBO> changePassBal(LoginBO login)
        {
            List<LoginBO> listdata = new List<LoginBO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_userId", login.UserId));
                mysqlParameters.Add(DataClass.GetParameter("@p_password", login.Password));
                mysqlParameters.Add(DataClass.GetParameter("@p_username", login.Username));
                mysqlParameters.Add(DataClass.GetParameter("@p_oldPassword", login.oldpassword));
                listdata = getDrtolistParam.getdatafromreder<LoginBO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_ChangePassword"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("LoginBAL", "changePassBal", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        public int GetCompanyIdByUserId(int userId)
        {
            int companyId = 0;
            try
            {
                if (userId <= 0)
                {
                    return 0;
                }

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));

                using (var reader = DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_company_by_userid"))
                {
                    if (reader != null && reader.Read())
                    {
                        if (HasColumn(reader, "company_id") && reader["company_id"] != DBNull.Value)
                        {
                            int.TryParse(Convert.ToString(reader["company_id"]), out companyId);
                        }
                    }
                }
            }
            catch
            {
                // Fallback: direct lookup from userM if SP not present.
                try
                {
                    string mysqlConnection = ConfigurationManager.ConnectionStrings["MysqlConnection"] != null
                        ? ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString
                        : string.Empty;
                    using (MySqlConnection con = new MySqlConnection(mysqlConnection))
                    using (MySqlCommand cmd = new MySqlCommand("SELECT company_id FROM userM WHERE user_id=@p_user_id AND is_active=1 LIMIT 1;", con))
                    {
                        cmd.Parameters.AddWithValue("@p_user_id", userId);
                        con.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value)
                        {
                            int.TryParse(Convert.ToString(obj), out companyId);
                        }
                    }
                }
                catch { }
            }
            return companyId;
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
        //public List<changePassDO> VerifyOTP(changePassDO login)
        //{
        //    List<changePassDO> listdata = new List<changePassDO>();
        //    try
        //    {


        //        getDrtolist getDrtolistParam = new getDrtolist();
        //        List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //        mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", login.user_mail_id));
        //        mysqlParameters.Add(DataClass.GetParameter("p_type", "mailmatch"));
        //        mysqlParameters.Add(DataClass.GetParameter("@p_user_otp", login.VerifyOTP));
        //        mysqlParameters.Add(
        //            DataClass.GetParameter("p_user_id", login.userId ?? "0")
        //        );
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_user_fullname", MySqlDbType.VarChar, 100));

        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_userId", MySqlDbType.VarChar, 100));
        //        mysqlParameters.Add(DataClass.GetOutputParameter("p_result", MySqlDbType.VarChar, 200));
        //        listdata = getDrtolistParam.getdatafromreder<changePassDO>(
        //            DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updatePasswordByEmail")
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("LoginBAL", "VerifyOTP",
        //            "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace, UserId);
        //    }
        //    return listdata;
        //}
    }
}
