using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class Remunerationdetails : System.Web.UI.Page
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
                BindMonths();
                BindYears();

                // Hide both on first load
                pnlMonth.Visible = false;
                pnlYear.Visible = false;

            }
        }
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlMonth.Visible = false;
            pnlYear.Visible = false;

            if (ddlType.SelectedValue == "Monthly")
            {
                pnlMonth.Visible = true;
            }
            else if (ddlType.SelectedValue == "Yearly")
            {
                pnlYear.Visible = true;
            }
        }

        public void BindMonths()
        {
            ddlMonth.Items.Clear();
            ddlMonth.Items.Add(new ListItem("-- Select Month --", ""));

            for (int i = 1; i <= 12; i++)
            {
                ddlMonth.Items.Add(new ListItem(
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    i.ToString()
                ));
            }
        }

        public void BindYears()
        {
            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("-- Select Year --", ""));

            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 5; year <= currentYear + 5; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }

        protected void BindGrid(string type, int? month, int? year)
        {
            try
            {
                SalarySlipBL bl = new SalarySlipBL();
                List<RemunerationDO> list = bl.GetRemunerationDetails(type, month, year);

                // Apply sorting if needed
                ApplySorting(ref list);

                int totalRecords = list.Count;
                int pageSize = 10;

                // Get current page index from session, reset if out of range
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                if (pageIndex * pageSize >= totalRecords)
                {
                    pageIndex = 0;
                    Session["CurrentPageIndex"] = 0;
                }

                hfPageIndexViewUser.Value = pageIndex.ToString();

                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                // Toggle Monthly/Yearly columns
                gridview.Columns[3].Visible = (type == "Monthly"); // MonthlyCTC
                gridview.Columns[4].Visible = (type == "Yearly");  // YearlyCTC

                // Prepare data for current page
                List<RemunerationDO> displayedData = new List<RemunerationDO>();
                if (totalRecords > 0 && startRowIndex < totalRecords)
                {
                    displayedData = list.GetRange(startRowIndex, endRowIndex - startRowIndex);
                }

                // Bind data to GridView
                gridview.DataSource = displayedData;
                gridview.DataBind();

                // Show/hide controls based on data
                bool hasData = displayedData.Count > 0;
                gridview.Visible = hasData;
                paginationContainer.Visible = hasData;
                ddlPageSelector.Visible = hasData && totalRecords > pageSize;

                UpdatePageInfoLabel(pageIndex, totalRecords);

                lblMessage.Visible = !hasData;
                if (!hasData)
                {
                    lblMessage.Text = "No records found.";
                }
            }
            catch (Exception ex)
            {
                // Hide grid and pagination on error
                gridview.Visible = false;
                paginationContainer.Visible = false;
                ddlPageSelector.Visible = false;

                // Log error
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Remuneration",
                    "BindGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );
            }
        }





        protected void btnsearch_click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            string type = ddlType.SelectedValue;

            if (string.IsNullOrEmpty(type))
            {
                lblMessage.Text = "Please select type.";
                lblMessage.Visible = true;
                return;
            }

            int? year = null;
            int? month = null;

            if (type == "Yearly")
            {
                if (!int.TryParse(ddlYear.SelectedValue, out int y))
                {
                    lblMessage.Text = "Please select year.";
                    lblMessage.Visible = true;
                    return;
                }
                year = y;
            }

            if (type == "Monthly")
            {
                if (!int.TryParse(ddlMonth.SelectedValue, out int m))
                {
                    lblMessage.Text = "Please select month.";
                    lblMessage.Visible = true;
                    return;
                }
                month = m;
            }

            gridview.PageIndex = 0;
            BindGrid(type, month, year);   // ✅ pass values directly
        }


        protected void btnclear_click(object sender, EventArgs e)
        {
            try
            {
                ddlType.SelectedIndex = 0;
                ddlMonth.SelectedIndex = 0;
                ddlYear.SelectedIndex = 0;
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


        public int TotalRecordCount()
        {

            RemunerationDO userDO = new RemunerationDO();
            SalarySlipBL userbl = new SalarySlipBL();
            string type = ViewState["Type"]?.ToString();
            int year = Convert.ToInt32(ViewState["Year"]);
            int? month = ViewState["Month"] as int?;
            List<RemunerationDO> users = userbl.GetRemunerationDetails(type, month, year);

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
                errorlog.fnStoreErrorLog("useruploaddocuments", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;
                string type = ddlType.SelectedValue;
                int? month = string.IsNullOrEmpty(ddlMonth.SelectedValue) ? (int?)null : Convert.ToInt32(ddlMonth.SelectedValue);
                int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);

                if (Session["AdvSearchResViewUser"] != null)
                {
                    List<RemunerationDO> searchResults = (List<RemunerationDO>)Session["AdvSearchResViewUser"];
                    //searchResults = searchResults.OrderByDescending(t => t.Inserteddate).ToList();
                    ApplySorting(ref searchResults);

                    int totalRecords = searchResults.Count;
                    int pageIndex = selectedPageIndex;
                    hfPageIndexViewUser.Value = pageIndex.ToString();

                    int pageSize = gridview.PageSize;
                    int startRowIndex = pageIndex * pageSize;
                    int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                    List<RemunerationDO> displayedUsers = searchResults.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gridview.DataSource = displayedUsers;
                    gridview.DataBind();

                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    //int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    //BindGridViewFromAPI(companyId);
                    BindGrid(type, month, year);

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("useruploaddocuments", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            Session["CurrentPageIndex"] = e.NewPageIndex; // store current page

            string type = ddlType.SelectedValue;
            int? month = string.IsNullOrEmpty(ddlMonth.SelectedValue) ? (int?)null : Convert.ToInt32(ddlMonth.SelectedValue);
            int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);

            BindGrid(type, month, year); // Pass only
            //BindGridView();
            //int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
            //BindGridViewFromAPI(companyId);
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            SalarySlipBL userDetailsBL = new SalarySlipBL();
            try
            {
                string type = ViewState["Type"]?.ToString();
                int year = Convert.ToInt32(ViewState["Year"]);
                int? month = ViewState["Month"] as int?;
                List<RemunerationDO> createdet = userDetailsBL.GetRemunerationDetails(type, month, year);

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
                errorlog.fnStoreErrorLog("useruploaddocuments", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<RemunerationDO> users)
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
                errorlog.fnStoreErrorLog("useruploaddocuments", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}