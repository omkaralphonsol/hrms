using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataObject;
using System.Globalization;

namespace HRMS.View.Modules
{
    public partial class AddUserMain : System.Web.UI.Page
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
                BindUserData(userId);
                BindDesignation();
                BindReportinngManager();
                BindCompany();
                BindEmployeeExporIntern();
            }
        }
        public void BindDesignation()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownDesigntion();
                if (list1 != null && list1.Count > 0)
                {
                    ddldesign.DataSource = list1;
                    ddldesign.DataTextField = "Text";
                    ddldesign.DataValueField = "Id";
                }
                else
                {
                    ddldesign.DataSource = null;
                }

                ddldesign.DataBind();
                ddldesign.Items.Insert(0, new ListItem("-- Please Select --", ""));

                // Optional: Set default selected value (if required)
                //string defaultValue = "17"; // e.g., Technician
                //ListItem item = ddldesign.Items.FindByValue(defaultValue);
                //if (item != null)
                //{
                //    ddldesign.SelectedValue = defaultValue;
                //}
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduser", "BindDesignation", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindReportinngManager()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownassignby();
                if (list1 != null)
                {
                    ddl_reportingmanager.DataSource = list1;
                    ddl_reportingmanager.DataTextField = "Text";
                    ddl_reportingmanager.DataValueField = "Id";
                }
                else
                {
                    ddl_reportingmanager.DataSource = null;
                }
                ddl_reportingmanager.DataBind();
                ddl_reportingmanager.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduser", "BindReportinngManager", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindCompany()
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
                errorlog.fnStoreErrorLog("Adduser", "BindCompany", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindEmployeeExporIntern()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownEmpexporIntern();
                if (list1 != null)
                {
                    ddlexporintern.DataSource = list1;
                    ddlexporintern.DataTextField = "Text";
                    ddlexporintern.DataValueField = "Id";
                }
                else
                {
                    ddlexporintern.DataSource = null;
                }
                ddlexporintern.DataBind();
                ddlexporintern.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddTask", "BindEmployeeExporIntern", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        private void BindUserData(int userId)
        {
            try
            {
                UserDetailsBL userBAL = new UserDetailsBL();
                List<UserDetailsDO> userDetailsList = new List<UserDetailsDO>();
                userDetailsList = userBAL.GetUserDetails(userId);

                if (userId != 0)
                {
                    if (userDetailsList.Count > 0)
                    {
                        UserDetailsDO userDetails = userDetailsList[0];
                        txtEmployeeCode.Text = userDetails.EmployeeCode;
                        txt_name.Text = userDetails.Username;
                        txt_fullname.Text = userDetails.user_fullname;
                        txt_email.Text = userDetails.user_mail_id;
                        txt_contact.Text = userDetails.contact_detail;
                        ddldesign.SelectedValue = userDetails.designation_name;

                        txtESICNo.Text = userDetails.ESIC_no.ToString();
                        txtPFNo.Text = userDetails.PF_no.ToString();
                        txtbranch.Text = userDetails.branch;
                        txtdept.Text = userDetails.department;
                        txtDivision.Text = userDetails.division;

                        if (userDetails.probation_period_months != null)
                            ddlprobationperiod.SelectedValue = userDetails.probation_period_months.ToString();

                        if (userDetails.date_of_joining != null)
                        {
                            DateTime doj = Convert.ToDateTime(userDetails.date_of_joining);
                            txtDateOfJoining.Text = doj.ToString("dd-MM-yyyy");
                        }
                        if (userDetails.reporting_manager != null)
                        {
                            string rep = userDetails.reporting_manager.ToString();

                            ddl_reportingmanager.SelectedValue = rep;
                        }

                        if (userDetails.employee_type != null)
                        {
                            string emptype = userDetails.employee_type.ToString();

                            ddlexporintern.SelectedValue = emptype;
                        }
                        ddlcompany.SelectedValue = userDetails.company_name;
                        if (userDetails.company_id != null)
                        {
                            string compId = userDetails.company_id.ToString();

                            ddlcompany.SelectedValue = compId;
                        }   


                        if (userDetails.designation_id != null)
                        {
                            string desigId = userDetails.designation_id.ToString();

                            ddldesign.SelectedValue = desigId;
                        }

                        txt_password.Visible = false;
                        lblpass.Visible = false;

                        if (Request.QueryString["mode"] == "edit")
                        {
                            txt_name.Enabled = true;
                            txt_fullname.Enabled = true;
                            txt_email.Enabled = true;
                            txt_contact.Enabled = true;

                            txtESICNo.Enabled = true;
                            txtPFNo.Enabled = true;
                            txtbranch.Enabled = true;
                            txtdept.Enabled = true;
                            txtDivision.Enabled = true;

                           
                            lbluser.Text = "Update User";
                            btn_submit.Text = "Update";
                            btn_submit.Visible = true;
                            btn_rest.Text = "Clear";
                            btn_rest.Visible = true;

                            ddldesign.Enabled = true;
                            ddlcompany.Enabled = true;
                            ddl_reportingmanager.Enabled = true;
                            ddlprobationperiod.Enabled = true;

                        }
                    }
                }
                else
                {
                    //txtEmployeeCode.Text = userDetailsList[0].EmployeeCode;
                   // txtEmployeeCode.Enabled = false;
                }

            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUsers", "BindUserData", "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

        }

        private UserDetailsDO MergeUserDetails(UserDetailsDO primary, UserDetailsDO secondary, UserDetailsDO mergedBase)
        {
            UserDetailsDO result = secondary ?? primary ?? mergedBase;
            if (result == null)
            {
                return null;
            }

            Func<string, string, string, string> pickString = (a, b, c) =>
                !string.IsNullOrWhiteSpace(a) ? a :
                !string.IsNullOrWhiteSpace(b) ? b :
                !string.IsNullOrWhiteSpace(c) ? c : string.Empty;

            Func<int, int, int, int> pickInt = (a, b, c) =>
                a > 0 ? a : (b > 0 ? b : (c > 0 ? c : 0));

            result.EmployeeCode = pickString(result.EmployeeCode, primary != null ? primary.EmployeeCode : null, mergedBase != null ? mergedBase.EmployeeCode : null);
            result.Username = pickString(result.Username, primary != null ? primary.Username : null, mergedBase != null ? mergedBase.Username : null);
            result.user_fullname = pickString(result.user_fullname, primary != null ? primary.user_fullname : null, mergedBase != null ? mergedBase.user_fullname : null);
            result.user_mail_id = pickString(result.user_mail_id, primary != null ? primary.user_mail_id : null, mergedBase != null ? mergedBase.user_mail_id : null);
            result.contact_detail = pickString(result.contact_detail, primary != null ? primary.contact_detail : null, mergedBase != null ? mergedBase.contact_detail : null);
            result.designation_name = pickString(result.designation_name, primary != null ? primary.designation_name : null, mergedBase != null ? mergedBase.designation_name : null);

            result.designation_id = pickInt(result.designation_id, primary != null ? primary.designation_id : 0, mergedBase != null ? mergedBase.designation_id : 0);
            result.company_id = pickInt(result.company_id, primary != null ? primary.company_id : 0, mergedBase != null ? mergedBase.company_id : 0);
            result.CompanyId = pickInt(result.CompanyId, primary != null ? primary.CompanyId : 0, mergedBase != null ? mergedBase.CompanyId : 0);
            result.company_name = pickString(result.company_name, primary != null ? primary.company_name : null, mergedBase != null ? mergedBase.company_name : null);

            result.ESIC_no = pickInt(result.ESIC_no, primary != null ? primary.ESIC_no : 0, mergedBase != null ? mergedBase.ESIC_no : 0);
            result.PF_no = pickInt(result.PF_no, primary != null ? primary.PF_no : 0, mergedBase != null ? mergedBase.PF_no : 0);
            result.department = pickString(result.department, primary != null ? primary.department : null, mergedBase != null ? mergedBase.department : null);
            result.branch = pickString(result.branch, primary != null ? primary.branch : null, mergedBase != null ? mergedBase.branch : null);
            result.division = pickString(result.division, primary != null ? primary.division : null, mergedBase != null ? mergedBase.division : null);
            result.reporting_manager = pickString(result.reporting_manager, primary != null ? primary.reporting_manager : null, mergedBase != null ? mergedBase.reporting_manager : null);
            result.employee_type = pickString(result.employee_type, primary != null ? primary.employee_type : null, mergedBase != null ? mergedBase.employee_type : null);
            result.probation_period_months = pickInt(result.probation_period_months, primary != null ? primary.probation_period_months : 0, mergedBase != null ? mergedBase.probation_period_months : 0);

            if (result.date_of_joining == DateTime.MinValue)
            {
                if (primary != null && primary.date_of_joining > DateTime.MinValue)
                {
                    result.date_of_joining = primary.date_of_joining;
                }
                else if (mergedBase != null && mergedBase.date_of_joining > DateTime.MinValue)
                {
                    result.date_of_joining = mergedBase.date_of_joining;
                }
            }

            return result;
        }

        private bool IsOnlyBasicUserData(UserDetailsDO user)
        {
            if (user == null) return true;

            bool hasAnyEmploymentData =
                user.ESIC_no > 0 ||
                user.PF_no > 0 ||
                !string.IsNullOrWhiteSpace(user.department) ||
                !string.IsNullOrWhiteSpace(user.branch) ||
                !string.IsNullOrWhiteSpace(user.division) ||
                user.probation_period_months > 0 ||
                !string.IsNullOrWhiteSpace(user.reporting_manager) ||
                !string.IsNullOrWhiteSpace(user.employee_type) ||
                user.date_of_joining > DateTime.MinValue;

            return !hasAnyEmploymentData;
        }
        protected void SubmitButtonClick(object sender, EventArgs e)
        {
            UserDetailsDO user = new UserDetailsDO();
            ResponseDO response = new ResponseDO();
            try
            {

                if (btn_submit.Text == "Submit")
                {
                    int userId = GetClientIdFromSession();

                    user.user_type = "2"; //11 means emp id
                    user.Insertedby = userId;
                    user.CompanyId = 3; // Default
                    UserDetailsBL userdetails = new UserDetailsBL();
                    user.Username = txt_name.Text;
                    user.user_fullname = txt_fullname.Text;
                    user.user_mail_id = txt_email.Text;
                    user.contact_detail = txt_contact.Text;
                    user.password = "pass@123";
                    user.EmployeeCode = txtEmployeeCode.Text;
                    user.designation_id = Convert.ToInt32(ddldesign.SelectedValue);
                    user.designation_name = ddldesign.SelectedItem != null ? ddldesign.SelectedItem.Text : string.Empty;
                    string emailId = user.user_mail_id;

                    user.company_id = Convert.ToInt32(ddlcompany.SelectedValue);

                    user.ESIC_no = string.IsNullOrWhiteSpace(txtESICNo.Text) ? 0 : Convert.ToInt32(txtESICNo.Text);
                    user.PF_no = string.IsNullOrWhiteSpace(txtPFNo.Text) ? 0 : Convert.ToInt32(txtPFNo.Text);
                    user.branch = txtbranch.Text;
                    user.department = txtdept.Text;
                    user.division = txtDivision.Text;
                    DateTime joiningDate;
                    if (!DateTime.TryParseExact(txtDateOfJoining.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out joiningDate))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('Failed', 'Please enter Date Of Joining in dd-MM-yyyy format.');", true);
                        return;
                    }
                    user.date_of_joining = joiningDate;
                    user.probation_period_months = Convert.ToInt32(ddlprobationperiod.SelectedValue);
                    user.reporting_manager = string.IsNullOrWhiteSpace(ddl_reportingmanager.SelectedValue) ? "0" : ddl_reportingmanager.SelectedValue;
                    user.employee_type = ddlexporintern.SelectedValue;

                    string username = user.Username;
                    string password = user.password;
                    List<UserDetailsDO> userdata = userdetails.SaveUserDetailsMainDb(user);
                    if (string.Equals(userdata[0].Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            string status = userdata[0].Status;
                            string remark = userdata[0].Remarks;

                            ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = 'ViewUser.aspx'; }, 5000);", true);
                            userdetails.SendUserCredentialsMail(emailId, password);

                            ClearButton_Click(sender, e);
                        }
                        catch (Exception ex)
                        {
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("addUser", "SubmitButtonClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                        }
                    }
                    else
                    {
                        string status = userdata[0].Status;
                        string remark = userdata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = 'ViewUser.aspx'; }, 5000);", true);
                        return;
                    }

                }
                else if (btn_submit.Text == "Update")
                {
                    user.UserId = Convert.ToInt32(Request.QueryString["user_id"]);
                    UserDetailsBL userdetails = new UserDetailsBL();
                    user.EmployeeCode = txtEmployeeCode.Text;
                    user.Username = txt_name.Text;
                    user.user_fullname = txt_fullname.Text;
                    user.designation_id = Convert.ToInt32(ddldesign.SelectedValue);
                    user.user_mail_id = txt_email.Text;
                    user.contact_detail = txt_contact.Text;

                    user.company_id = Convert.ToInt32(ddlcompany.SelectedValue);

                    user.ESIC_no = string.IsNullOrWhiteSpace(txtESICNo.Text) ? 0 : Convert.ToInt32(txtESICNo.Text);
                    user.PF_no = string.IsNullOrWhiteSpace(txtPFNo.Text) ? 0 : Convert.ToInt32(txtPFNo.Text);
                    user.branch = txtbranch.Text;
                    user.department = txtdept.Text;
                    user.division = txtDivision.Text;
                    user.date_of_joining = Convert.ToDateTime(txtDateOfJoining.Text);
                    user.probation_period_months = Convert.ToInt32(ddlprobationperiod.SelectedValue);
                    user.reporting_manager = ddl_reportingmanager.SelectedValue;
                    user.employee_type = ddlexporintern.SelectedValue;
                    //user.password = txt_password.Text;
                    List<UserDetailsDO> userdata = userdetails.UpdateUserDetailsMainDb(user);
                    if (userdata == null || userdata.Count == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('Failed', 'Update response not received from database.');", true);
                        return;
                    }

                    string status = Convert.ToString(userdata[0].Status ?? string.Empty).Trim();
                    string remark = Convert.ToString(userdata[0].Remarks ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(remark))
                    {
                        remark = string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase)
                            ? "User updated successfully."
                            : "User update failed.";
                    }

                    if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = 'ViewUser.aspx'; }, 4000);", true);
                        return;
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showUserSavedMessage('" + status + "', '" + remark + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("addUser", "SubmitButtonClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                txtEmployeeCode.Text = string.Empty;
                txt_name.Text = string.Empty;
                txt_fullname.Text = string.Empty;
                ddldesign.SelectedIndex = 0;
                txt_email.Text = string.Empty;
                txt_contact.Text = string.Empty;
                txt_password.Text = string.Empty;

                ddlcompany.SelectedIndex = 0;
                ddl_reportingmanager.SelectedIndex = 0;
                ddlprobationperiod.SelectedIndex = 0;
                txtdept.Text = string.Empty;
                txtDivision.Text = string.Empty;
                txtESICNo.Text = string.Empty;
                txtPFNo.Text = string.Empty;
                txtbranch.Text = string.Empty;
                txtDateOfJoining.Text = string.Empty;
                ddlexporintern.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUser", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/Viewuser.aspx", false);
        }

        private int GetClientIdFromSession()
        {
            int userId = 0;
            if (Session["UserId"] != null)
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            return userId;
        }
    }
}
