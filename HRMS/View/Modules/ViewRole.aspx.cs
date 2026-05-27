using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ViewRole : System.Web.UI.Page
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

                BindGridView();
                BindSearchbyRole();
            }
        }
        private string GetNullableString(string value)
        {

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
  
        protected void AdvSearchButton_Click(object sender, EventArgs e)
        {
            roleDO role = new roleDO();
            try
            {

                //if (string.IsNullOrWhiteSpace(ddl_role.Text))
                //{
                //    string status = "Error";
                //    string remark = "Role is required.";

                //    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                //        $"showRoleSavedMessage('{status}', '{remark}');", true);

                //    return;
                //}


                //role.Roledescription = GetNullableString(ddl_role.Text);
                if (string.IsNullOrWhiteSpace(ddl_role.Text))
                {
                    string status = "Error";
                    string remark = "Role is required.";

                    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                        $"showRoleSavedMessage('{status}', '{remark}');", true);
                    return;
                }

                role.Roledescription = ddl_role.Text.Trim();
                role.searchValue = ddl_role.SelectedValue;



                RoleDetailsBL rolebl = new RoleDetailsBL();

                List<roleDO> roleDo = rolebl.AdvanceSearch(role);



                roleDo = roleDo.OrderByDescending(t => t.Roleid).ToList();

                int totalRecords = roleDo.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hdnClickedRoleId.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    Session["AdvSearchResViewUser"] = roleDo;
                    List<roleDO> displayedUsers = roleDo.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gridview.DataSource = displayedUsers;
                    gridview.DataBind();

                    advancedSearchFields.Visible = false;
                    gridview.Visible = true;

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

                    Session["SearchResults"] = null;
                    advancedSearchFields.Visible = false;
                    gridview.Visible = true;
                }
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_addRole.Visible = false;
                btnBack1.Visible = true;
                clearfileds();
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewRole", "AdvSearchButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void clearfileds()
        {
            ddl_role.Text = string.Empty;
           
        }
        protected void AdvClearButton_Click(object sender, EventArgs e)
        {
            ddl_role.Text = string.Empty;
            BindGridView();

        }
        protected void AdvSearchFunction(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_addRole.Visible = false;
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
                errorlog.fnStoreErrorLog("Viewuser", "AdvSearchFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AdvBackButton_Click(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = true;
                btn_advanceserach.Visible = true;
                btn_addRole.Visible = true;
                advancedSearchFields.Visible = false;
                gridview.Visible = true;
                Session["AdvSearchResViewUser"] = null;
                BindGridView();
                btn_advanceserach.Attributes["class"] = "btn  btn-outline-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "black";
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "AdvBackButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void Button3_ServerClick(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/AddRole.aspx", false);
        }
       
        protected void ExportFunction(object sender, EventArgs e)
        {

        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            ddlsearchp.SelectedIndex = 0;
            txtSearch.Text = string.Empty;

        }
        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "viewRole")
                {
                    string userId = e.CommandArgument.ToString();
                    Response.Redirect("AddRole.aspx?roleid=" + userId + "&mode=edit", false);
                }

                else if (e.CommandName == "deleteRole")
                {
                    if (int.TryParse(e.CommandArgument.ToString(), out int roleid))
                    {
                        Session["Roleid"] = roleid;

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openModal", "openDeleteModal();", true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewRole", "gv_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void BindSearchbyRole()
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

        protected void BindGridView()
        {
            try
            {
                RoleDetailsBL roldedetail = new RoleDetailsBL();
                List<roleDO> roles = roldedetail.ViewAllRoles();
                ApplySorting(ref roles);
                int totalRecords = roles.Count;

                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hdnClickedRoleId.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<roleDO> displayeddata = roles.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gridview.DataSource = displayeddata;
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
                errorlog.fnStoreErrorLog("ViewRole", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("ViewRole", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        public int TotalRecordCount()
        {
            try
            {
                roleDO roleDo = new roleDO();
                RoleDetailsBL roldedetail = new RoleDetailsBL();
                List<roleDO> listdata = roldedetail.ViewAllRoles();

                return listdata.Count;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewRole", "TotalRecordCount", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                return 0;
            }
        }
  
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPage = Convert.ToInt32(ddlPageSelector.SelectedValue);
                gridview.PageIndex = selectedPage - 1;
                BindGridView();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewRole", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            BindGridView();
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            RoleDetailsBL roldedetail = new RoleDetailsBL();
            List<roleDO> createdet = roldedetail.ViewAllRoles();

            if (createdet != null)
            {
                try
                {
                    string sortExpression = e.SortExpression;
                    string sortDirection = GetSortDirection(sortExpression);

                    if (sortDirection == "ASC")
                    {
                        createdet = createdet.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        createdet = createdet.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }

                    gridview.DataSource = createdet;
                    gridview.DataBind();
                }
                catch (Exception ex)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog("viewRole", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

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
        private void ApplySorting(ref List<roleDO> patients)
        {
            try
            {
                string sortExpression = ViewState["SortExpression"] as string;
                string sortDirection = ViewState["SortDirection"] as string;

                if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "ASC")
                    {
                        patients = patients.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        patients = patients.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewRole", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void confirmDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["Roleid"] != null && int.TryParse(Session["Roleid"].ToString(), out int Roleid))
                {
                    RoleDetailsBL rolebl = new RoleDetailsBL();


                    roleDO role = rolebl.DeactivateRole(Roleid);

                    if (role.Status == "Success")
                    {
                        string status = role.Status;
                        string remark = role.Remarks;
                        BindGridView();
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showUserSavedMessage('" + status + "', '" + remark + "');", true);
                    }
                    else
                    {
                        // Handle deletion failure
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showUserSavedMessage('Failed', 'UserId not found in session');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewRole", "confirmDeleteButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnBack1_click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/ViewRole.aspx", false);
        }
    }
}