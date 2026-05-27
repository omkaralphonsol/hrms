using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
    public partial class EmployeeAction : System.Web.UI.Page
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
        //public void BindDropdownTerminationReason()
        //{
        //    List<DropDownData> list1 = new List<DropDownData>();
        //    CommonBL commonbl = new CommonBL();
        //    try
        //    {
        //        list1 = commonbl.dropdowterminationReason();
        //        if (list1 != null)
        //        {
        //            ddlTerminationReason.DataSource = list1;
        //            ddlTerminationReason.DataTextField = "Text";
        //            ddlTerminationReason.DataValueField = "Id";
        //        }
        //        else
        //        {
        //            ddlTerminationReason.DataSource = null;
        //        }
        //        ddlTerminationReason.DataBind();
        //        ddlTerminationReason.Items.Insert(0, new ListItem("-- Please Select --", ""));


        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("EmployeeAction", "BindDropdownTerminationReason", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
        //    }
        //}
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
                errorlog.fnStoreErrorLog("EmployeeAction", "BindGridViewFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
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
              ;
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
        //protected void btnConfirmTermination_Click(object sender, EventArgs e)
        //{

        //    TerminationProcessDO obj = new TerminationProcessDO
        //    {
        //        CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]),
        //        UserId = Convert.ToInt32(hfUserId.Value),
        //        EmployeeCode = hfEmployeeCode.Value,
        //        TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
        //        //TerminationReasonId = Convert.ToInt32(ddlTerminationReason.SelectedValue),
        //       // Remark = txtTerminationRemark.Text.Trim(),
        //        InsertedBy = Convert.ToInt32(Session["UserId"])
        //    };

        //    HandoverprocessBL bl = new HandoverprocessBL();
        //    var result = bl.SaveEmployeeTermination(obj);

        //    if (result.Count > 0 && result[0].Status == 1)
        //    {
        //        // ✅ Clear controls
        //        ClearTerminationForm();

        //        // ✅ Success message
        //        ScriptManager.RegisterStartupScript(
        //            this,
        //            GetType(),
        //            "TerminationSuccess",
        //            $"showUserSavedMessage('Success', '{result[0].Message}');",
        //            true
        //        );

        //        // ✅ Close modal (optional but recommended)
        //        ScriptManager.RegisterStartupScript(
        //            this,
        //            GetType(),
        //            "CloseModal",
        //            "var m=document.getElementById('terminationModal'); if(m){bootstrap.Modal.getInstance(m)?.hide();}",
        //            true
        //        );

        //        // ✅ Refresh grid
        //        BindGridViewFromAPI(obj.CompanyId);
        //    }
        //}
        protected void btnConfirmTermination_Click(object sender, EventArgs e)
        {
            //int userId = 0;
            //if (!string.IsNullOrWhiteSpace(hfUserId.Value))
            //    int.TryParse(hfUserId.Value, out userId); // safe conversion

            //string employeeCode = hfEmployeeCode?.Value ?? "";
            //string terminationType = hfTerminationType?.Value ?? "";
            //string employeeEmail = hfEmployeeEmail?.Value ?? "";
            //string employeeName = hfEmployeeName?.Value ?? "";

            if (!ValidateTerminationForm(out string errorMessage))
            {
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "validation",
                    $"showUserSavedMessage('Validation Error','{errorMessage}');",
                    true);

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "keepopen",
                    "var m = new bootstrap.Modal(document.getElementById('terminationModal')); m.show();",
                    true);

                return; // stop execution
            }


            TerminationProcessDO obj = new TerminationProcessDO
            {
                CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]),
                UserId = Convert.ToInt32(hfUserId.Value),
                EmployeeCode = hfEmployeeCode.Value,
                TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
                termination_reason = hfTerminationType.Value,
                InsertedBy = Convert.ToInt32(Session["UserId"]),
                EmployeeEmail = hfEmployeeEmail.Value,   
                EmployeeName = hfEmployeeName.Value
                //CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]),
                //UserId = userId,                
                //EmployeeCode = employeeCode,
                //TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
                //termination_reason = terminationType,
                //InsertedBy = Convert.ToInt32(Session["UserId"]),
                //EmployeeEmail = employeeEmail,
                //EmployeeName = employeeName

            };

            // PERFORMANCE BASED
            if (hfTerminationType.Value == "Performance")
            {

                obj.termination_reason = "Performance Based Letter";

                if (!string.IsNullOrEmpty(hfPerformanceRating.Value))
                    obj.PerformanceRating = Convert.ToInt32(hfPerformanceRating.Value);
                else
                    obj.PerformanceRating = null;

                obj.NoticePeriodDays = Convert.ToInt32(ddlNoticePeriod.SelectedValue);
                obj.TerminationLetter = txtLetterPreview.Text.Trim();
            }

            // SHOW CAUSE
            if (hfTerminationType.Value == "ShowCause")
            {
                obj.termination_reason = "Show Cause Notice";

                if (!string.IsNullOrWhiteSpace(txtResponseDeadline.Text))
                    obj.ResponseDeadline = Convert.ToDateTime(txtResponseDeadline.Text);
                else
                    obj.ResponseDeadline = null;

                obj.NoticeLetter = txtNoticeLetter.Text.Trim();
            }

            HandoverprocessBL bl = new HandoverprocessBL();
            var result = bl.SaveEmployeeTermination(obj);

            if (result.Count > 0 && result[0].Status == 1)
            {
                SendTerminationEmail(obj);
                ClearTerminationForm();

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "ok",
                    $"showUserSavedMessage('Success','{result[0].Message}');", true);

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "close",
                    "bootstrap.Modal.getInstance(document.getElementById('terminationModal'))?.hide();",
                    true);

                BindGridViewFromAPI(obj.CompanyId);
            }
        }

        private bool ValidateTerminationForm(out string errorMessage)
        {
            errorMessage = "";

            // Performance Based validation
            if (hfTerminationType.Value == "Performance")
            {
                if (string.IsNullOrWhiteSpace(hfPerformanceRating.Value))
                {
                    errorMessage = "Performance rating is required.";
                    return false;
                }

                if (ddlNoticePeriod.SelectedIndex == 0)
                {
                    errorMessage = "Please select notice period.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtLetterPreview.Text))
                {
                    errorMessage = "Termination letter cannot be empty.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtTerminationDate.Text))
                {
                    errorMessage = "Please select termination date.";
                    return false;
                }

                if (!DateTime.TryParse(txtTerminationDate.Text, out _))
                {
                    errorMessage = "Invalid termination date.";
                    return false;
                }
            }

            if (hfTerminationType.Value == "ShowCause")
            {
                if (string.IsNullOrWhiteSpace(txtResponseDeadline.Text))
                {
                    errorMessage = "Response deadline is required.";
                    return false;
                }

                if (!DateTime.TryParse(txtResponseDeadline.Text, out _))
                {
                    errorMessage = "Invalid response deadline.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtNoticeLetter.Text))
                {
                    errorMessage = "Notice letter cannot be empty.";
                    return false;
                }
                
            }

            return true;
        }

        private void SendTerminationEmail(TerminationProcessDO obj)
        {
            try
            {
                if (obj == null)
                    return;

                string employeeEmail = obj.EmployeeEmail;
                string employeeName = obj.EmployeeName;


                if (string.IsNullOrEmpty(employeeEmail))
                    return;

                string subject = "Employment Termination Notice";

                string body = $@"
<div style='font-family: Arial, sans-serif; line-height:1.6; color:#333;'>
    <h2 style='color:#dc3545;'>Employment Termination</h2>
    <p>Dear <strong>{employeeName}</strong>,</p>

    <p>This is to formally inform you that your employment has been terminated.</p>

    <table style='border-collapse:collapse; width:100%; margin-top:10px;'>
        <tr>
            <td style='padding:8px; border:1px solid #ddd;'><strong>Termination Reason</strong></td>
            <td style='padding:8px; border:1px solid #ddd;'>{obj.termination_reason}</td>
        </tr>
        <tr>
            <td style='padding:8px; border:1px solid #ddd;'><strong>Termination Date</strong></td>
            <td style='padding:8px; border:1px solid #ddd;'>{obj.TerminationDate:dd-MMM-yyyy}</td>
        </tr>";

                if (obj.termination_reason == "Performance Based Letter")
                {
                    body += $@"
        <tr>
            <td style='padding:8px; border:1px solid #ddd;'><strong>Notice Period</strong></td>
            <td style='padding:8px; border:1px solid #ddd;'>{obj.NoticePeriodDays} days</td>
        </tr>
    </table>

    <h4 style='margin-top:15px;'>Termination Letter</h4>
    <div style='padding:10px; border:1px solid #ccc;'>
        {obj.TerminationLetter}
    </div>";
                }

                else if (obj.termination_reason == "Show Cause Notice")
                {
                    body += $@"
        <tr>
            <td style='padding:8px; border:1px solid #ddd;'><strong>Response Deadline</strong></td>
            <td style='padding:8px; border:1px solid #ddd;'>{obj.ResponseDeadline:dd-MMM-yyyy}</td>
        </tr>
    </table>

    <h4 style='margin-top:15px;'>Notice</h4>
    <div style='padding:10px; border:1px solid #ccc;'>
        {obj.NoticeLetter}
    </div>";
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
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "emailError",
                    $"console.error('Email Error: {ex.Message}');", true);
            }
        }


        private void ClearTerminationForm()
            {
                txtTerminationDate.Text = string.Empty;

                hfUserId.Value = string.Empty;
                hfEmployeeCode.Value = string.Empty;
                hfTerminationType.Value = string.Empty;
                hfPerformanceRating.Value = string.Empty;

                txtLetterPreview.Text = string.Empty;

                if (ddlNoticePeriod.Items.Count > 0)
                    ddlNoticePeriod.SelectedIndex = 0;

                txtResponseDeadline.Text = string.Empty;
                txtNoticeLetter.Text = string.Empty;

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

        private string GetShowCauseEmailTemplate(
    string employeeName,
    string companyName,
    string issueDetails,
    DateTime deadline,
    string signatoryName,
    string designation)
        {
            return $@"
<div style='font-family:Calibri,Arial; line-height:1.7; color:#000;'>

    <h3 style='text-align:center;'>Sample Show Cause Notice – Misconduct / Performance Issue</h3>

    <p><strong>Subject:</strong> Show Cause Notice – Explanation Required</p>

    <p>Dear {employeeName},</p>

    <p>
    This is to formally inform you that certain concerns have been observed
    with respect to your performance / conduct / adherence to company policies
    during the course of your employment with <strong>{companyName}</strong>.
    </p>

    <p>
    It has been noted that <strong>{issueDetails}</strong>, which is not in line
    with the expectations and policies of the organization. Despite previous
    discussions, the matter remains unresolved.
    </p>

    <p>
    You are hereby called upon to show cause in writing within
    <strong>15 days</strong> from the date of receipt of this notice
    (on or before <strong>{deadline:dd-MMM-yyyy}</strong>)
    as to why disciplinary action should not be initiated.
    </p>

    <p>
    Your explanation must clearly state the reasons along with
    supporting documents, if any. Failure to respond within the
    stipulated time will be treated as absence of explanation.
    </p>

    <p>
    This notice is issued without prejudice and is part of an
    internal review process.
    </p>

    <p>We expect your cooperation in this matter.</p>

    <br/>

    <p>
    Sincerely,<br/>
    <strong>{signatoryName}</strong><br/>
    {designation}
    </p>

</div>";
        }

        
        protected void btnSendShowCause_Click(object sender, EventArgs e)
        {
            try
            {
                string empName = hfEmployeeName.Value;
                string empEmail = hfEmployeeEmail.Value;

                

          
                DateTime deadline = Convert.ToDateTime(txtResponseDeadline.Text);

                if (!DateTime.TryParse(txtResponseDeadline.Text, out deadline))
                {
                    ShowMessage("Response Deadline is not a valid date.", "error");

                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;


                }

                string issue = txtNoticeLetter?.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(issue))
                {
                    ShowMessage("Please enter the Notice Letter.", "error");

                    // Keep modal open
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                        "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;
                }

                string companyName = "Alphonsol Pvt.Ltd";
                string signatory = "HR Manager";
                string designation = "Human Resources";

                if (string.IsNullOrEmpty(empEmail))
                {
                    ShowMessage("Employee email not found.", "error");

                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;
                }

                TerminationProcessDO obj = new TerminationProcessDO();

                obj.CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                obj.UserId = Convert.ToInt32(hfUserId.Value); 
                obj.EmployeeCode = hfEmployeeCode.Value;

                obj.NoticeLetter = issue;
                obj.ResponseDeadline = deadline;
                obj.InsertedBy = Convert.ToInt32(Session["UserId"]);

                string letterHtml = GetShowCauseEmailTemplate(
                    empName,
                    companyName,
                    issue,
                    deadline,
                    signatory,
                    designation
                );

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                string subject = "Show Cause Notice – Explanation Required";
                string body = letterHtml;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System");
                    mail.To.Add(empEmail);
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

                HandoverprocessBL bl = new HandoverprocessBL();
                var result = bl.saveshowcausenotice(obj);
                ShowMessage("Show cause notice sent successfully.", "Success");


            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, "error");
            }
        }

        public void LoadShowCauseButtonStatus()
        {
            string USERID = hfUserId.Value;

            if (string.IsNullOrEmpty(USERID))
                return;

            HandoverprocessBL bl = new HandoverprocessBL();

            string status = bl.GetShowCauseStatus(USERID);

            if (status == "Show Cause Issued" || status == "Responded" || status == "Response_pending")
            {
                btnSendShowCause.Text = "Show Cause Issued";
                btnSendShowCause.Enabled = false;
                btnSendShowCause.CssClass = "btn btn-danger";
            }
            else
            {
                btnSendShowCause.Text = "Send Show Cause Notice";
                btnSendShowCause.Enabled = true;
                btnSendShowCause.CssClass = "btn btn-danger";
            }
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectEmployee")
            {
                int index = Convert.ToInt32(e.CommandArgument); 

                var keys = gridview.DataKeys[index];

                hfUserId.Value = keys["UserId"].ToString();
                hfEmployeeCode.Value = keys["EmployeeCode"].ToString();
                hfEmployeeEmail.Value = keys["user_mail_id"].ToString();
                hfEmployeeName.Value = keys["user_fullname"].ToString();

                LoadShowCauseButtonStatus();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "openModal",
                    @"
            var myModal = new bootstrap.Modal(document.getElementById('terminationModal'));
            myModal.show();
            ",
                    true
                );
            }
        }




    }
}
