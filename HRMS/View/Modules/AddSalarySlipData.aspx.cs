using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    public partial class AddSalarySlipData : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            if (!IsPostBack)
            {
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

                BindDesignation();
                BindEmpoyeeName();
                BindMonths();
                BindEmployeeSalary();
                BindExistingSavedSlip();
            }
        }

        private void BindMonths()
        {
            ddlMonth.Items.Clear();

            for (int i = 1; i <= 12; i++)
            {
                ddlMonth.Items.Add(new ListItem(
                    new DateTime(DateTime.Now.Year, i, 1).ToString("MMMM"),
                    i.ToString()
                ));
            }

            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
        }
        protected void txtAbsent_TextChanged(object sender, EventArgs e)
        {
            RecalculateDays();
            CalculateSalary();
        }

        private void BindEmployeeSalary()
        {
            int userId = Convert.ToInt32(Request.QueryString["user_id"]);
            SalarySlipBL bl = new SalarySlipBL();
            UserDetailsBL userBL = new UserDetailsBL();
            string employeeCode = string.Empty;
            var mergedUser = userBL.ViewAllUsers().FirstOrDefault(x => x.UserId == userId);
            if (mergedUser != null)
            {
                employeeCode = Convert.ToString(mergedUser.EmployeeCode ?? string.Empty).Trim();
            }

            var data = new List<SalarySlipDO>();
            if (!string.IsNullOrWhiteSpace(employeeCode))
            {
                // Strict rule: load salary master by employee code only.
                data = bl.GetSalaryMasterByEmployeeCode(employeeCode);
            }

            if (data != null && data.Count > 0)
            {
                var master = data[0];

                txtDaysPaid.Text = "30";

                txtBasicSalary.Text = master.BasicSalary.ToString("0.00");
                txtHRA.Text = master.HouseRentAllowance.ToString("0.00");
                txtSpecialAllowance.Text = master.SpecialAllowance.ToString("0.00");
                txtLTA.Text = master.LeaveTravelAllowance.ToString("0.00");
                txtProfessionalTax.Text = master.ProfessionalTax.ToString("0.00");

                txtTotalEarnings.Text = master.TotalEarnings.ToString("0.00");
                txtTotalDeductions.Text = master.TotalDeductions.ToString("0.00");
                txtNetPay.Text = master.NetPay.ToString("0.00");



                // 🔥 STORE ORIGINAL VALUES (VERY IMPORTANT)
                ViewState["FullBasic"] = master.BasicSalary;
                ViewState["FullHRA"] = master.HouseRentAllowance;
                ViewState["FullSpecial"] = master.SpecialAllowance;
                ViewState["FullLTA"] = master.LeaveTravelAllowance;
                ViewState["FullPT"] = master.ProfessionalTax;
            }
            else
            {
                // If employee code not found in employee_salary_master, show zeros.
                txtDaysPaid.Text = "30";
                txtBasicSalary.Text = "0.00";
                txtHRA.Text = "0.00";
                txtSpecialAllowance.Text = "0.00";
                txtLTA.Text = "0.00";
                txtProfessionalTax.Text = "0.00";
                txtTotalEarnings.Text = "0.00";
                txtTotalDeductions.Text = "0.00";
                txtNetPay.Text = "0.00";

                ViewState["FullBasic"] = 0m;
                ViewState["FullHRA"] = 0m;
                ViewState["FullSpecial"] = 0m;
                ViewState["FullLTA"] = 0m;
                ViewState["FullPT"] = 0m;
            }
        }

        private void BindExistingSavedSlip()
        {
            try
            {
                string mode = Convert.ToString(Request.QueryString["mode"]);
                if (!string.Equals(mode, "edit", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                int userId;
                if (!int.TryParse(Convert.ToString(Request.QueryString["user_id"]), out userId) || userId <= 0)
                {
                    return;
                }

                SalarySlipBL bl = new SalarySlipBL();
                SalarySlipDO savedSlip = bl.GetLatestSavedSlipByUserId(userId);
                UserDetailsBL userBL = new UserDetailsBL();
                var userData = userBL.GetUserDetails(userId);
                var userInfo = (userData != null && userData.Count > 0) ? userData[0] : null;

                if (userInfo != null)
                {
                    ListItem userItem = ddl_employeename.Items.FindByValue(userId.ToString());
                    if (userItem != null)
                    {
                        ddl_employeename.SelectedValue = userId.ToString();
                    }
                    else
                    {
                        ddl_employeename.Items.Clear();
                        ddl_employeename.Items.Add(new ListItem(userInfo.user_fullname, userId.ToString()));
                        ddl_employeename.SelectedValue = userId.ToString();
                    }

                    string designationName = userInfo.designation_name;
                    if (!string.IsNullOrWhiteSpace(designationName))
                    {
                        ListItem designationItem = ddldesign.Items.FindByText(designationName);
                        if (designationItem != null)
                        {
                            ddldesign.SelectedValue = designationItem.Value;
                        }
                        else
                        {
                            ddldesign.Items.Clear();
                            ddldesign.Items.Add(new ListItem(designationName, designationName));
                            ddldesign.SelectedValue = designationName;
                        }
                    }
                }

                if (savedSlip == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(savedSlip.Month))
                {
                    ListItem monthItem = ddlMonth.Items.FindByValue(savedSlip.Month);
                    if (monthItem != null)
                    {
                        ddlMonth.SelectedValue = savedSlip.Month;
                    }
                }

                txtEmployeeId.Text = savedSlip.employeecode.ToString();
                txtDaysPaid.Text = savedSlip.DaysPaid.ToString();
                txtDaysPresent.Text = savedSlip.DaysPresent.ToString();
                txtAbsent.Text = savedSlip.DaysAbsent.ToString();

                txtBasicSalary.Text = savedSlip.BasicSalary.ToString("0.00");
                txtHRA.Text = savedSlip.HouseRentAllowance.ToString("0.00");
                txtSpecialAllowance.Text = savedSlip.SpecialAllowance.ToString("0.00");
                txtLTA.Text = savedSlip.LeaveTravelAllowance.ToString("0.00");
                txtProfessionalTax.Text = savedSlip.ProfessionalTax.ToString("0.00");
                txtTotalEarnings.Text = savedSlip.TotalEarnings.ToString("0.00");
                txtTotalDeductions.Text = savedSlip.TotalDeductions.ToString("0.00");
                txtNetPay.Text = savedSlip.NetPay.ToString("0.00");

                // Keep prorating logic consistent with loaded values.
                ViewState["FullBasic"] = savedSlip.BasicSalary;
                ViewState["FullHRA"] = savedSlip.HouseRentAllowance;
                ViewState["FullSpecial"] = savedSlip.SpecialAllowance;
                ViewState["FullLTA"] = savedSlip.LeaveTravelAllowance;
                ViewState["FullPT"] = savedSlip.ProfessionalTax;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddSalarySlipData", "BindExistingSavedSlip",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void CalculateSalary()
        {
            if (ViewState["FullBasic"] == null)
                return;

            int daysPaid = Convert.ToInt32(txtDaysPaid.Text);

            // original monthly values
            decimal fullBasic = (decimal)ViewState["FullBasic"];
            decimal fullHRA = (decimal)ViewState["FullHRA"];
            decimal fullSpecial = (decimal)ViewState["FullSpecial"];
            decimal fullLTA = (decimal)ViewState["FullLTA"];
            decimal fullPT = (decimal)ViewState["FullPT"];

            // calculate prorated values
            decimal basicSalary = (fullBasic / 30) * daysPaid;
            decimal hra = (fullHRA / 30) * daysPaid;
            decimal special = (fullSpecial / 30) * daysPaid;
            decimal lta = (fullLTA / 30) * daysPaid;

            // set UI
            txtBasicSalary.Text = basicSalary.ToString("0.00");
            txtHRA.Text = hra.ToString("0.00");
            txtSpecialAllowance.Text = special.ToString("0.00");
            txtLTA.Text = lta.ToString("0.00");

            decimal totalEarnings = basicSalary + hra + special + lta;
            txtTotalEarnings.Text = totalEarnings.ToString("0.00");

            // deductions
            decimal professionalTax = fullPT; // use DB PT value
            txtProfessionalTax.Text = professionalTax.ToString("0.00");

            decimal totalDeductions = professionalTax;
            txtTotalDeductions.Text = totalDeductions.ToString("0.00");

            decimal netPay = totalEarnings - totalDeductions;
            txtNetPay.Text = netPay.ToString("0.00");
        }

        private void RecalculateDays()
        {
            int totalWorkingDays = 30;

            int absentDays = string.IsNullOrEmpty(txtAbsent.Text)
                                ? 0
                                : Convert.ToInt32(txtAbsent.Text);

            int daysPresent = totalWorkingDays - absentDays;

            txtDaysPresent.Text = daysPresent.ToString();
            txtDaysPaid.Text = daysPresent.ToString();
        }

        public void BindDesignation()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();

                    var content = new StringContent("{}", Encoding.UTF8, "application/json");
                    var response = client.PostAsync("UserList/BindDesignation", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        DesignationResponseDO apiResponse =
                            Newtonsoft.Json.JsonConvert.DeserializeObject<DesignationResponseDO>(json);

                        if (apiResponse != null && apiResponse.Success && apiResponse.DesignationList != null)
                        {
                            ddldesign.DataSource = apiResponse.DesignationList;
                            ddldesign.DataTextField = "Text";
                            ddldesign.DataValueField = "Id";
                            ddldesign.DataBind();
                            ddldesign.Items.Insert(0, new ListItem("-- Please Select --", ""));
                        }
                        else
                        {
                            ddldesign.Items.Clear();
                            ddldesign.Items.Insert(0, new ListItem("-- No Users --", ""));
                        }
                    }
                    else
                    {
                        ddldesign.Items.Clear();
                        ddldesign.Items.Insert(0, new ListItem("-- API Error --", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AddSalarySlipData",
                    "BindDesignation",
                    "Exception=" + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );

                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Error --", ""));
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

                UserDetailsBL userBL = new UserDetailsBL();
                var users = userBL.ViewAllUsers()
                    .Where(u => u.UserId > 0)
                    .GroupBy(u => u.UserId)
                    .Select(g => g.First())
                    .OrderBy(u => u.user_fullname)
                    .ToList();

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
            UserDetailsBL userBL = new UserDetailsBL();
            var users = userBL.ViewAllUsers().Where(x => x.UserId == userId).ToList();
            UserDetailsDO emp = users.FirstOrDefault();

            if (emp == null || string.IsNullOrWhiteSpace(emp.EmployeeCode) || string.IsNullOrWhiteSpace(emp.designation_name))
            {
                string selectedEmpCode = users.FirstOrDefault() != null ? users.FirstOrDefault().EmployeeCode : string.Empty;
                var secondary = userBL.GetUserDetailsFromSecondary(userId, selectedEmpCode);
                if (secondary != null && secondary.Count > 0)
                {
                    emp = secondary.FirstOrDefault();
                }
                else
                {
                    var primary = userBL.GetUserDetails(userId);
                    if (primary != null && primary.Count > 0)
                    {
                        emp = primary.FirstOrDefault();
                    }
                }
            }

            if (emp != null)
            {
                txtEmployeeId.Text = emp.EmployeeCode;
                ddldesign.Items.Clear();
                string designation = string.IsNullOrWhiteSpace(emp.designation_name) ? "-- Not Found --" : emp.designation_name;
                ddldesign.Items.Add(new ListItem(designation, designation));
                ddldesign.SelectedValue = designation;
                HttpContext.Current.Session["CompanyId"] = emp.company_id > 0 ? emp.company_id : emp.CompanyId;
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


        protected void btnSave_Click(object sender, EventArgs e)
        {
         
           // List<SalarySlipDO> salarydata = null;

            try
            {

                int companyId = 0;
                int.TryParse(Convert.ToString(Session["CompanyID"] ?? Session["CompanyId"] ?? "0"), out companyId);
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


                if (string.IsNullOrWhiteSpace(txtEmployeeId.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Employee ID.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDaysPaid.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Days Paid.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDaysPresent.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Days Present.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtAbsent.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Days Absent.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBasicSalary.Text))
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

                if (string.IsNullOrWhiteSpace(txtLTA.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter LTA.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtProfessionalTax.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Validation",
                        "showsalarySavedMessage('Error','Please enter Professional Tax.');", true);
                    return;
                }

                SalarySlipDO slip = new SalarySlipDO();
                slip.UserId = string.IsNullOrEmpty(ddl_employeename.SelectedValue) ? 0 : Convert.ToInt32(ddl_employeename.SelectedValue);
                slip.Month = ddlMonth.SelectedValue;
                slip.Year = DateTime.Now.Year;
                slip.Username = ddl_employeename.SelectedItem != null ? ddl_employeename.SelectedItem.Text : "";
                slip.DesignationName = ddldesign.SelectedItem != null ? ddldesign.SelectedItem.Text : "";
                slip.employeecode = string.IsNullOrEmpty(txtEmployeeId.Text) ? 0 : Convert.ToInt32(txtEmployeeId.Text);

                slip.DaysPaid = string.IsNullOrEmpty(txtDaysPaid.Text) ? 0 : Convert.ToInt32(txtDaysPaid.Text);
                slip.DaysPresent = string.IsNullOrEmpty(txtDaysPresent.Text) ? 0 : Convert.ToInt32(txtDaysPresent.Text);
                slip.DaysAbsent = string.IsNullOrEmpty(txtAbsent.Text) ? 0 : Convert.ToInt32(txtAbsent.Text);

                // Earnings
                slip.BasicSalary = string.IsNullOrEmpty(txtBasicSalary.Text) ? 0 : Convert.ToDecimal(txtBasicSalary.Text);
                slip.HouseRentAllowance = string.IsNullOrEmpty(txtHRA.Text) ? 0 : Convert.ToDecimal(txtHRA.Text);
                slip.SpecialAllowance = string.IsNullOrEmpty(txtSpecialAllowance.Text) ? 0 : Convert.ToDecimal(txtSpecialAllowance.Text);
                slip.LeaveTravelAllowance = string.IsNullOrEmpty(txtLTA.Text) ? 0 : Convert.ToDecimal(txtLTA.Text);

                // Deductions
                slip.ProfessionalTax = string.IsNullOrEmpty(txtProfessionalTax.Text) ? 0 : Convert.ToDecimal(txtProfessionalTax.Text);

                // Calculations
                slip.TotalEarnings = slip.BasicSalary + slip.HouseRentAllowance + slip.SpecialAllowance + slip.LeaveTravelAllowance;
                slip.TotalDeductions = slip.ProfessionalTax;
                slip.NetPay = slip.TotalEarnings - slip.TotalDeductions;
                slip.company_id = companyId;


                SalarySlipBL slipBL = new SalarySlipBL();
                List<SalarySlipDO> salarydata = slipBL.SaveSalaryDetails(slip);

                if (salarydata != null && salarydata.Count > 0)
                {
                    string status = salarydata[0].Status;
                    string remark = salarydata[0].Remarks;

                    if (status == "Success")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                 "showsalarySavedMessage('" + status + "', '" + remark + "');", true);

                        btnClear_Click(sender, e);
                    }
                    else
                    {

                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showsalarySavedMessage('" + status + "', '" + remark + "');", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ErrorMsg",
                      "alert('Unexpected error: " + ex.Message + "');", true);
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {

            txtBasicSalary.Text = string.Empty;
            txtHRA.Text = string.Empty;
            txtSpecialAllowance.Text = string.Empty;
            txtLTA.Text = string.Empty;
            txtProfessionalTax.Text = string.Empty;

            txtTotalEarnings.Text = string.Empty;
            txtTotalDeductions.Text = string.Empty;
            txtNetPay.Text = string.Empty;

            txtDaysPaid.Text = string.Empty;
            txtDaysPresent.Text = string.Empty;
            txtAbsent.Text = string.Empty;
            txtEmployeeId.Text = string.Empty;

            ddldesign.Items.Clear();
            ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));

            ddlMonth.SelectedIndex = 0;
            ddl_employeename.Items.Clear();
            ddl_employeename.Items.Insert(0, new ListItem("-- Select --", ""));


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
                                designation=u.designation,
                                company_id=u.company_id,
                                
                            }).ToList();
                            if (users.Count > 0)
                            {
                                // Example: storing first employee's data
                               // HttpContext.Current.Session["EmployeeCode"] = users[0].EmployeeCode;
                                HttpContext.Current.Session["CompanyId"] = users[0].company_id;

                                // If you want to store lists of all employee codes and company IDs:
                                //HttpContext.Current.Session["EmployeeCodesList"] = users.Select(x => x.EmployeeCode).ToList();
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

        //protected void ddl_employeename_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(ddl_employeename.SelectedValue))
        //    {
        //        int userId = Convert.ToInt32(ddl_employeename.SelectedValue);


        //        var empData = BindgetAPIEmployeeCodeAndDesignationData(userId);

        //        if (empData != null && empData.Count > 0)
        //        {
        //            var emp = empData.FirstOrDefault();


        //            txtEmployeeId.Text = emp.EmployeeCode;
        //            ddldesign.Items.Clear();
        //            ddldesign.Items.Add(new ListItem(emp.designation, emp.designation));
        //            ddldesign.SelectedValue = emp.designation;

        //        }
        //        else
        //        {
        //            txtEmployeeId.Text = "";
        //            ddldesign.Items.Clear();
        //            ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
        //        }
        //    }
        //    else
        //    {
        //        txtEmployeeId.Text = "";
        //        ddldesign.Items.Clear();
        //        ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
        //    }
        //}

        protected void ddl_employeename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_employeename.SelectedValue))
            {
                int userId = Convert.ToInt32(ddl_employeename.SelectedValue);
                BindEmployeeCodeAndDesignation(userId);
            }
            else
            {
                txtEmployeeId.Text = "";
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/SalarySlip.aspx", false);
        }


    }
}
