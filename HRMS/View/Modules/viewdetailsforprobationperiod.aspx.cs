using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace HRMS.View.Modules
{
    public partial class viewdetailsforprobationperiod : System.Web.UI.Page
    {
        protected string UserId = null;
        public static List<UserDetailsDO> userDo = new List<UserDetailsDO>();
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

                // BindGridView();
                BindUsername();
                Bindcompany();
                //BindEmpCode();
                //BindGridViewFromAPI(companyId);

            }
        }
        public void BindUsername()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();

                    var content = new StringContent("{}", Encoding.UTF8, "application/json");
                    var response = client.PostAsync("UserList/BindUserName", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        UsernameResponseDO apiResponse =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<UsernameResponseDO>(json);

                        if (apiResponse != null && apiResponse.Success && apiResponse.UserNameList != null)
                        {
                            ddl_username.DataSource = apiResponse.UserNameList;
                            ddl_username.DataTextField = "Text";
                            ddl_username.DataValueField = "Id";
                            ddl_username.DataBind();
                            ddl_username.Items.Insert(0, new ListItem("-- Please Select --", ""));
                        }
                        else
                        {
                            ddl_username.Items.Clear();
                            ddl_username.Items.Insert(0, new ListItem("-- No Users --", ""));
                        }
                    }
                    else
                    {
                        ddl_username.Items.Clear();
                        ddl_username.Items.Insert(0, new ListItem("-- API Error --", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "viewdetailsforprobationperiod",
                    "BindUsername",
                    "Exception=" + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );

                ddl_username.Items.Clear();
                ddl_username.Items.Insert(0, new ListItem("-- Error --", ""));
            }
        }
        protected void AdvSearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(txt_contact.Text) &&
                    (string.IsNullOrEmpty(ddl_username.SelectedValue) || ddl_username.SelectedValue == "0"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowRequiredFieldsScript",
                        "showUserSavedMessage('Error', 'At least one field is required.');", true);
                    return;
                }



                var apiRequest = new SearchUserRequest
                {
                    UserId = string.IsNullOrEmpty(ddl_username.SelectedValue) || ddl_username.SelectedValue == "0"
          ? (int?)null
          : Convert.ToInt32(ddl_username.SelectedValue),

                    //EmpCodeId = ddl_employeeCode.SelectedValue == "0" ? (int?)null : Convert.ToInt32(ddl_employeeCode.SelectedValue)
                    ContactDetail = GetNullableString(txt_contact.Text)

                };




                userResponseDataDO apiResult = CallApiSynchronously(apiRequest);

                if (apiResult == null || !apiResult.Success || apiResult.UsersList == null || apiResult.UsersList.Count == 0)
                {
                    gridview.DataSource = null;
                    gridview.DataBind();
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);

                    advancedSearchFields.Visible = false;
                    gridview.Visible = true;
                    searchdata.Visible = false;
                    btn_advanceserach.Visible = false;
                    btnBack1.Visible = true;
                    clearfileds();
                    return;
                }

                var userDo = apiResult.UsersList.OrderByDescending(t => t.UserId).ToList();
                int totalRecords = userDo.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                List<usersDataDO> displayedUsers = userDo.GetRange(startRowIndex, endRowIndex - startRowIndex);

                gridview.DataSource = displayedUsers;
                gridview.DataBind();

                advancedSearchFields.Visible = false;
                gridview.Visible = true;

                ddlPageSelector.Visible = (totalRecords > pageSize);
                UpdatePageInfoLabel(pageIndex, totalRecords);

                searchdata.Visible = false;
                btn_advanceserach.Visible = false;
                btnBack1.Visible = true;
                btnsearchclear.Visible = false;
                ddlcomp.Visible = false;

                clearfileds();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "AdvSearchButton_Click",
                    "Exception Message: " + ex.Message + " | StackTrace: " + ex.StackTrace, UserId);

                ClientScript.RegisterStartupScript(this.GetType(), "ErrorScript",
                    "showUserSavedMessage('Error', 'An unexpected error occurred.');", true);
            }
        }

        private userResponseDataDO CallApiSynchronously(SearchUserRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://103.118.17.144:813/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );

                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response = client.PostAsync("UserList/SearchUserData", jsonContent)
                                                   .GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string resultJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<userResponseDataDO>(resultJson);
                }
            }

            return new userResponseDataDO
            {
                Success = false,
                ResponseMsg = "API call failed."
            };
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
                //btn_adduser.Visible = false;
                Session["CurrentPageIndex"] = 0;
                advancedSearchFields.Visible = true;
                gridview.Visible = false;
                btn_advanceserach.Attributes["class"] = "btn btn-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "white";
                ddlPageSelector.Visible = false;
                btnsearchclear.Visible = false;
                ddlcomp.Visible = false;
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "AdvSearchFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void AdvBackButton_Click(object sender, EventArgs e)
        {
            try
            {
                searchdata.Visible = true;
                btn_advanceserach.Visible = true;
                //btn_adduser.Visible = true;
                advancedSearchFields.Visible = false;
                gridview.Visible = true;
                Session["AdvSearchResViewUser"] = null;
                // BindGridView();
                int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                BindGridViewFromAPI(companyId);
                btn_advanceserach.Attributes["class"] = "btn  btn-outline-dark ms-2 light-border";
                btn_advanceserach.Style["color"] = "black";
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "AdvBackButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void clearfileds()
        {
            txt_contact.Text = string.Empty;
            ddl_username.SelectedIndex = 0;
            //ddl_employeeCode.SelectedIndex = 0;
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "AdvSearchButton_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "AddFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        protected void ExportFunction(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("viewdetailsforprobationperiod.aspx", false);
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "ExportFunction", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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

                if (e.CommandName == "updateprobationperiod")
                {
                    string userId = e.CommandArgument.ToString();
                    Response.Redirect("updateprobationperiodflag.aspx?user_id=" + userId + "&mode=edit", false);
                }
                //if (e.CommandName == "viewUsers")
                //{
                //    string userId = e.CommandArgument.ToString();

                //    Response.Redirect("viewuserdocuments.aspx?user_id=" + userId + "&mode=view", false);
                //}
                else if (e.CommandName == "deleteUser")
                {
                    if (int.TryParse(e.CommandArgument.ToString(), out int userId))
                    {
                        Session["DeleteUserId"] = userId;

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openModal", "openDeleteModal();", true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "gvUsers_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        protected List<UserDetailsDO> GetUsersFromAPI(int companyId)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Send companyId as JSON
                    var content = new StringContent(
                        new JavaScriptSerializer().Serialize(new { companyId = companyId }),
                        Encoding.UTF8,
                        "application/json"
                    );

                    HttpResponseMessage response = client.PostAsync("UserList/UserData", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<userResponseDataDO>(jsonString);

                        if (result.Success && result.UsersList != null)
                        {
                            users = result.UsersList.Select(u => new UserDetailsDO
                            {
                                EmployeeCode = u.EmployeeCode,
                                UserId = u.UserId,
                                Username = u.Username,
                                user_fullname = u.User_fullname,
                                user_mail_id = u.user_mail_id,
                                contact_detail = u.contact_detail,
                                //roledescription = u.roledescription
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log error
            }
            return users;
        }
        protected void BindGridViewFromAPI(int companyId)
        {
            try
            {
                var users = GetUsersFromAPI(companyId); // ✅ pass companyId

                ApplySorting(ref users); // existing sorting logic

                int totalRecords = users.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "BindGridViewFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                    //BindGridView();
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindGridViewFromAPI(companyId);

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("viewdetailsforprobationperiod", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        //protected void Button3_ServerClick(object sender, EventArgs e)
        //{
        //    Response.Redirect("~/view/modules/Adduser.aspx", false);
        //}
        protected void btnBack1_click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/viewdetailsforprobationperiod.aspx", false);
        }
    }
}