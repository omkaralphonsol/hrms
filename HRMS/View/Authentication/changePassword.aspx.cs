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
    public partial class changePassword : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["userid"])))
            {
                int userId = Convert.ToInt32(Session["userid"]);
                if (!IsPostBack)
                {
                    GetUserName();
                }
            }
            else
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
            }
        }
        protected void btnchange_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtusername.Text;
                string oldpassword = txtoldpass.Text;
                string newpassword = txtnewpass.Text;
                string confirmPassword = txtconfirmpass.Text;

                if (!string.IsNullOrEmpty(oldpassword) && !string.IsNullOrEmpty(newpassword) && !string.IsNullOrEmpty(confirmPassword))
                {
                    if (newpassword == confirmPassword)
                    {
                        int userId = GetUserIdByUsername(username);

                        if (userId > 0)
                        {
                            LoginBO login = new LoginBO();
                            LoginBAL loginBAL = new LoginBAL();
                            login.Password = newpassword;
                            login.UserId = userId;
                            login.oldpassword = oldpassword;
                            login.Username = username;

                            List<LoginBO> logindata = loginBAL.changePassBal(login);

                            if (logindata[0].result == "Success")
                            {
                                string status = logindata[0].result;
                                string remark = logindata[0].message;

                                ClientScript.RegisterStartupScript(this.GetType(), "changepassSavedScript",
                                    "showChangePassMessage('" + status + "', '" + remark + "');" +
                                    "setTimeout(function(){ window.location.href = 'login.aspx'; }, 5000);", true);

                                return;
                            }
                            else
                            {
                                string status = logindata[0].result;
                                string remark = logindata[0].message;

                                ClientScript.RegisterStartupScript(this.GetType(), "changepassSavedScript",
                                    "showChangePassMessage('" + status + "', '" + remark + "');" +
                                    "setTimeout(function(){ window.location.href = 'changePassword.aspx'; }, 5000);", true);

                                return;
                            }
                        }
                        else
                        {
                            divAlert.Visible = true;
                            lblErrorMessager.Text = "Invalid username.";
                        }
                    }
                    else
                    {
                        divAlert.Visible = true;
                        lblErrorMessager.Text = "Passwords do not match.";
                    }
                }
                else
                {
                    // Handle the case where any of the password fields is empty
                    divAlert.Visible = true;
                    lblErrorMessager.Text = "Please fill in all password fields.";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("changePassword", "btnchange_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void GetUserName()
        {
            try
            {
                userBAL userBAL = new userBAL();
                MenuDO menuDO = new MenuDO();
                menuDO.userId = Convert.ToString(Session["userId"]);
                List<MenuDO> menuDataList = userBAL.GetUser(menuDO);
                if (menuDataList.Count > 0)
                {
                    MenuDO userDetails = menuDataList[0];
                    txtusername.Text = userDetails.username;
                    txtusername.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("changePassword", "btnchange_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private int GetUserIdByUsername(string username)
        {
            try
            {
                LoginBO login = new LoginBO();
                LoginBAL loginBAL = new LoginBAL();
                login.Username = username;
                List<LoginBO> listdata = loginBAL.GetUserIdByUsername(login);
                return listdata[0].UserId;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("changePassword", "GetUserIdByUsername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return 0;
        }

        protected void cancelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/view/modules/Home.aspx", false);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("changePassword", "btnchange_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}