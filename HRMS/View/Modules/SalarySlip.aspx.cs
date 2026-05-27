using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class SalarySlip : System.Web.UI.Page
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
                errorlog.fnStoreErrorLog("SalarySlip", "Bindcompany", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnsearch_click(object sender, EventArgs e)
        {
            try
            {
                int companyId = 0;
                if (!int.TryParse(Convert.ToString(ddlcompany.SelectedValue), out companyId) || companyId <= 0)
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "Error",
                        "showUserSavedMessage('Error', 'Please select Company.');",
                        true
                    );
                    return;
                }

                Session["SelectedCompanyId"] = companyId;  // save for later (paging, sorting)
                Session["CurrentPageIndex"] = 0; // reset page on new search

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
                UserDetailsBL userDetailsBL = new UserDetailsBL();
                var users = userDetailsBL.ViewAllUsers()
                    .Where(u => u.company_id == companyId || u.CompanyId == companyId)
                    .ToList();

                ApplySorting(ref users); // existing sorting logic

                int totalRecords = users.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                int pageSize = 10;
                int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / pageSize) : 0;
                if (totalPages == 0)
                {
                    pageIndex = 0;
                }
                else if (pageIndex < 0 || pageIndex >= totalPages)
                {
                    pageIndex = 0;
                    Session["CurrentPageIndex"] = 0;
                }
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<UserDetailsDO> displayedData = users.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gridview.DataSource = displayedData;
                    gridview.DataBind();
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
                    gridview.Visible = false;
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gridview.Visible = false;
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("useruploaddocuments", "BindGridViewFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
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
                errorlog.fnStoreErrorLog("useruploaddocuments", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("useruploaddocuments", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                int companyId = Session["SelectedCompanyId"] != null ? Convert.ToInt32(Session["SelectedCompanyId"]) : 0;
                List<UserDetailsDO> createdet = userDetailsBL.ViewAllUsers()
                    .Where(u => companyId == 0 || u.company_id == companyId || u.CompanyId == companyId)
                    .ToList();

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
                errorlog.fnStoreErrorLog("useruploaddocuments", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "viewUser")
                {
                    string userId = e.CommandArgument.ToString();

                    int? companyId = Session["SelectedCompanyId"] != null ? Convert.ToInt32(Session["SelectedCompanyId"]) : (int?)null;

                    if (companyId == null)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            "alert('Company information missing. Please log in again.');", true);
                        return;
                    }

                    // ✅ Redirect based on company_id
                    switch (companyId)
                    {
                        case 1:
                            Response.Redirect("AddImsetsalaryForm.aspx?user_id=" + userId + "&mode=edit", false);
                            break;

                        case 2:
                            Response.Redirect("AddBioxiasalaryform.aspx?user_id=" + userId + "&mode=edit", false);
                            break;

                        case 3:
                            Response.Redirect("AddSalarySlipData.aspx?user_id=" + userId + "&mode=edit", false);
                            break;

                        default:
                            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                                "alert('Invalid company. Cannot open form.');", true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("useruploaddocuments", "gvUsers_RowCommand",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

    }
}
