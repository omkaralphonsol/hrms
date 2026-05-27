using DataObject;
using Newtonsoft.Json;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ResignationList : System.Web.UI.Page
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

                BindResignationGrid();
            }
        }
        protected void BindResignationGrid()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                var users = userBL.ViewAllUsers().OrderByDescending(u => u.UserId).ToList();
                var resignationRows = GetResignationsFromAPI();

                var resignationMap = resignationRows
                    .GroupBy(r => r.UserId)
                    .ToDictionary(g => g.Key, g => g.First());

                var resignations = users.Select(u =>
                {
                    ResignationDO r;
                    resignationMap.TryGetValue(u.UserId, out r);

                    return new ResignationDO
                    {
                        EmployeeResignationId = r != null ? r.EmployeeResignationId : 0,
                        UserId = u.UserId,
                        EmployeeName = u.user_fullname,
                        EmployeeEmail = u.user_mail_id,
                        resignation_date = r != null ? r.resignation_date : DateTime.MinValue,
                        notice_period_days = r != null ? r.notice_period_days : 0,
                        last_working_date = r != null ? r.last_working_date : DateTime.MinValue,
                        reason = r != null ? r.reason : "-",
                        hr_status = r != null ? r.hr_status : "Pending",
                        last_working_date_display = r != null ? r.last_working_date_display : "-"
                    };
                }).ToList();

                ApplySorting(ref resignations); 

                int totalRecords = resignations.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<ResignationDO> displayedData = resignations.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gvResignations.DataSource = displayedData; 
                    gvResignations.DataBind();
                    gvResignations.Visible = true;

                    if (totalRecords > pageSize)
                    {
                        paginationContainer.Visible = true;
                        ddlPageSelector.Visible = true;
                        UpdatePageInfoLabel(pageIndex, totalRecords);
                    }
                    else
                    {
                        paginationContainer.Visible = false;
                        ddlPageSelector.Visible = false;
                    }
                }
                else
                {
                    gvResignations.DataSource = null;
                    gvResignations.DataBind();
                    gvResignations.Visible = false;
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gvResignations.Visible = false;
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationBL", "BindResignationGridFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }
        protected List<ResignationDO> GetResignationsFromAPI()
        {
            List<ResignationDO> users = new List<ResignationDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    //client.BaseAddress = new Uri("https://localhost:44360/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                    );

                    HttpResponseMessage response =
        client.PostAsync(
            "UserList/GetResignationDetails",
            new StringContent("{}", Encoding.UTF8, "application/json")
        ).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<ResignationResponseDO>(jsonString);

                        if (result.Success && result.ResignationList != null)
                        {
                            users = result.ResignationList.Select(u => new ResignationDO
                            {
                                EmployeeResignationId = u.EmployeeResignationId,
                                UserId = u.UserId,
                                EmployeeName = u.EmployeeName,
                                EmployeeEmail = u.EmployeeEmail,
                                resignation_date = u.resignation_date,
                                notice_period_days = u.notice_period_days,
                                last_working_date = u.last_working_date,
                                reason = u.reason,
                                hr_status = u.hr_status,
                                last_working_date_display=u.last_working_date_display
                            }).ToList();

                            UserDetailsBL userBL = new UserDetailsBL();
                            var userMap = userBL.ViewAllUsers()
                                .GroupBy(x => x.UserId)
                                .ToDictionary(g => g.Key, g => g.First());

                            users = users.Where(r => userMap.ContainsKey(r.UserId)).ToList();

                            foreach (var row in users)
                            {
                                UserDetailsDO userInfo;
                                if (userMap.TryGetValue(row.UserId, out userInfo))
                                {
                                    row.EmployeeName = string.IsNullOrWhiteSpace(userInfo.user_fullname) ? row.EmployeeName : userInfo.user_fullname;
                                    row.EmployeeEmail = string.IsNullOrWhiteSpace(userInfo.user_mail_id) ? row.EmployeeEmail : userInfo.user_mail_id;
                                }
                            }
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

        protected void btnSubmitResignationAction_Click(object sender, EventArgs e)
        {
            int resignationId = Convert.ToInt32(hfResignationId.Value);
            string action = hfHrAction.Value;  // Use hidden field
            string remark = txtHrRemark.Text.Trim();

            int? extendedDays = string.IsNullOrEmpty(txtExtendedDays.Text)
                ? null : (int?)Convert.ToInt32(txtExtendedDays.Text);

            DateTime? lastWorkingDate = string.IsNullOrEmpty(txtLastWorkingDate.Text)
                ? null : (DateTime?)Convert.ToDateTime(txtLastWorkingDate.Text);

            if (action == "Rejected" && string.IsNullOrEmpty(remark))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please enter HR remark for rejection.');", true);
                return;
            }

            var requestObj = new
            {
                EmployeeResignationId = resignationId,
                HrAction = action,
                HrRemarks = remark,
                LastWorkingDate = lastWorkingDate,
                ExtendedNoticeDays = extendedDays
            };

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                   // client.BaseAddress = new Uri("https://localhost:44360/");

                    string json = JsonConvert.SerializeObject(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync("UserList/UpdateResignationAction", content).Result;
                    var resultJson = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResignationActionResponseDO>(resultJson);

                    if (result != null && result.Success)
                    {
                        ScriptManager.RegisterStartupScript(
                             this,
                             GetType(),
                             "ResignationSavedScript",
                             $"showUserSavedMessage('Success', '{result.ResponseMsg}');",
                             true
                         );
                        SendResignationEmail(resignationId, action, remark, lastWorkingDate);

                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                             this,
                             GetType(),
                             "ResignationSavedScript",
                             $"showUserSavedMessage('Error', '{result?.ResponseMsg ?? "Unknown error"}');",
                             true
                         );

                    }

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }

            BindResignationGrid();
            // Close modal after showing message
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeResignationModal(); resetResignationFields();", true);
        }


        private void SendResignationEmail(int resignationId, string action, string remark, DateTime? lastWorkingDate)
        {
            try
            {
                var resignations = GetResignationsFromAPI();
                var resignation = resignations.FirstOrDefault(r => r.EmployeeResignationId == resignationId);

                if (resignation == null || string.IsNullOrEmpty(resignation.EmployeeEmail))
                    return;

                string employeeEmail = resignation.EmployeeEmail;
                string employeeName = resignation.EmployeeName;

                string subject = $"Your Resignation has been {action}";

                string statusColor = action.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ? "#28a745" : "#dc3545";

                string body = $@"
        <div style='font-family: Arial, sans-serif; line-height:1.6; color:#333;'>
            <h2 style='color:{statusColor};'>Your Resignation has been {action}</h2>
            <p>Dear <strong>{employeeName}</strong>,</p>";

                if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                {
                    body += $@"
            <p>Your resignation has been <strong style='color:{statusColor};'>accepted</strong> by HR.</p>
            <p>Your last working date is: <strong>{lastWorkingDate?.ToString("dd-MMM-yyyy")}</strong></p>";
                }
                else if (action.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    body += $@"
            <p>Your resignation has been <strong style='color:{statusColor};'>rejected</strong> by HR.</p>
            <p>HR Remark: <strong>{remark}</strong></p>";
                }

                body += @"
            <hr style='border:none; border-top:1px solid #ccc;'/>
            <p>Regards,<br/>HR Team</p>
        </div>";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System");
                    mail.To.Add(employeeEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "emailError", $"console.error('Email Error: {ex.Message}');", true);
            }
        }

        public int TotalRecordCount()
        {

            UserDetailsDO userDO = new UserDetailsDO();
            UserDetailsBL userbl = new UserDetailsBL();
            List<UserDetailsDO> users = userbl.ViewAllUsers();

            return users.Count;
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;

                if (Session["AdvSearchResViewUser"] != null)
                {
                    List<ResignationDO> searchResults = (List<ResignationDO>)Session["AdvSearchResViewUser"];
                    //searchResults = searchResults.OrderByDescending(t => t.Inserteddate).ToList();
                    ApplySorting(ref searchResults);

                    int totalRecords = searchResults.Count;
                    int pageIndex = selectedPageIndex;
                    hfPageIndexViewUser.Value = pageIndex.ToString();

                    int pageSize = gvResignations.PageSize;
                    int startRowIndex = pageIndex * pageSize;
                    int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                    List<ResignationDO> displayedUsers = searchResults.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gvResignations.DataSource = displayedUsers;
                    gvResignations.DataBind();

                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindResignationGrid();

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResignations.PageIndex = e.NewPageIndex;
            BindResignationGrid();
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

                    gvResignations.DataSource = createdet;
                    gvResignations.DataBind();
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<ResignationDO> users)
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
                errorlog.fnStoreErrorLog("ResignationList", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("ResignationList", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int resignationId = Convert.ToInt32(e.CommandArgument);
                if (resignationId <= 0)
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "noResignation",
                        "showUserSavedMessage('Error', 'No resignation request found for this user.');", true);
                    return;
                }
                hfResignationId.Value = resignationId.ToString();

                if (e.CommandName == "Accept")
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "openModal", "openResignationModal('Accepted');", true);
                }

                if (e.CommandName == "Reject")
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "openModal", "openResignationModal('Rejected');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "ResignationList",
                    "gvUsers_RowCommand",
                    ex.Message,
                    UserId);
            }
        }



    }
}
