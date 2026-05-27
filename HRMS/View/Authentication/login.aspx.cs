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
    public partial class login : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetNoStore();

            if (Session["userId"] != null)
            {
                UserId = Convert.ToString(Session["userId"]);
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }
        }
        protected void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Reset UI messages
                lblUserNameError.Visible = false;
                lblPasswordError.Visible = false;
                lblCaptchaError.Visible = false;
                lblErrorMessager.Visible = false;
                divAlert.Visible = false;

                bool hasError = false;

                // ================= VALIDATION START =================

                if (string.IsNullOrWhiteSpace(usernametxt.Text))
                {
                    lblUserNameError.Text = "Username is required.";
                    lblUserNameError.Visible = true;
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(passwordtxt.Text))
                {
                    lblPasswordError.Text = "Password is required.";
                    lblPasswordError.Visible = true;
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(txtVerificationCode.Text))
                {
                    lblCaptchaError.Text = "Captcha is required.";
                    lblCaptchaError.Visible = true;
                    hasError = true;
                }

                if (hasError)
                    return;

                // ================= CAPTCHA CHECK =================

                if (txtVerificationCode.Text != Convert.ToString(Session["CaptchaVerify"]))
                {
                    lblCaptchaError.Text = "Incorrect captcha entered.";
                    lblCaptchaError.Visible = true;
                    txtVerificationCode.Text = "";
                    return;
                }

                // ================= LOGIN PROCESS =================

                string username = usernametxt.Text.Trim();
                Session["userName"] = username;

                LoginDO login = new LoginDO();
                LoginBAL loginBAL = new LoginBAL();

                login.Username = username;
                login.Password = passwordtxt.Text;

                login = loginBAL.UserLogin(login);

                if (login != null)
                {
                    if (login.result == "Login Successfully")
                    {
                        divAlert.Visible = true;
                        lblErrorMessager.ForeColor = System.Drawing.Color.Green;
                        lblErrorMessager.Text = "Login successful";

                        string un = Convert.ToString(login.userId);

                        Session["userId"] = Convert.ToInt32(un);
                        Session["userrole"] = login.UserRole;
                        int sessionCompanyId = loginBAL.GetCompanyIdByUserId(Convert.ToInt32(un));
                        Session["company_id"] = sessionCompanyId;

                        Response.Redirect("/View/Authentication/verifyOTP.aspx?unid=" + un, false);
                    }
                    else
                    {
                        ClearSession();
                        lblErrorMessager.Text = login.remark;
                        divAlert.Visible = true;
                    }
                }
                else
                {
                    ClearSession();
                    lblErrorMessager.Text = "Invalid username or password.";
                    divAlert.Visible = true;
                }
            }
            catch (Exception ex)
            {
                var currentUserId = Convert.ToString(Session["userId"]);

                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "login",
                    "btnlogin_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    currentUserId
                );

                ClearSession();
            }
        }
        //protected void loginButton_Click(object sender, EventArgs e)
        //{

        //    lblUserNameError.Visible = false;
        //    lblPasswordError.Visible = false;
        //    lblCaptchaError.Visible = false;
        //    divAlert.Visible = false;

        //    try
        //    {
        //        bool hasError = false;

        //        if (string.IsNullOrWhiteSpace(usernametxt.Text))
        //        {
        //            lblUserNameError.Text = "Username is required.";
        //            lblUserNameError.Visible = true;
        //            hasError = true;
        //        }

        //        if (string.IsNullOrWhiteSpace(passwordtxt.Text))
        //        {
        //            lblPasswordError.Text = "Password is required.";
        //            lblPasswordError.Visible = true;
        //            hasError = true;
        //        }

        //        if (string.IsNullOrWhiteSpace(txtVerificationCode.Text))
        //        {
        //            lblCaptchaError.Text = "Captcha is required.";
        //            lblCaptchaError.Visible = true;
        //            hasError = true;
        //        }

        //        if (hasError)
        //            return;

        //        if (txtVerificationCode.Text != Convert.ToString(Session["CaptchaVerify"]))
        //        {
        //            lblCaptchaError.Text = "Invalid captcha entered.";
        //            lblCaptchaError.Visible = true;
        //            txtVerificationCode.Text = "";
        //            return;
        //        }

        //        string username = usernametxt.Text.Trim();
        //        string password = passwordtxt.Text;

        //        LoginBO login = new LoginBO { Username = username, Password = password };
        //        LoginBAL loginBAL = new LoginBAL();
        //        List<LoginBO> logindata = loginBAL.UserLogin(login);

        //        if (logindata != null && logindata.Count > 0 && logindata[0].result == "Login Successfully")
        //        {
        //            string un = logindata[0].userId;
        //            Session["userid"] = Convert.ToInt32(un);
        //            Session["roleid"] = Convert.ToInt32(logindata[0].role_id);

        //            //try
        //            //{
        //            //    SaveLogInTime(Convert.ToInt32(un));
        //            //}
        //            //catch (Exception exx)
        //            //{
        //            //    new CommonBL().fnStoreErrorLog("login", "SaveLogInTime", exx.Message, UserId);
        //            //}

        //            ScriptManager.RegisterStartupScript(this, GetType(), "SetUsernameInSessionStorage", $"sessionStorage.setItem('username', '{username}');", true);

        //            if (logindata[0].isPasswordReset == 0)
        //                Response.Redirect("~/view/authentication/verifyOTP.aspx", true);
        //            else
        //            {
        //                int roleId = Convert.ToInt32(Session["roleid"]);
        //                if (roleId == 2)
        //                    Response.Redirect("/View/Modules/LoginLogout.aspx?unid=" + un, false);
        //                else
        //                    Response.Redirect("/View/Modules/EmployeeLoginDetails.aspx?unid=" + un, false);
        //            }
        //        }
        //        else
        //        {
        //            if (logindata[0].result == "Invalid UserName or Password")
        //            {
        //                lblUserNameError.Text = logindata[0].remark;
        //                lblUserNameError.Visible = true;
        //                hasError = true;
        //            }
        //            else if (logindata[0].result == "Login Failed")
        //            {
        //                lblPasswordError.Text = logindata[0].remark;
        //                lblPasswordError.Visible = true;
        //                hasError = true;
        //            }
        //            //lblErrorMessager.Text = logindata != null ? logindata[0].remark : "Username or Password is incorrect.";
        //            //divAlert.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new CommonBL().fnStoreErrorLog("login", "loginButton_Click", ex.Message + " StackTrace: " + ex.StackTrace, UserId);
        //    }
        //}

        private void ClearSession()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cookies.Clear();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
        }
    }
}
