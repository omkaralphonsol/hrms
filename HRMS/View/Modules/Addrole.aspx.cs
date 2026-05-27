using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class Addrole : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                if (Request.QueryString["roleid"] != null)
                {
                    try
                    {

                        int Roleid = Convert.ToInt32(Request.QueryString["roleid"]);
                        RoleDetailsBL roleBal = new RoleDetailsBL();
                        List<roleDO> roledetaillist = roleBal.GetRoleDetails(Roleid);

                        if (roledetaillist.Count > 0)
                        {
                            roleDO roledetails = roledetaillist[0];

                            txt_role.Text = roledetails.Roledescription;

                            if (Request.QueryString["mode"] == "edit")
                            {

                                txt_role.Enabled = true;

                                RoleTitleLabel.Text = "Update Role";
                                btn_submit.Text = "Update";
                                btn_submit.Visible = true;
                                btn_reset.Text = "Clear";
                                btn_reset.Visible = true;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog("AddRole", "pageload", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                    }
                }

            }
        }
        private int GetClientIdFromSession()
        {
            int userId = 0;
            if (Session["UserId"] != null)
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            return userId;
        }
        protected void save_roleclick(object sender, EventArgs e)
        {
            roleDO role = new roleDO();
            ResponseDO response = new ResponseDO();
            try
            {
                int userId = GetClientIdFromSession();
                role.Insertedby = userId;
                if (btn_submit.Text == "Submit")
                {
                    RoleDetailsBL roledetails = new RoleDetailsBL();
                    role.Roledescription = txt_role.Text;
                    List<roleDO> roledata = roledetails.SaveRoleDetails(role);
                    if (roledata[0].Status == "Success")
                    {
                        string status = roledata[0].Status;
                        string remark = roledata[0].Remarks;

                        ClientScript.RegisterStartupScript(this.GetType(), "TaskSavedScript",
                             "showRoleSavedMessage('" + status + "', '" + remark + "');" +
                             "setTimeout(function(){ window.location.href = 'ViewRole.aspx'; }, 5000);", true);
                        return;
                    }
                    else
                    {
                        string status = roledata[0].Status;
                        string remark = roledata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "TaskSavedScript",
                              "showRoleSavedMessage('" + status + "', '" + remark + "');" +
                              "setTimeout(function(){ window.location.href = 'ViewRole.aspx'; }, 5000);", true);
                        return;
                    }

                }
                else if (btn_submit.Text == "Update")
                {
                    role.Roleid = Convert.ToInt32(Request.QueryString["roleid"]);
                    RoleDetailsBL roledetails = new RoleDetailsBL();
                    role.Roledescription = txt_role.Text;

                    List<roleDO> roledata = roledetails.UpdateRoleDetails(role);
                    if (roledata[0].Status == "Success")
                    {
                        string status = roledata[0].Status;
                        string remark = roledata[0].Remarks;

                        ClientScript.RegisterStartupScript(this.GetType(), "TaskSavedScript",
                              "showRoleSavedMessage('" + status + "', '" + remark + "');" +
                              "setTimeout(function(){ window.location.href = 'ViewRole.aspx'; }, 5000);", true);
                        return;
                    }
                    else
                    {
                        string status = roledata[0].Status;
                        string remark = roledata[0].Remarks;

                        ClientScript.RegisterStartupScript(this.GetType(), "TaskSavedScript",
                              "showRoleSavedMessage('" + status + "', '" + remark + "');" +
                              "setTimeout(function(){ window.location.href = 'ViewRole.aspx'; }, 5000);", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddRole", "save_roleclick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void viewRoleClick(object sender, EventArgs e)
        {
            Response.Redirect("ViewRole.aspx", false);
        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                txt_role.Text = "";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddRole", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
    }
}