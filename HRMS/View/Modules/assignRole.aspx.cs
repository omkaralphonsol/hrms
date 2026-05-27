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
    public partial class assignRole : System.Web.UI.Page
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

                if (Request.QueryString["userroledetailsid"] != null)
                {
                    try
                    {
                        int Userroledetailsid = Convert.ToInt32(Request.QueryString["userroledetailsid"]);
                        AssignRoleBL assignrole = new AssignRoleBL();
                        List<AssignRoleDO> assignrolelist = assignrole.GetAssignedRole(Userroledetailsid);

                        if (assignrolelist.Count > 0)
                        {
                            AssignRoleDO assigndrole = assignrolelist[0];
                            ddluser.SelectedValue = assigndrole.Userloginid.ToString();
                            ddlrole.SelectedValue = assigndrole.Roleid.ToString();

                            if (Request.QueryString["mode"] == "edit")
                            {
                                ddlrole.Enabled = true;
                                ddluser.Enabled = true;
                                btn_submit.Text = "Update";
                                btn_submit.Visible = true;
                                btn_clear.Text = "Clear";
                                btn_clear.Visible = true;
                                // btnReset.Visible = true;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog("assignRole", "Page_Load", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                    }
                }
                BindRoles();
                BindUser();
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
        public void BindRoles()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownroles();
                if (list1 != null)
                {
                    ddlrole.DataSource = list1;
                    ddlrole.DataTextField = "Text";
                    ddlrole.DataValueField = "Id";
                }
                else
                {
                    ddlrole.DataSource = null;
                }
                ddlrole.DataBind();
                ddlrole.Items.Insert(0, new ListItem("--Select Role--", "0"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "BindRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        public void BindUser()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownusers();
                if (list1 != null)
                {
                    ddluser.DataSource = list1;
                    ddluser.DataTextField = "Text";
                    ddluser.DataValueField = "Id";
                }
                else
                {
                    ddluser.DataSource = null;
                }
                ddluser.DataBind();
                ddluser.Items.Insert(0, new ListItem("--Select User--", "0"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "BindUser", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void save_assignroleclick(object sender, EventArgs e)
        {
            AssignRoleDO assignrole = new AssignRoleDO();
            ResponseDO response = new ResponseDO();
            try
            {
                int userId = GetClientIdFromSession();
                assignrole.Insertedby = userId;
                if (btn_submit.Text == "Submit")
                {
                    AssignRoleBL assignroledetails = new AssignRoleBL();

                    assignrole.roledescription = ddlrole.SelectedValue;
                    assignrole.UserName = ddluser.SelectedValue;

                    List<AssignRoleDO> listdata = assignroledetails.SaveAssignRoleDetails(assignrole);
                    if (listdata[0].Status == "Success")
                    {
                        string status = listdata[0].Status;
                        string remark = listdata[0].Remarks;
                        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "AssignroleUpdatedScript",
                        //"showAssignedRoleMessage('" + status + "', '" + remark + "');" +
                        //"setTimeout(function(){ window.location.href = 'ViewassignRole.aspx'; }, 5000);", true);
                        ClientScript.RegisterStartupScript(this.GetType(), "AssignroleUpdatedScript",
                            "showAssignedRoleMessage('" + status + "', '" + remark + "');" +
                            "setTimeout(function(){ window.location.href = 'ViewassignRole.aspx'; }, 5000);", true);
                        return;
                        //Response.Redirect("/View/Modules/ViewassignRole.aspx", false);

                    }
                    else
                    {
                        string status = listdata[0].Status;
                        string remark = listdata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "AssignroleUpdatedScript",
                            "showAssignedRoleMessage('" + status + "', '" + remark + "');" +
                            "setTimeout(function(){ window.location.href = 'ViewassignRole.aspx'; }, 5000);", true);
                        return;
                    }
                }
                else if (btn_submit.Text == "Update")
                {
                    assignrole.Userroledetailsid = Convert.ToInt32(Request.QueryString["userroledetailsid"]);
                    AssignRoleBL assignedrole = new AssignRoleBL();
                    assignrole.roledescription = ddlrole.SelectedValue;
                    assignrole.UserName = ddluser.SelectedValue;
                    List<AssignRoleDO> listdata = assignedrole.UpdateRoleDetails(assignrole);
                    if (listdata[0].Status == "Success")
                    {
                        string status = listdata[0].Status;
                        string remark = listdata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "AssignroleUpdatedScript",
                            "showAssignedRoleMessage('" + status + "', '" + remark + "');" +
                            "setTimeout(function(){ window.location.href = 'ViewassignRole.aspx'; }, 5000);", true);
                        return;
                    }
                    else
                    {
                        string status = listdata[0].Status;
                        string remark = listdata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "AssignroleUpdatedScript",
                            "showAssignedRoleMessage('" + status + "', '" + remark + "');" +
                            "setTimeout(function(){ window.location.href = 'ViewassignRole.aspx'; }, 5000);", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "save_assignroleclick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                ddlrole.SelectedIndex = 0;
                ddluser.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("assignRole", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/ViewassignRole.aspx", false);
            return;
        }
    }
}