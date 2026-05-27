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
    public partial class AssignRights : System.Web.UI.Page
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
                BindRoles();
                BindMenu("Bindmenu","");
            }
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
                errorlog.fnStoreErrorLog("AssignRights", "BindRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        public void BindMenu(string type, string menuId)
        {
            try
            {
                List<DropDownData> list1 = new List<DropDownData>();
                CommonBL commonbl = new CommonBL();
                list1 = commonbl.dropdownMenu(type, menuId);
                if (list1 != null)
                {
                    ddlMenu.DataSource = list1;
                    ddlMenu.DataTextField = "Text";
                    ddlMenu.DataValueField = "Id";
                }
                else
                {
                    ddlMenu.DataSource = null;
                }
                ddlMenu.DataBind();
                ddlMenu.Items.Insert(0, new ListItem("-- Please Select --", ""));


            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "BindMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void ddlMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string menuId = ddlMenu.SelectedValue;
                BindSubMenus("Bindsubmenu", menuId);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "ddlMenu_SelectedIndexChanged", "Exception Message=" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void BindSubMenus(string type, string menuId)
        {
            try
            {
                ddl_submenu.Items.Clear();

                CommonBL commonbl = new CommonBL();
                List<DropDownData> list1 = commonbl.dropdownSubMenu(type, menuId);

                if (list1 != null && list1.Count > 0)
                {
                    ddl_submenu.DataSource = list1;
                    ddl_submenu.DataTextField = "Text";
                    ddl_submenu.DataValueField = "Id";
                    ddl_submenu.DataBind();

                    // Insert default option
                    ddl_submenu.Items.Insert(0, new ListItem("-- Please Select --", ""));
                    lbl_submenu.Visible = true;
                }
                else
                {
                    ddl_submenu.Items.Insert(0, new ListItem("-- No Submenu Found --", ""));
                    lbl_submenu.Visible = false;
                }

                ddl_submenu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "BindSubMenus", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void ddlSubMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string submenuId = ddl_submenu.SelectedValue;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "ddlSubMenu_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/viewAssignRights.aspx", false);
            return;
        }
        protected void btn_reset_Click(object sender, EventArgs e)
        {
            try
            {
                ddlrole.SelectedIndex = 0;
                ddlMenu.SelectedIndex = 0;
                ddl_submenu.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "btn_reset_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            rightsDO rightsDO = new rightsDO();
            ResponseDO response = new ResponseDO();
            try
            {
                int userId = GetClientIdFromSession();
                rightsDO.Insertedby = userId;
                assignRightsBL assignrights = new assignRightsBL();
                rightsDO.roleid = Convert.ToInt32(ddlrole.SelectedValue);
                rightsDO.menuid = Convert.ToInt32(ddlMenu.SelectedValue);
                //rightsDO.submenuid = Convert.ToInt32(ddl_submenu.SelectedValue);
                // rightsDO.submenuid = string.IsNullOrEmpty(ddl_submenu.SelectedValue) ? null : (int?)Convert.ToInt32(ddl_submenu.SelectedValue);
                if (!string.IsNullOrEmpty(ddl_submenu.SelectedValue))
                {
                    rightsDO.submenuid = Convert.ToInt32(ddl_submenu.SelectedValue);
                }
                else
                {
                    rightsDO.submenuid = 0;
                }
                List<rightsDO> rights = assignrights.SaveRights(rightsDO);
                if (rights[0].Status == "Success")
                {
                    string status = rights[0].Status;
                    string remark = rights[0].Remarks;

                    ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript",
                                   "showRightSavedMessage('" + status + "', '" + remark + "');" +
                                   "setTimeout(function(){ window.location.href = 'ViewAssignRights.aspx'; }, 5000);", true);
                }
                else
                {
                    string status = rights[0].Status;
                    string remark = rights[0].Remarks;
                    ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript",
                                    "showRightSavedMessage('" + status + "', '" + remark + "');" +
                                    "setTimeout(function(){ window.location.href = 'ViewAssignRights.aspx'; }, 5000);", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "btn_submit_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}