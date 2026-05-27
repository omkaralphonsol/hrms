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
    public partial class viewAssignRights : System.Web.UI.Page
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
                Session["CurrentPageIndex"] = 0;
                Session["SearchResults"] = null;
                BindGridView();
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
                    ddl_role.DataSource = list1;
                    ddl_role.DataTextField = "Text";
                    ddl_role.DataValueField = "Id";
                }
                else
                {
                    ddl_role.DataSource = null;
                }
                ddl_role.DataBind();
                ddl_role.Items.Insert(0, new ListItem("-- Please Select --", "0"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "BindRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                    ddl_menu.DataSource = list1;
                    ddl_menu.DataTextField = "Text";
                    ddl_menu.DataValueField = "Id";
                }
                else
                {
                    ddl_menu.DataSource = null;
                }
                ddl_menu.DataBind();
                ddl_menu.Items.Insert(0, new ListItem("-- Please Select --", ""));


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
                string menuId = ddl_menu.SelectedValue;
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
        protected void AdvSearchButton_Click(object sender, EventArgs e)
        {
            rightsDO assRghts = new rightsDO();
            try
            {
                if ((string.IsNullOrEmpty(ddl_role.SelectedValue) || ddl_role.SelectedValue == "0") &&
     (string.IsNullOrEmpty(ddl_menu.SelectedValue) || ddl_menu.SelectedValue == "0") &&
     (string.IsNullOrEmpty(ddl_submenu.SelectedValue) || ddl_submenu.SelectedValue == "0"))
                {
                    string status = "Error";
                    string remark = "At least one field is required.";

                    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                        $"showRightsSavedMessage('{status}', '{remark}');", true);

                    return;
                }
                if ((ddl_submenu.SelectedValue != "0" && ddl_submenu.SelectedValue != "") &&
                    (ddl_menu.SelectedValue == "0" || string.IsNullOrEmpty(ddl_menu.SelectedValue)))
                {
                    string status = "Error";
                    string remark = "Please select Menu if Submenu is selected.";

                    ClientScript.RegisterStartupScript(this.GetType(), "SubmenuDependsOnMenuScript",
                        $"showRightsSavedMessage('{status}', '{remark}');", true);
                    return;
                }

                assRghts.roleId = int.TryParse(ddl_role.SelectedValue, out int roleId) ? (roleId != 0 ? roleId : (int?)null) : null;
                assRghts.menuId = int.TryParse(ddl_menu.SelectedValue, out int menuId) ? (menuId != 0 ? menuId : (int?)null) : null;
                assRghts.submenuId = int.TryParse(ddl_submenu.SelectedValue, out int submenuId) ? (submenuId != 0 ? submenuId : (int?)null) : null;

                assignRightsBL assrghtsbl = new assignRightsBL();

                List<rightsDO> searchResults = assrghtsbl.AdvanceSearch(assRghts);

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
                btn_addassignrights.Visible = false;
                btnBack1.Visible = true;
                clearfileds();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "AdvSearchButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AdvSearchFunction(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_addassignrights.Visible = false;
                Session["SearchResults"] = null;
                Session["CurrentPageIndex"] = 0;
                advancedSearchFields.Visible = true;
                gridview.Visible = false;
                btn_advanceserach.Attributes["class"] = "btn btn-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "white";
                ddlPageSelector.Visible = false;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "AdvSearchFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AdvBackButton_Click(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = true;
                btn_advanceserach.Visible = true;
                btn_addassignrights.Visible = true;
                advancedSearchFields.Visible = false;
                Session["SearchResults"] = null;
                gridview.Visible = true;
                BindGridView();
                btn_advanceserach.Attributes["class"] = "btn  btn-outline-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "black";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "AdvBackButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void clearfileds()
        {
            ddl_role.SelectedIndex = 0;
            ddl_menu.SelectedIndex = 0;
            ddl_submenu.SelectedIndex = 0;
        }
        protected void AdvClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                clearfileds();
                ddlPageSelector.Visible = false;
                Session["SearchResults"] = null;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "AdvClearButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AddFunction(object sender, EventArgs e)
        {
            Response.Redirect("AssignRights.aspx", false);
        }
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "deleteRight")
                {
                    if (int.TryParse(e.CommandArgument.ToString(), out int userrightsid))
                    {
                        Session["UserRightsId"] = userrightsid;

                        // Open modal from code-behind after setting session
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openModal", "openDeleteModal();", true);
                    }
                    //if (int.TryParse(e.CommandArgument.ToString(), out int userrightsid))
                    //{
                    //    Session["UserRightsId"] = userrightsid;
                    //}

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "gvUsers_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void BindGridView()
        {
            try
            {
                List<rightsDO> rights = new List<rightsDO>();

                if (Session["SearchResults"] != null)
                {
                    rights = (List<rightsDO>)Session["SearchResults"];
                }
                else
                {
                    assignRightsBL assignRightsBL = new assignRightsBL();
                    rights = assignRightsBL.ViewAllAssignRights();
                }

                ApplySorting(ref rights);
                int totalRecords = rights.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewRights.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<rightsDO> displayedLeaveRequests = rights.GetRange(startRowIndex, endRowIndex - startRowIndex);
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
                errorlog.fnStoreErrorLog("viewAssignRights", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        public int TotalRecordCount()
        {
            try
            {
                assignRightsBL assignRightsBL = new assignRightsBL();
                List<rightsDO> rights = assignRightsBL.ViewAllAssignRights();


                List<rightsDO> lstread = new List<rightsDO>();
                lstread = assignRightsBL.ViewAllAssignRights();

                return lstread.Count;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "TotalRecordCount", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewAssignRights", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewAssignRights", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                    errorlog.fnStoreErrorLog("viewAssignRights", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<rightsDO> listdata)
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
                errorlog.fnStoreErrorLog("viewAssignRights", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }


        protected void confirmDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserRightsId"] != null && int.TryParse(Session["UserRightsId"].ToString(), out int userrightsid))
                {
                    assignRightsBL assignRightsBl = new assignRightsBL();
                    rightsDO rights = assignRightsBl.DeactivateRights(userrightsid);

                    if (rights.Status == "Success")
                    {
                        string status = rights.Status;
                        string remark = rights.Remarks;
                        BindGridView();
                        ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript", "showRightsSavedMessage('" + status + "', '" + remark + "');", true);
                    }
                    else
                    {
                        // Handle deletion failure
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript", "showRightsSavedMessage('Failed', 'UserId not found in session');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "confirmDeleteButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnBack1_click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/viewAssignRights.aspx", false);
        }
    }
}