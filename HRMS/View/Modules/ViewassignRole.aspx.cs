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
    public partial class ViewassignRole : System.Web.UI.Page
    {
        protected TextBox txtComment;
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
                else
                {
                    Session["CurrentPageIndex"] = 0;
                    Session["SearchResults"] = null;
                    BindGridView();
                }

                BindRoles();
                BindUser();
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
                    ddl_role.DataSource = list1;
                    ddl_role.DataTextField = "Text";
                    ddl_role.DataValueField = "Id";
                }
                else
                {
                    ddl_role.DataSource = null;
                }
                ddl_role.DataBind();
                ddl_role.Items.Insert(0, new ListItem("--Select Role--", "0"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "BindRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                    ddl_username.DataSource = list1;
                    ddl_username.DataTextField = "Text";
                    ddl_username.DataValueField = "Id";
                }
                else
                {
                    ddl_username.DataSource = null;
                }
                ddl_username.DataBind();
                ddl_username.Items.Insert(0, new ListItem("--Select User--", "0"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "BindUser", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void AdvSearchButton_Click(object sender, EventArgs e)
        {
            AssignRoleDO assRole = new AssignRoleDO();
            try
            {
                if ((string.IsNullOrEmpty(ddl_username.SelectedValue) || ddl_username.SelectedValue == "0") && (string.IsNullOrEmpty(ddl_role.SelectedValue) || ddl_role.SelectedValue == "0"))
                {
                    string status = "Error";
                    string remark = "At least one field (Username or Role) is required.";

                    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                        $"showRoleSavedMessage('{status}', '{remark}');", true);

                    return;
                }

                assRole.usernameId = int.TryParse(ddl_username.SelectedValue, out int usernameId) ? (usernameId != 0 ? usernameId : (int?)null) : null;
                assRole.roleId = int.TryParse(ddl_role.SelectedValue, out int roleId) ? (roleId != 0 ? roleId : (int?)null) : null;
                AssignRoleBL assrolebl = new AssignRoleBL();

                List<AssignRoleDO> searchResults = assrolebl.AdvanceSearch(assRole);

                if (searchResults != null && searchResults.Count > 0)
                {
                    Session["SearchResults"] = searchResults;
                    gridview.PageIndex = 0;
                    BindGridView();
                    advancedSearchFields.Visible = false;
                    gridview.Visible = true;
                }
                else
                {
                    gridview.DataSource = null;
                    gridview.DataBind();
                    UpdatePageInfoLabel(0, 0);
                    advancedSearchFields.Visible = false;
                    gridview.Visible = true;
                    Session["SearchResults"] = null;
                }
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_assignrole.Visible = false;
                btnBack1.Visible = true;
                clearfileds();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "AdvSearchButton_Click", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }
        protected void AdvSearchFunction(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_assignrole.Visible = false;
                Session["CurrentPageIndex"] = 0;
                Session["SearchResults"] = null;
                advancedSearchFields.Visible = true;
                gridview.Visible = false;
                btn_advanceserach.Attributes["class"] = "btn btn-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "white";
                ddlPageSelector.Visible = false;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "AdvSearchFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void AdvBackButton_Click(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = true;
                btn_advanceserach.Visible = true;
                btn_assignrole.Visible = true;
                advancedSearchFields.Visible = false;
                gridview.Visible = true;
                BindGridView();
                btn_advanceserach.Attributes["class"] = "btn  btn-outline-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "black";
                Session["SearchResults"] = null;
                clearfileds();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "AdvBackButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void clearfileds()
        {
            ddl_username.SelectedIndex = 0;
            ddl_role.SelectedIndex = 0;

        }
        protected void AdvClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_assignrole.Visible = false;
                clearfileds();
                Session["SearchResults"] = null;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "AdvClearButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AddFunction(object sender, EventArgs e)
        {
            Response.Redirect("assignRole.aspx", false);

        }
        protected void addAssignClick(object sender, EventArgs e)
        {
            Response.Redirect("assignRole.aspx", false);
        }
        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "editassignedrole")
                {
                    string Userroledetailsid = e.CommandArgument.ToString();
                    Response.Redirect("assignRole.aspx?userroledetailsid=" + Userroledetailsid + "&mode=edit", false);
                }
                else if (e.CommandName == "deleteassignedrole")
                {

                    if (int.TryParse(e.CommandArgument.ToString(), out int Userroledetailsid))
                    {
                        Session["UserRoleDetailsId"] = Userroledetailsid;
                    }

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "gvUsers_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void BindGridView()
        {
            try
            {
                List<AssignRoleDO> assignroles;
                if (Session["SearchResults"] != null)
                {
                    assignroles = (List<AssignRoleDO>)Session["SearchResults"];
                }
                else
                {
                    AssignRoleBL assignrole = new AssignRoleBL();
                    assignroles = assignrole.ViewAllAssignRoles(); ;
                }


                ApplySorting(ref assignroles);
                int totalRecords = assignroles.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewAssign.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<AssignRoleDO> displayedLeaveRequests = assignroles.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gridview.DataSource = displayedLeaveRequests;
                    gridview.DataBind();

                    if (totalRecords > pageSize)
                    {
                        ddlPageSelector.Visible = true;
                        UpdatePageInfoLabel(pageIndex, totalRecords);
                    }
                    else
                    {
                        ddlPageSelector.Visible = false;
                    }
                }
                else
                {
                    gridview.DataSource = null;
                    gridview.DataBind();
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        public int TotalRecordCount()
        {
            try
            {
                AssignRoleBL assignrole = new AssignRoleBL();
                List<AssignRoleDO> assignroles = assignrole.ViewAllAssignRoles();

                List<AssignRoleDO> lstread = new List<AssignRoleDO>();
                lstread = assignrole.ViewAllAssignRoles();

                return lstread.Count;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "TotalRecordCount", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                return 0;
            }
        }
        protected void UpdatePageInfoLabel(int pageIndex, int pagecount)
        {
            try
            {
                int currentPage = pageIndex + 1;
                int totalPages = (int)Math.Ceiling((double)pagecount / 10);
                ddlPageSelector.Items.Clear();
                for (int i = 1; i <= totalPages; i++)
                {
                    ddlPageSelector.Items.Add(new System.Web.UI.WebControls.ListItem($"{i}/{totalPages}", (i - 1).ToString()));
                }
                if (ddlPageSelector.Items.Count > 0)
                {
                    ddlPageSelector.SelectedValue = pageIndex.ToString();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;
                BindGridView();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Session["CurrentPageIndex"] = e.NewPageIndex;
            BindGridView();
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            AssignRoleBL assignrole = new AssignRoleBL();
            List<AssignRoleDO> assignroles = assignrole.ViewAllAssignRoles();

            List<AssignRoleDO> lstread = new List<AssignRoleDO>();
            lstread = assignrole.ViewAllAssignRoles();
            if (lstread != null)
            {
                try
                {
                    string sortExpression = e.SortExpression;
                    string sortDirection = GetSortDirection(sortExpression);

                    if (sortDirection == "ASC")
                    {
                        lstread = lstread.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        lstread = lstread.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }

                    gridview.DataSource = lstread;
                    gridview.DataBind();
                }
                catch (Exception ex)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog("ViewassignRole", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                }
            }
        }
        private string GetSortDirection(string column)
        {

            string sortDirection = "ASC";
            if (ViewState["SortDirection"] != null)
            {
                if (ViewState["SortExpression"].ToString() == column)
                {
                    sortDirection = ViewState["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
                }
            }
            ViewState["SortExpression"] = column;
            ViewState["SortDirection"] = sortDirection;
            return sortDirection;
        }
        private void ApplySorting(ref List<AssignRoleDO> listdata)
        {
            try
            {
                string sortExpression = ViewState["SortExpression"] as string;
                string sortDirection = ViewState["SortDirection"] as string;

                if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "ASC")
                    {
                        listdata = listdata.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        listdata = listdata.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

            }
        }
        protected void ExportFunction(object sender, EventArgs e)
        {

        }
  
        protected void confirmDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserRoleDetailsId"] != null && int.TryParse(Session["UserRoleDetailsId"].ToString(), out int Userroledetailsid))
                {
                    AssignRoleBL assignrole = new AssignRoleBL();
                    AssignRoleDO role = assignrole.DeactivateAssignedRole(Userroledetailsid);

                    if (role.Status == "Success")
                    {
                        string status = role.Status;
                        string remark = role.Remarks;
                        BindGridView();
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showRoleSavedMessage('" + status + "', '" + remark + "');", true);
                    }
                    else
                    {
                        // Handle deletion failure
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showRoleSavedMessage('Failed', 'UserId not found in session');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewassignRole", "confirmDeleteButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnBack1_click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/ViewassignRole.aspx", false);
        }
    }
}