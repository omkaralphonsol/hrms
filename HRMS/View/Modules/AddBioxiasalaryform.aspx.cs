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
    public partial class AddBioxiasalaryform : System.Web.UI.Page
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
                txtEmployeeId.Text = emp.EmployeeCode;

                ddldesign.Items.Clear();
                ddldesign.Items.Add(new ListItem(emp.designation, emp.designation));
                ddldesign.SelectedValue = emp.designation;
            }
            else
            {
                 txtEmployeeId.Text = "";
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


                    //txtEmployeeId.Text = emp.EmployeeCode;
                    ddldesign.Items.Clear();
                    ddldesign.Items.Add(new ListItem(emp.designation, emp.designation));
                    ddldesign.SelectedValue = emp.designation;

                }
                else
                {
                    //txtEmployeeId.Text = "";
                    ddldesign.Items.Clear();
                    ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
                }
            }
            else
            {
                //txtEmployeeId.Text = "";
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmployeeId.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Employee Code.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(ddl_employeename.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please select Employee Name.');", true);
                    return;
                }
                if (ddlMonth.SelectedIndex == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please select Month.');", true);
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
                if (string.IsNullOrWhiteSpace(txtDivision.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Division.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtESIC.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter ESIC Number.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPFNo.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter PF Number.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtJoiningDate.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Joining Date.');", true);
                    return;
                }

                // 🔹 Earnings fields
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
                if (string.IsNullOrWhiteSpace(txtTravel.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Travel Allowance.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMobile.Text))
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

                // 🔹 Deductions
                if (string.IsNullOrWhiteSpace(txtProfTax.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Professional Tax.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOtherDeduction.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Other Deductions.');", true);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtTaxDeducted.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Tax Deducted.');", true);
                    return;
                }

                // 🔹 Attendance
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
                if (string.IsNullOrWhiteSpace(txtTDS.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter TDS.');", true);
                    return;
                }

                decimal basicSalary = string.IsNullOrWhiteSpace(txtBasic.Text) ? 0 : Convert.ToDecimal(txtBasic.Text);
                decimal hra = string.IsNullOrWhiteSpace(txtHRA.Text) ? 0 : Convert.ToDecimal(txtHRA.Text);
                decimal specialAllowance = string.IsNullOrWhiteSpace(txtSpecialAllowance.Text) ? 0 : Convert.ToDecimal(txtSpecialAllowance.Text);
                decimal lta = string.IsNullOrWhiteSpace(txtTravel.Text) ? 0 : Convert.ToDecimal(txtTravel.Text);
                decimal mobile = string.IsNullOrWhiteSpace(txtMobile.Text) ? 0 : Convert.ToDecimal(txtMobile.Text);
                decimal bonus = string.IsNullOrWhiteSpace(txtBonus.Text) ? 0 : Convert.ToDecimal(txtBonus.Text);
                decimal incentive = string.IsNullOrWhiteSpace(txtIncentive.Text) ? 0 : Convert.ToDecimal(txtIncentive.Text);
                decimal otherEarnings = string.IsNullOrWhiteSpace(txtOtherEarnings.Text) ? 0 : Convert.ToDecimal(txtOtherEarnings.Text);

                decimal professionalTax = string.IsNullOrWhiteSpace(txtProfTax.Text) ? 0 : Convert.ToDecimal(txtProfTax.Text);
                decimal otherDeductions = string.IsNullOrWhiteSpace(txtOtherDeduction.Text) ? 0 : Convert.ToDecimal(txtOtherDeduction.Text);
                decimal professionalTaxDeducted = string.IsNullOrWhiteSpace(txtTaxDeducted.Text) ? 0 : Convert.ToDecimal(txtTaxDeducted.Text);
                decimal TDS = string.IsNullOrWhiteSpace(txtTDS.Text) ? 0 : Convert.ToDecimal(txtTDS.Text);

                // Calculate total earnings
                decimal totalEarnings = basicSalary + hra + specialAllowance + lta + mobile + bonus + incentive + otherEarnings;

                // Calculate total deductions
                decimal totalDeductions = professionalTax + otherDeductions + professionalTaxDeducted;

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
                    employeecode = Convert.ToInt32(txtEmployeeId.Text),
                    Department = txtDepartment.Text,
                    Branch = txtBranch.Text,
                    Grade = txtGrade.Text,
                    Division = txtDivision.Text,
                    ESIC_No = txtESIC.Text,
                    PF_No = txtPFNo.Text,
                    JoiningDate = string.IsNullOrWhiteSpace(txtJoiningDate.Text) ? DateTime.Now : Convert.ToDateTime(txtJoiningDate.Text),

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
                    ProfessionalTaxDeducted = professionalTaxDeducted,
                    TDS = TDS,

                    TotalEarnings = totalEarnings,
                    TotalDeductions = netPay,
                    finalNetpay = finalnetPay,

                    InsertedBy = Convert.ToInt32(HttpContext.Current.Session["userId"] ?? "0")
                };

                SalarySlipBL slipBL = new SalarySlipBL();
                List<BioxiaSalarySlipDO> salarydata = slipBL.SaveBioxiaSalaryDetails(slip);

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
         
            txtpayslip.Text = "";
            ddldesign.Items.Clear();
            ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
            txtBranch.Text = "";
            ddl_employeename.SelectedIndex = 0;
            txtGrade.Text = "";
            txtDepartment.Text = "";
            txtEmployeeId.Text = "";
            ddldesign.SelectedIndex = 0;
            txtDivision.Text = "";
            txtESIC.Text = "";
            txtPFNo.Text = "";
            txtJoiningDate.Text = "";
            txtDaysPaid.Text = "";
            txtAbsent.Text = "";

            // --- Earnings ---
            txtBasic.Text = "";
            txtHRA.Text = "";
            txtTravel.Text = "";
            txtSpecialAllowance.Text = "";
            txtMobile.Text = "";
            txtBonus.Text = "";
            txtIncentive.Text = "";
            txtOtherEarnings.Text = "";

            // --- Deductions ---
            txtProfTax.Text = "";
            txtTDS.Text = "";
            txtOtherDeduction.Text = "";

            // --- Totals ---
            txtTotalEarnings.Text = "";
            txtTotalDeductions.Text = "";
            txtGrossPay.Text = "";
            txtTaxDeducted.Text = "";
            txtNetPay.Text = "";
            txtFinalNetPay.Text = "";
        }

    }
}