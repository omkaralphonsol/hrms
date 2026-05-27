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
    public partial class Viewuser : System.Web.UI.Page
    {
        protected string UserId = null;
        public static List<UserDetailsDO> userDo = new List<UserDetailsDO>();
        private bool IsEmployeeFlow
        {
            get
            {
                return string.Equals(
                    Convert.ToString(Request.QueryString["flow"] ?? string.Empty).Trim(),
                    "employee",
                    StringComparison.OrdinalIgnoreCase
                );
            }
        }

        private List<UserDetailsDO> GetUsersForCurrentFlow()
        {
            UserDetailsBL userBL = new UserDetailsBL();
            return IsEmployeeFlow ? userBL.ViewAllUsers() : userBL.ViewAllUsersMainDb();
        }
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
                Session["AdvSearchResViewUser"] = null;
                ApplyFlowLabels();

                BindGridView();
                BindUsername();
                BindEmpCode();

            }
        }

        private void ApplyFlowLabels()
        {
            if (IsEmployeeFlow)
            {
                btn_adduser.Attributes["title"] = "Add Employee";
                btn_adduser.InnerHtml = "<i class=\"fas fa-user-plus\"></i>&nbsp;Add Employee";
            }
            else
            {
                btn_adduser.Attributes["title"] = "Add User";
                btn_adduser.InnerHtml = "<i class=\"fas fa-user-plus\"></i>&nbsp;Add User";
            }
        }
        public void BindUsername()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> users = GetUsersForCurrentFlow();

                ddl_username.Items.Clear();
                if (users != null && users.Count > 0)
                {
                    var distinctUsers = users
                        .Where(u => u.UserId > 0 && !string.IsNullOrWhiteSpace(u.user_fullname))
                        .GroupBy(u => u.UserId)
                        .Select(g => g.First())
                        .OrderBy(u => u.user_fullname)
                        .ToList();

                    ddl_username.DataSource = distinctUsers.Select(u => new
                    {
                        Text = u.user_fullname,
                        Id = u.UserId
                    });
                    ddl_username.DataTextField = "Text";
                    ddl_username.DataValueField = "Id";
                    ddl_username.DataBind();
                }
                ddl_username.Items.Insert(0, new ListItem("-- Please Select --", ""));


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "BindUsername", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        public void BindEmpCode()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> users = GetUsersForCurrentFlow();

                ddl_employeeCode.Items.Clear();
                if (users != null && users.Count > 0)
                {
                    var distinctEmpCodes = users
                        .Where(u => !string.IsNullOrWhiteSpace(u.EmployeeCode))
                        .GroupBy(u => u.EmployeeCode.Trim())
                        .Select(g => g.First())
                        .OrderBy(u => u.EmployeeCode)
                        .ToList();

                    ddl_employeeCode.DataSource = distinctEmpCodes.Select(u => new
                    {
                        Text = u.EmployeeCode,
                        Id = u.UserId
                    });
                    ddl_employeeCode.DataTextField = "Text";
                    ddl_employeeCode.DataValueField = "Id";
                    ddl_employeeCode.DataBind();
                }
                ddl_employeeCode.Items.Insert(0, new ListItem("-- Please Select --", ""));


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "BindStatus", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void AdvSearchButton_Click(object sender, EventArgs e)
        {
            UserDetailsDO user = new UserDetailsDO();
            try
            {

                if ((string.IsNullOrEmpty(ddl_employeeCode.SelectedValue) || ddl_employeeCode.SelectedValue == "0") &&
                    (string.IsNullOrWhiteSpace(txt_contact.Text)) &&
                    (string.IsNullOrEmpty(ddl_username.SelectedValue) || ddl_username.SelectedValue == "0"))
                {
                    string status = "Error";
                    string remark = "At least one field is required.";

                    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                        $"showUserSavedMessage('{status}', '{remark}');", true);

                    return;
                }

                user.contact_detail = GetNullableString(txt_contact.Text);

                user.usernameId = int.TryParse(ddl_username.SelectedValue, out int usernameId) ? (usernameId != 0 ? usernameId : (int?)null) : null;
                user.empcodeId = int.TryParse(ddl_employeeCode.SelectedValue, out int empcodeId) ? (empcodeId != 0 ? empcodeId : (int?)null) : null;


                UserDetailsBL userbl = new UserDetailsBL();

                List<UserDetailsDO> userDo = userbl.AdvanceSearch(user);



                userDo = userDo.OrderByDescending(t => t.UserId).ToList();

                int totalRecords = userDo.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    Session["AdvSearchResViewUser"] = userDo;
                    List<UserDetailsDO> displayedUsers = userDo.GetRange(startRowIndex, endRowIndex - startRowIndex);
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
                btn_adduser.Visible = false;
                btnBack1.Visible = true;
                clearfileds();
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "AdvSearchButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private string GetNullableString(string value)
        {

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        protected void AdvSearchFunction(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btn_adduser.Visible = false;
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
                btn_adduser.Visible = true;
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
        private void clearfileds()
        {
            txt_contact.Text = string.Empty;
            ddl_username.SelectedIndex = 0;
            ddl_employeeCode.SelectedIndex = 0;
        }
        protected void AdvClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                clearfileds();
                ddlPageSelector.Visible = false;
                Session["AdvSearchResViewUser"] = null;
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "AdvSearchButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void AddFunction(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Adduser.aspx", false);
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "AddFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void ExportFunction(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Viewuser.aspx", false);
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "ExportFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void addUserClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Adduser.aspx", false);
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "addUserClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                if (e.CommandName == "viewUser")
                {
                    string[] args = Convert.ToString(e.CommandArgument).Split('|');
                    string userId = args.Length > 0 ? args[0] : "0";
                    string employeeCode = args.Length > 1 ? args[1] : string.Empty;
                    if (IsEmployeeFlow)
                    {
                        Response.Redirect("AddEmployee.aspx?user_id=" + userId + "&emp_code=" + HttpUtility.UrlEncode(employeeCode) + "&mode=edit", false);
                    }
                    else
                    {
                        Response.Redirect("Adduser.aspx?user_id=" + userId + "&emp_code=" + HttpUtility.UrlEncode(employeeCode) + "&mode=edit", false);
                    }
                }
                else if (e.CommandName == "deleteUser")
                {
                    string[] args = Convert.ToString(e.CommandArgument).Split('|');
                    string userIdRaw = args.Length > 0 ? args[0] : "0";
                    string employeeCode = args.Length > 1 ? args[1] : string.Empty;
                    if (int.TryParse(userIdRaw, out int userId))
                    {
                        Session["DeleteUserId"] = userId;
                        Session["DeleteEmployeeCode"] = employeeCode;

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openModal", "openDeleteModal();", true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "gvUsers_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void BindGridView()
        {
            try
            {
                UserDetailsBL userDetailsBL = new UserDetailsBL();
                List<UserDetailsDO> users = GetUsersForCurrentFlow();
                ApplySorting(ref users);
                int totalRecords = users.Count;

                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<UserDetailsDO> displayeddata = users.GetRange(startRowIndex, endRowIndex - startRowIndex);
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
                errorlog.fnStoreErrorLog("Viewuser", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        public int TotalRecordCount()
        {

            UserDetailsDO userDO = new UserDetailsDO();
            UserDetailsBL userbl = new UserDetailsBL();
            List<UserDetailsDO> users = GetUsersForCurrentFlow();

            return users.Count;
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
                errorlog.fnStoreErrorLog("Viewuser", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;

                if (Session["AdvSearchResViewUser"] != null)
                {
                    List<UserDetailsDO> searchResults = (List<UserDetailsDO>)Session["AdvSearchResViewUser"];
                    searchResults = searchResults.OrderByDescending(t => t.Inserteddate).ToList();
                    ApplySorting(ref searchResults);

                    int totalRecords = searchResults.Count;
                    int pageIndex = selectedPageIndex;
                    hfPageIndexViewUser.Value = pageIndex.ToString();

                    int pageSize = gridview.PageSize;
                    int startRowIndex = pageIndex * pageSize;
                    int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                    List<UserDetailsDO> displayedUsers = searchResults.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gridview.DataSource = displayedUsers;
                    gridview.DataBind();

                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    BindGridView();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            BindGridView();
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            UserDetailsBL userDetailsBL = new UserDetailsBL();
            try
            {
                List<UserDetailsDO> createdet = GetUsersForCurrentFlow();

                if (createdet != null)
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
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<UserDetailsDO> users)
        {
            try
            {
                string sortExpression = ViewState["SortExpression"] as string;
                string sortDirection = ViewState["SortDirection"] as string;

                if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "ASC")
                    {
                        users = users.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        users = users.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Viewuser", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void Button3_ServerClick(object sender, EventArgs e)
        {
            if (IsEmployeeFlow)
            {
                Response.Redirect("~/view/modules/AddEmployee.aspx", false);
            }
            else
            {
                Response.Redirect("~/view/modules/Adduser.aspx", false);
            }
        }
     
        protected void confirmDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["DeleteUserId"] != null && int.TryParse(Session["DeleteUserId"].ToString(), out int userId))
                {
                    string employeeCode = Convert.ToString(Session["DeleteEmployeeCode"] ?? string.Empty);
                    UserDetailsBL userdetail = new UserDetailsBL();
                    UserDetailsDO users = userdetail.DeactivateUser(userId, employeeCode);

                    if (users.Status == "Success")
                    {
                        string status = users.Status;
                        string remark = users.Remarks;
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
              errorlog.fnStoreErrorLog("Viewuser", "confirmDeleteButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
           }
        }

        protected void btnBack1_click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/Viewuser.aspx", false);
        }
    }
}
