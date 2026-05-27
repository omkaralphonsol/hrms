using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data;
using System.ComponentModel.Design;


namespace HRMS.View.Modules
{
    public partial class TerminationList : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            if (!IsPostBack)
            {

                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                if (Request.QueryString["user_id"] != null)
                {
                    userId = Convert.ToInt32(Request.QueryString["user_id"]);
                }
                else
                {
                    userId = 0;
                }

                Bindcompany();
                //BindDropdownTerminationReason();
            }
        }
        public void Bindcompany()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdowCompany();
                if (list1 != null)
                {
                    ddlcompany.DataSource = list1;
                    ddlcompany.DataTextField = "Text";
                    ddlcompany.DataValueField = "Id";
                }
                else
                {
                    ddlcompany.DataSource = null;
                }
                ddlcompany.DataBind();
                ddlcompany.Items.Insert(0, new ListItem("-- Please Select --", ""));


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "Bindcompany", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnsearch_click(object sender, EventArgs e)
        {
            try
            {
                int companyId = Convert.ToInt32(ddlcompany.SelectedValue);
                Session["SelectedCompanyId"] = companyId;  // save for later (paging, sorting)

                // Always bind using selected companyId
                BindGridViewFromAPI(companyId);

                gridview.Visible = gridview.Rows.Count > 0;
                paginationContainer.Visible = gridview.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                gridview.Visible = false;
                paginationContainer.Visible = false;
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Error",
                    $"showUserSavedMessage('Error', 'Please search Company');",
                    true
                );
            }
        }




        protected void btnclear_click(object sender, EventArgs e)
        {
            try
            {
                ddlcompany.SelectedIndex = 0; // reset company dropdown
                gridview.DataSource = null;
                gridview.DataBind();
                gridview.Visible = false;     // hide grid on clear
                ddlPageSelector.Visible = false; // hide pagination dropdown
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Error",
                    $"showsalarySavedMessage('Error', 'Clear failed: {ex.Message}');", true);
            }
        }

        protected void BindGridViewFromAPI(int companyId)
        {
            try
            {
                var users = GetUsersFromAPI(companyId);

                HandoverprocessBL bl = new HandoverprocessBL();
                List<UserDetailsDO> terminationList = bl.GetTerminationList(companyId);


                // Prefer UserId-based matching for accuracy; fallback to EmployeeCode.
                var terminationByUserId = terminationList
                    .GroupBy(t => t.UserId)
                    .ToDictionary(g => g.Key, g => g.First());

                var terminationByEmpCode = terminationList
                    .Where(t => !string.IsNullOrWhiteSpace(t.EmployeeCode))
                    .ToLookup(t => t.EmployeeCode.Trim());

                var mergedData = users
                    .Select(u =>
                    {
                        string empCode = (u.EmployeeCode ?? "").Trim();
                        UserDetailsDO term = null;

                        if (!terminationByUserId.TryGetValue(u.UserId, out term))
                        {
                            term = terminationByEmpCode[empCode].FirstOrDefault();
                        }

                        return new UserDetailsDO
                        {
                            UserId = u.UserId,
                            EmployeeCode = u.EmployeeCode,
                            user_fullname = u.user_fullname,
                            user_mail_id = u.user_mail_id,
                            TerminationDate = term?.TerminationDate,
                            notice_status = (term?.notice_status ?? "Pending").Trim().ToLower(),
                        };
                    })
                    .ToList();

                ApplySorting(ref mergedData);

                // Pagination
                int totalRecords = mergedData.Count();   // ✅ Important

                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    var displayedData = mergedData
                        .Skip(startRowIndex)
                        .Take(pageSize)
                        .ToList();

                    gridview.DataSource = displayedData;
                    gridview.DataBind();
                    gridview.Visible = true;

                    //foreach (GridViewRow row in gridview.Rows)
                    //{
                    //    if (row.RowType == DataControlRowType.DataRow)
                    //    {
                    //        Button btnPending = (Button)row.FindControl("btnPending");
                    //        Button btnResponded = (Button)row.FindControl("btnResponded");

                    //        var data = displayedData[row.RowIndex];

                    //        if (data.notice_status == "Response_pending")
                    //        {
                    //            // Pending already clicked → disable Pending
                    //            btnPending.Enabled = false;

                    //            // Responded enabled only after 15 days
                    //            btnResponded.Enabled = data.ResponseDeadline.HasValue &&
                    //                                   DateTime.Now.Date >= data.ResponseDeadline.Value.AddDays(15);
                    //        }
                    //        else if (data.notice_status == "Responded")
                    //        {
                    //            // Both buttons disabled
                    //            btnPending.Enabled = false;
                    //            btnResponded.Enabled = false;
                    //        }
                    //        else
                    //        {
                    //            // Initial state: enable Pending, disable Responded
                    //            btnPending.Enabled = true;
                    //            btnResponded.Enabled = true;
                    //        }
                    //    }
                    //}


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
                    gridview.Visible = false;
                    ddlPageSelector.Visible = false;

                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gridview.Visible = false;

                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeAction",
                    "BindGridViewFromAPI",
                    "Exception Message: " + ex.Message +
                    " StackTrace: " + ex.StackTrace,
                    UserId);
            }
        }


        protected List<UserDetailsDO> GetUsersFromAPI(int companyId)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                UserDetailsBL userDetailsBL = new UserDetailsBL();
                users = userDetailsBL.ViewAllUsers();

                if (companyId > 0)
                {
                    users = users.Where(u =>
                        u.company_id == companyId ||
                        u.CompanyId == companyId
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "GetUsersFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
            return users;
        }


        public int TotalRecordCount()
        {

            UserDetailsDO userDO = new UserDetailsDO();
            UserDetailsBL userbl = new UserDetailsBL();
            List<UserDetailsDO> users = userbl.ViewAllUsers();

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
                errorlog.fnStoreErrorLog("EmployeeAction", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindGridViewFromAPI(companyId);

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            //BindGridView();
            int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
            BindGridViewFromAPI(companyId);
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            UserDetailsBL userDetailsBL = new UserDetailsBL();
            try
            {
                List<UserDetailsDO> createdet = userDetailsBL.ViewAllUsers();

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
                errorlog.fnStoreErrorLog("EmployeeAction", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("EmployeeAction", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
      
        protected void btnPending_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);

            HandoverprocessBL bl = new HandoverprocessBL();
            var data = bl.GetTerminationByUserId(userId);

            if (data == null || !data.ResponseDeadline.HasValue)
            {
                ShowMessage("Cannot process response!", "error");
                return;
            }

            DateTime allowedDate = data.ResponseDeadline.Value.AddDays(15);

            if (DateTime.Now.Date < allowedDate)
            {
                ShowMessage("You can respond only after 15 days!", "error");
                return;
            }

            bl.UpdateNoticeStatus(userId, "Response_pending");
            ShowMessage("Response submitted successfully!", "Success");


            btn.Enabled = false;


        }


        protected void btnResponded_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);

            HandoverprocessBL bl = new HandoverprocessBL();
            var data = bl.GetTerminationByUserId(userId);

            if (data == null || !data.ResponseDeadline.HasValue)
            {
                ShowMessage("No response deadline data present!", "error");
                return;
            }

            DateTime allowedDate = data.ResponseDeadline.Value.AddDays(15);
            if (DateTime.Now.Date < allowedDate)
            {
                ShowMessage("You can respond only after 15 days!", "error");
                return;
            }

            bl.UpdateNoticeStatus(userId, "Responded");
            ShowMessage("Response submitted successfully!", "Success");

            btn.Enabled = false;

            // Optional disable Pending button too if you want to prevent re-click
            Button btnPending = (Button)FindControl("btnPending");
            if (btnPending != null)
                btnPending.Enabled = false;

        }

        protected void gridview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnPending = (Button)e.Row.FindControl("btnPending");
                Button btnResponded = (Button)e.Row.FindControl("btnResponded");

                var data = (UserDetailsDO)e.Row.DataItem;

                string status = (data.notice_status ?? "").Trim().ToLower();

                // Responded OR Show Cause → lock both
                if (status == "responded")
                {
                    btnPending.Enabled = false;
                    btnResponded.Enabled = false;
                }

                // Pending already clicked
                else if (status == "response_pending")
                {
                    btnPending.Enabled = false;
                    btnResponded.Enabled = true;
                }
                else
                {
                    btnPending.Enabled = true;
                    btnResponded.Enabled = true;
                }
            }
        }

        private void ShowMessage(string msg, string type)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "alert",
                $"showUserSavedMessage('{type}','{msg}');",
                true
            );
        }

    }
}
