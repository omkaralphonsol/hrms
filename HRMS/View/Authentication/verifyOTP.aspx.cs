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
    public partial class verifyOTP : System.Web.UI.Page
    {
        protected string UserId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    
                    int userId = Convert.ToInt32(Request.QueryString["unid"]);
                    LoginBAL loginBAL = new LoginBAL();

                    List<changePassDO> listdata = loginBAL.GenerateOtpByUserId(userId);

                    if (listdata.Count > 0 && listdata[0].result == "1")
                    {
                        Session["userId"] = Convert.ToInt32(listdata[0].userId);
                        Session["verificationCode"] = listdata[0].generated_otp;
                        Session["isPasswordReset"] = listdata[0].passresetflag;

                        loginBAL.SendVerificationCodeEmail(listdata[0].user_mail_id, listdata[0].generated_otp);

                        div_otp.Visible = true;
                        string status = "Success";
                                       string remark = "OTP generated successfully on your registered Email-ID";
                                      ClientScript.RegisterStartupScript(
                                         this.GetType(),
                                          "ShowRequiredFieldsScript",
                                          $"showDataSavedMessage('{status}', '{remark}');",
                                           true
                                      );
                    }
                    else
                    {
                        string status = "Success";
                        string remark = "Unable to send OTP. Please try again.";
                        ClientScript.RegisterStartupScript(
                           this.GetType(),
                            "ShowRequiredFieldsScript",
                            $"showDataSavedMessage('{status}', '{remark}');",
                             true
                        );
                    }
                }
                catch (Exception ex)
                {
                    lblErrorMessager.Text = "Error: " + ex.Message;
                    lblErrorMessager.CssClass = "text-danger";
                    lblErrorMessager.Visible = true;
                }
            }
        }



        //protected void btn_send_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LoginBAL loginBAL = new LoginBAL();

        //        if (btn_sendmail.Text == "Send OTP")
        //        {
        //            changePassDO login = new changePassDO
        //            {
        //                user_mail_id = emailtxt.Text.Trim()
        //            };
        //            string emailId = login.user_mail_id;

        //            List<changePassDO> listdata = loginBAL.UserMailPass(login);

        //            if (listdata.Count > 0 && listdata[0].result == "1" && !string.IsNullOrEmpty(listdata[0].userId))
        //            {
        //                btn_sendmail.Text = "Verify OTP";
        //                div_otp.Visible = true;

        //                Session["userId"] = Convert.ToInt32(listdata[0].userId);
        //                Session["verificationCode"] = listdata[0].generated_otp;
        //                Session["isPasswordReset"] = listdata[0].passresetflag;

        //                loginBAL.SendVerificationCodeEmail(emailId, listdata[0].generated_otp);

        //                string status = "Success";
        //                string remark = "OTP Generated Successfully.";
        //                ClientScript.RegisterStartupScript(
        //                    this.GetType(),
        //                    "ShowRequiredFieldsScript",
        //                    $"showDataSavedMessage('{status}', '{remark}');",
        //                    true
        //                );
        //            }
        //            else
        //            {
        //                lblErrorMessager.Text = "Mail ID is incorrect.";
        //                divAlert.Visible = true;
        //            }
        //        }
        //        else if (btn_sendmail.Text == "Verify OTP")
        //        {
        //            string enteredOtp = txt_verifyotp.Text.Trim();
        //            string sessionOtp = Session["verificationCode"]?.ToString() ?? "";

        //            if (enteredOtp == sessionOtp)
        //            {
        //                string userId = Session["userId"]?.ToString() ?? "0";


        //                var isPasswordReset = Session["isPasswordReset"]?.ToString();

        //                if (isPasswordReset == "0")  
        //                {

        //                    Response.Redirect("~/View/Authentication/changepassword.aspx?unid=" + userId, false);

        //                }
        //                else
        //                {
        //                    Response.Redirect("~/View/Modules/Home.aspx?unid=" + userId, false);
        //                }
        //            }
        //            else
        //            {
        //                lblErrorMessager.Text = "OTP is invalid.";
        //                divAlert.Visible = true;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("btn_send_Click", "verifyOTp", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //}
        protected void btn_verifyOtp_Click(object sender, EventArgs e)
        {
            string enteredOtp = txt_verifyotp.Text.Trim();
            string sessionOtp = Session["verificationCode"]?.ToString() ?? "";

            if (enteredOtp == sessionOtp)
            {
                string userId = Session["userId"]?.ToString() ?? "0";
                var isPasswordReset = Session["isPasswordReset"]?.ToString();

                if (isPasswordReset == "0")
                    Response.Redirect("~/View/Authentication/changepassword.aspx?unid=" + userId, false);
                else
                    Response.Redirect("~/View/Modules/Home.aspx?unid=" + userId, false);
            }
            else
            {
                lblErrorMessager.Text = "Invalid OTP. Please try again.";
                //lblErrorMessager.CssClass = "text-danger";
                divAlert.Visible = true;
            }
        }



    }
}