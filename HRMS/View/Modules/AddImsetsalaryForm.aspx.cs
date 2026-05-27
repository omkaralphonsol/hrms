using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AddImsetsalaryForm : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            if (!IsPostBack)
            {
                ddlMonth.Items.Clear();
                ddlMonth.Items.Add(new ListItem("-- Select Month --", "0"));
                for (int i = 1; i <= 12; i++)
                {
                    ddlMonth.Items.Add(new ListItem(
                        System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        i.ToString()
                    ));
                }
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

                BindEmpoyeeName();

            }
        }

        //public void BindEmpoyeeName()
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("https://localhost:44360/");
        //            client.DefaultRequestHeaders.Accept.Clear();

        //            var content = new StringContent("{}", Encoding.UTF8, "application/json");
        //            var response = client.PostAsync("UserList/BindEmployeeName", content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var json = response.Content.ReadAsStringAsync().Result;
        //                EmployeeNameResponseDO apiResponse =
        //                    Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeNameResponseDO>(json);

        //                if (apiResponse != null && apiResponse.Success && apiResponse.EmployeeNameList != null)
        //                {
        //                    ddl_employeename.DataSource = apiResponse.EmployeeNameList;
        //                    ddl_employeename.DataTextField = "Text";
        //                    ddl_employeename.DataValueField = "Id";
        //                    ddl_employeename.DataBind();
        //                    ddl_employeename.Items.Insert(0, new ListItem("-- Please Select --", ""));
        //                }
        //                else
        //                {
        //                    ddl_employeename.Items.Clear();
        //                    ddl_employeename.Items.Insert(0, new ListItem("-- No Users --", ""));
        //                }
        //            }
        //            else
        //            {
        //                ddl_employeename.Items.Clear();
        //                ddl_employeename.Items.Insert(0, new ListItem("-- API Error --", ""));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog(
        //            "AddSalarySlipData",
        //            "BindUsername",
        //            "Exception=" + ex.Message + " | StackTrace=" + ex.StackTrace,
        //            UserId
        //        );

        //        ddl_employeename.Items.Clear();
        //        ddl_employeename.Items.Insert(0, new ListItem("-- Error --", ""));
        //    }
        //}
        public void BindEmpoyeeName()
        {
            try
            {

                int userId = 0;
                if (!string.IsNullOrEmpty(Request.QueryString["user_id"]))
                {
                    int.TryParse(Request.QueryString["user_id"], out userId);
                }
                var users = BindgetAPIUserData(userId);

                ddl_employeename.DataSource = users;
                ddl_employeename.DataTextField = "user_fullname";
                ddl_employeename.DataValueField = "UserId";
                ddl_employeename.DataBind();

                if (userId == 0)
                    ddl_employeename.Items.Insert(0, new ListItem("-- Please Select --", ""));
                else
                {
                    ddl_employeename.SelectedValue = userId.ToString();

                    // Auto-bind Employee Code & Designation
                    BindEmployeeCodeAndDesignation(userId);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalarySlip", "BindEmpoyeeName",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindEmployeeCodeAndDesignation(int userId)
        {
            var empData = BindgetAPIEmployeeCodeAndDesignationData(userId);

            if (empData != null && empData.Count > 0)
            {
                var emp = empData.FirstOrDefault();
               // txtEmployeeId.Text = emp.EmployeeCode;

                ddldesign.Items.Clear();
                ddldesign.Items.Add(new ListItem(emp.designation, emp.designation));
                ddldesign.SelectedValue = emp.designation;
            }
            else
            {
               // txtEmployeeId.Text = "";
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
            }
        }


        protected List<UserDetailsDO> BindgetAPIUserData(int userId)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestObj = new { user_id = userId };

                    var json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("UserList/GetUserData", content).Result;

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
                                //roledescription=u.roledescription
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewuserdocuments", "BindgetAPIUserData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

            return users;
        }
        protected List<userEmployeecodeanddesignDO> BindgetAPIEmployeeCodeAndDesignationData(int userId)
        {
            List<userEmployeecodeanddesignDO> users = new List<userEmployeecodeanddesignDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestObj = new { user_id = userId };

                    var json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("UserList/GetEmployeeCodeAndDesignation", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<userEmployeecodeanddesigresponseDO>(jsonString);

                        if (result.Success && result.EmployeeCodeAndDesignationList != null)
                        {
                            users = result.EmployeeCodeAndDesignationList.Select(u => new userEmployeecodeanddesignDO
                            {
                                EmployeeCode = u.EmployeeCode,
                                UserId = u.UserId,
                                designation = u.designation,
                                company_id = u.company_id,

                            }).ToList();
                            if (users.Count > 0)
                            {
                                // Example: storing first employee's data
                                HttpContext.Current.Session["EmployeeCode"] = users[0].EmployeeCode;
                                HttpContext.Current.Session["CompanyId"] = users[0].company_id;

                                // If you want to store lists of all employee codes and company IDs:
                                HttpContext.Current.Session["EmployeeCodesList"] = users.Select(x => x.EmployeeCode).ToList();
                                HttpContext.Current.Session["CompanyIdsList"] = users.Select(x => x.company_id).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddSalarySlipData", "BindgetAPIEmployeeCodeAndDesignationData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

            return users;
        }

        protected void ddl_employeename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_employeename.SelectedValue))
            {
                int userId = Convert.ToInt32(ddl_employeename.SelectedValue);

                var empData = BindgetAPIEmployeeCodeAndDesignationData(userId);

                if (empData != null && empData.Count > 0)
                {
                    var emp = empData.FirstOrDefault();

                    //// Store both EmployeeCode and CompanyID in Session
                    //Session["EmployeeCode"] = emp.EmployeeCode;
                    //Session["CompanyID"] = emp.company_id;

                    ddldesign.Items.Clear();
                    ddldesign.Items.Add(new ListItem(emp.designation, emp.designation));
                    ddldesign.SelectedValue = emp.designation;
                }
                else
                {
                    ddldesign.Items.Clear();
                    ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
                    //Session["EmployeeCode"] = 0;
                    //Session["CompanyID"] = 0;
                }
            }
            else
            {
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
                //Session["EmployeeCode"] = 0;
                //Session["CompanyID"] = 0;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeCode = Convert.ToInt32(Session["EmployeeCode"] ?? "0");
                int companyId = Convert.ToInt32(Session["CompanyID"] ?? "0");

                if (employeeCode == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Employee not selected or invalid.');", true);
                    return;
                }

                if (companyId == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Company ID not found.');", true);
                    return;
                }

                if (ddlMonth.SelectedIndex == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please select Month.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(ddl_employeename.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please select Employee Name.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(ddldesign.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please select Designation.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtDepartment.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Department.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBranch.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Branch.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtGrade.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Grade.');", true);
                    return;
                }

                // 🔹 Earnings validations
                if (string.IsNullOrWhiteSpace(txtBasic.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Basic Salary.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtHRA.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter HRA.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtSpecialAllowance.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Special Allowance.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtTravelAllowance.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Travel Allowance.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMobileReimb.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Mobile Reimbursement.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBonus.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Bonus.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtIncentive.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Incentive.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOtherEarnings.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Other Earnings.');", true);
                    return;
                }

                // 🔹 Deductions validations
                if (string.IsNullOrWhiteSpace(txtProfessionalTax.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Professional Tax.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOtherDeductions.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Other Deductions.');", true);
                    return;
                }
          

                // 🔹 Attendance validations
                if (string.IsNullOrWhiteSpace(txtDaysPaid.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Days Paid.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtAbsent.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Days Absent.');", true);
                    return;
                }

                decimal basicSalary = string.IsNullOrWhiteSpace(txtBasic.Text) ? 0 : Convert.ToDecimal(txtBasic.Text);
                decimal hra = string.IsNullOrWhiteSpace(txtHRA.Text) ? 0 : Convert.ToDecimal(txtHRA.Text);
                decimal specialAllowance = string.IsNullOrWhiteSpace(txtSpecialAllowance.Text) ? 0 : Convert.ToDecimal(txtSpecialAllowance.Text);
                decimal lta = string.IsNullOrWhiteSpace(txtTravelAllowance.Text) ? 0 : Convert.ToDecimal(txtTravelAllowance.Text);
                decimal mobile = string.IsNullOrWhiteSpace(txtMobileReimb.Text) ? 0 : Convert.ToDecimal(txtMobileReimb.Text);
                decimal bonus = string.IsNullOrWhiteSpace(txtBonus.Text) ? 0 : Convert.ToDecimal(txtBonus.Text);
                decimal incentive = string.IsNullOrWhiteSpace(txtIncentive.Text) ? 0 : Convert.ToDecimal(txtIncentive.Text);
                decimal otherEarnings = string.IsNullOrWhiteSpace(txtOtherEarnings.Text) ? 0 : Convert.ToDecimal(txtOtherEarnings.Text);

                decimal professionalTax = string.IsNullOrWhiteSpace(txtProfessionalTax.Text) ? 0 : Convert.ToDecimal(txtProfessionalTax.Text);
                decimal otherDeductions = string.IsNullOrWhiteSpace(txtOtherDeductions.Text) ? 0 : Convert.ToDecimal(txtOtherDeductions.Text);
                decimal TDS = string.IsNullOrWhiteSpace(txtTDS.Text) ? 0 : Convert.ToDecimal(txtTDS.Text);
                // Calculate total earnings
                decimal totalEarnings = basicSalary + hra + specialAllowance + lta + mobile + bonus + incentive + otherEarnings;

                // Calculate total deductions
                decimal totalDeductions = professionalTax + otherDeductions + TDS;

                // Calculate gross pay (usually sum of earnings without deductions if needed separately)
                decimal grossPay = totalEarnings;

                // Calculate net pay
                decimal netPay = totalDeductions;

                decimal finalnetPay = totalEarnings - totalDeductions;

                // Create SalarySlip object
                BioxiaSalarySlipDO slip = new BioxiaSalarySlipDO
                {
                    Month = ddlMonth.SelectedValue,
                    Username = ddl_employeename.SelectedItem.Text,
                    DesignationName = ddldesign.SelectedItem != null ? ddldesign.SelectedItem.Text : "",
                    Department = txtDepartment.Text,
                    Branch = txtBranch.Text,
                    Grade = txtGrade.Text,
                    //employeecode = string.IsNullOrWhiteSpace(txtEmployeeId.Text) ? 0 : Convert.ToInt32(txtEmployeeId.Text),

                    DaysPaid = string.IsNullOrWhiteSpace(txtDaysPaid.Text) ? 0 : Convert.ToInt32(txtDaysPaid.Text),
                    DaysAbsent = string.IsNullOrWhiteSpace(txtAbsent.Text) ? 0 : Convert.ToInt32(txtAbsent.Text),

                    BasicSalary = basicSalary,
                    HouseRentAllowance = hra,
                    SpecialAllowance = specialAllowance,
                    LeaveTravelAllowance = lta,
                    MobileReimbursement = mobile,
                    Bonus = bonus,
                    Incentive = incentive,
                    Others = otherEarnings,
                    GrossPay = grossPay,

                    ProfessionalTax = professionalTax,
                    taxOthers = otherDeductions,
                    TDS=TDS,

                    TotalEarnings = totalEarnings,
                    TotalDeductions = netPay,
                    finalNetpay = finalnetPay,

                   

                    InsertedBy = Convert.ToInt32(HttpContext.Current.Session["userId"] ?? "0"),
                    employeecode = employeeCode,
                    company_id = companyId,

                };

                SalarySlipBL slipBL = new SalarySlipBL();
                List<BioxiaSalarySlipDO> salarydata = slipBL.SaveImsetSalaryDetails(slip);

                if (salarydata != null && salarydata.Count > 0)
                {
                    string status = salarydata[0].Status;
                    string remark = salarydata[0].Remarks;

                    ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                        $"showsalarySavedMessage('{status}', '{remark}');", true);
                    btnClear_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ErrorMsg",
                    $"alert('Unexpected error: {ex.Message}');", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            // Clear textboxes
            txtPaySlip.Text = "";
            txtBranch.Text = "";
            txtGrade.Text = "";
            txtDepartment.Text = "";
            txtDaysPaid.Text = "";
            txtAbsent.Text = "";
            txtBasic.Text = "";
            txtHRA.Text = "";
            txtTravelAllowance.Text = "";
            txtSpecialAllowance.Text = "";
            txtMobileReimb.Text = "";
            txtBonus.Text = "";
            txtIncentive.Text = "";
            txtOtherEarnings.Text = "";
            txtProfessionalTax.Text = "";
            txtTDS.Text = "";
            txtOtherDeductions.Text = "";
            txtTotalEarnings.Text = "";
            txtGrossPay.Text = "";
            txtNetPay.Text = "";
            txtFinalNetPay.Text = "";


            ddldesign.Items.Clear();
            ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
            ddl_employeename.SelectedIndex = 0;
            ddldesign.SelectedIndex = 0;
        }

    }
}