using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Authentication
{
    public partial class forgetPassword : System.Web.UI.Page
    {
        protected string UserId = null;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_Emailsend_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_sendmail.Text == "Send OTP")
                {
                    changePassDO login = new changePassDO();
                    LoginBAL loginBAL = new LoginBAL();
                    login.user_mail_id = emailtxt.Text;
                    string emailId = login.user_mail_id;

                    List<changePassDO> listdata = loginBAL.UserMailPass(login);

                    if (listdata.Count > 0 && listdata[0].result == "1")
                    {
                        btn_sendmail.Text = "Verify OTP";
                        div_otp.Visible = true;

                        string un = listdata[0].userId;
                        if (!string.IsNullOrEmpty(un))
                        {
                            // Store userId in session
                            Session["userId"] = Convert.ToInt32(un);

                            string otp = listdata[0].generated_otp;
                            Session["verificationCode"] = otp; // Store backend OTP in session

                            loginBAL.SendVerificationCodeEmail(emailId, otp);

                            string status = "Success";
                            string remark = "OTP Generated Successfully.";
                            ClientScript.RegisterStartupScript(
                                this.GetType(),
                                "ShowRequiredFieldsScript",
                                $"showDataSavedMessage('{status}', '{remark}');",
                                true
                            );
                        }
                        else
                        {
                            Response.Redirect("/View/Authentication/setPassword.aspx", false);
                        }
                    }
                    else
                    {
                        lblErrorMessager.Text = "mail-id is incorrect";
                        divAlert.Visible = true;
                    }
                }
                else if (btn_sendmail.Text == "Verify OTP")
                {
                    // Compare entered OTP with session
                    string enteredOtp = txt_verifyotp.Text.Trim();
                    string sessionOtp = Session["verificationCode"] != null ? Session["verificationCode"].ToString() : "";

                    if (enteredOtp == sessionOtp)
                    {
                        string userId = Session["userId"]?.ToString() ?? "0";
                        Response.Redirect("/View/Authentication/setPassword.aspx?unid=" + userId, false);
                    }
                    else
                    {
                        lblErrorMessager.Text = "OTP Is Invalid";
                        divAlert.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("btn_send_Click", "verifyOTp", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}