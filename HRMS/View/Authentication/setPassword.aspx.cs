using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Authentication
{
    public partial class setPassword : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int UserId = Convert.ToInt32(Session["userid"]);
            }
        }

        protected void btnchange_Click(object sender, EventArgs e)
        {
            try
            {
                string password = Password.Text;
                string confirmPassword = newPassword.Text;
                if (!Regex.IsMatch(password, @"^[A-Z].{7,}$") ||
          !Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                {
                    divAlert.Visible = true;
                    lblErrorMessager.Text = "Password must start with a capital letter, be at least 8 characters long, and contain at least one special character.";
                    return;
                }
                if (password != confirmPassword)
                {
                    divAlert.Visible = true;
                    lblErrorMessager.Text = "Password and Confirm Password do not match.";
                    //ClientScript.RegisterStartupScript(this.GetType(), "passwordMismatchScript",
                    //    "showPassSavedMessage('Error', 'Password and Confirm Password do not match.');", true);
                    return;
                }
                if (password == confirmPassword)
                {
                    if (Session["userId"] != null)
                    {
                        int userId = Convert.ToInt32(Session["userId"]);
                        LoginBO login = new LoginBO();
                        LoginBAL loginBAL = new LoginBAL();
                        login.Password = Password.Text;
                        login.UserId = userId;

                        List<LoginBO> listdata = loginBAL.forgetPassBal(login);

                        if (listdata[0].result == "1")
                        {
                            string status = listdata[0].status;
                            string remark = listdata[0].message;

                            ClientScript.RegisterStartupScript(this.GetType(), "changepassSavedScript",
                                "showPassSavedMessage('" + status + "', '" + remark + "');" +
                                "setTimeout(function(){ window.location.href = 'login.aspx'; }, 5000);", true);

                            return;
                            //lblErrorMessager.Text = "Password Change Successfully";
                        }
                        else
                        {
                            string status = listdata[0].status;
                            string remark = listdata[0].message;

                            ClientScript.RegisterStartupScript(this.GetType(), "changepassSavedScript",
                                "showPassSavedMessage('" + status + "', '" + remark + "');" +
                                "setTimeout(function(){ window.location.href = 'login.aspx'; }, 5000);", true);

                            return;
                        }
                    }
                    else
                    {
                        Response.Redirect("~/login.aspx");
                    }
                }
                else
                {
                    divAlert.Visible = true;
                    lblErrorMessager.Text = "Passwords do not match.";
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("setPassword", "btnchange_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
    }
}