using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class Remunerationform : System.Web.UI.Page
    {
        public class ComponentSaveResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public int id { get; set; }
            public string text { get; set; }
            public string componentType { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ComponentSaveResponse SaveNewSalaryComponent(string componentName, string componentType)
        {
            try
            {
                string safeName = Convert.ToString(componentName ?? string.Empty).Trim();
                string safeType = Convert.ToString(componentType ?? string.Empty).Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(safeName))
                {
                    return new ComponentSaveResponse { success = false, message = "Component name is required." };
                }
                if (safeType != "EARNING" && safeType != "DEDUCTION")
                {
                    return new ComponentSaveResponse { success = false, message = "Invalid component type." };
                }

                int insertedBy = 0;
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    int.TryParse(Convert.ToString(HttpContext.Current.Session["userId"] ?? "0"), out insertedBy);
                }

                SalarySlipBL bl = new SalarySlipBL();
                UserDetailsDO result = bl.InsertSalaryComponent(safeName, safeType, insertedBy);
                bool ok = result != null && string.Equals(Convert.ToString(result.Status), "Success", StringComparison.OrdinalIgnoreCase);

                return new ComponentSaveResponse
                {
                    success = ok,
                    message = result != null ? Convert.ToString(result.Remarks) : "Unable to save component.",
                    id = result != null ? result.UserId : 0,
                    text = safeName,
                    componentType = safeType
                };
            }
            catch (Exception ex)
            {
                return new ComponentSaveResponse { success = false, message = ex.Message };
            }
        }

        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            BindComponents();
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

                BindUsername();
                BindYears();
                BindMonths();
                BindEmployeeCode();
                BindSavedComponentsForEdit();


            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (ddl_componentModal.Items.Count <= 1)
            {
                BindComponents();
            }
            base.OnPreRender(e);
        }
        public void BindComponents()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownComponent_Forremuneration();
                if (list1 != null && list1.Count > 0)
                {
                    ddl_component.DataSource = list1;
                    ddl_component.DataTextField = "Text";
                    ddl_component.DataValueField = "Id";

                    ddl_componentModal.DataSource = list1;
                    ddl_componentModal.DataTextField = "Text";
                    ddl_componentModal.DataValueField = "Id";
                }
                else
                {
                    ddl_component.DataSource = null;
                    ddl_componentModal.DataSource = null;
                }
                ddl_component.DataBind();
                ddl_component.Items.Insert(0, new ListItem("-- Please Select --", ""));

                ddl_componentModal.DataBind();
                ddl_componentModal.Items.Insert(0, new ListItem("-- Please Select --", ""));

                var options = (list1 ?? new List<DropDownData>())
                    .Select(comp => new
                    {
                        Id = comp.Id.ToString(),
                        Text = comp.Text,
                        ComponentType = (comp.ComponentType ?? string.Empty).Trim().ToUpper()
                    })
                    .ToList();

                hfComponentOptions.Value = new JavaScriptSerializer().Serialize(options);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Remunerationform", "BindComponents", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
                hfComponentOptions.Value = "[]";
            }
        }

        public void BindEmployeeCode()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> users = userBL.ViewAllUsers()
                    .Where(u => u.UserId > 0 && !string.IsNullOrWhiteSpace(u.EmployeeCode))
                    .GroupBy(u => (u.EmployeeCode ?? string.Empty).Trim().ToUpper() + "|" + (u.user_fullname ?? string.Empty).Trim().ToUpper())
                    .Select(g => g.First())
                    .OrderBy(u => u.EmployeeCode)
                    .ToList();

                ddl_empcode.Items.Clear();
                ddl_empcode.Items.Insert(0, new ListItem("-- Please Select --", ""));

                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        ddl_empcode.Items.Add(new ListItem((user.EmployeeCode ?? string.Empty).Trim(), user.UserId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Remunerationform", "BindEmployeeCode", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        public void BindMonths()
        {
            ddlMonth.Items.Clear();
            ddlMonth.Items.Add(new ListItem("-Select Month-", ""));

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
            ddlYear.Items.Add(new ListItem("-Select Year-", ""));

            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 5; year <= currentYear + 5; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }
        public void BindUsername()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> users = userBL.ViewAllUsers()
                    .Where(u => u.UserId > 0 && !string.IsNullOrWhiteSpace(u.user_fullname))
                    .GroupBy(u => (u.user_fullname ?? string.Empty).Trim().ToUpper() + "|" + (u.EmployeeCode ?? string.Empty).Trim().ToUpper())
                    .Select(g => g.First())
                    .OrderBy(u => u.user_fullname)
                    .ToList();
                ddl_username.Items.Clear();
                ddl_username.Items.Insert(0, new ListItem("-- Please Select --", ""));

                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        ddl_username.Items.Add(new ListItem((user.user_fullname ?? string.Empty).Trim(), user.UserId.ToString()));
                    }
                }
                else
                {
                    ddl_username.Items[0].Text = "-- No Users --";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Remunerationform",
                    "BindUsername",
                    "Exception=" + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );

                ddl_username.Items.Clear();
                ddl_username.Items.Insert(0, new ListItem("-- Error --", ""));
            }
        }
        protected void ddl_employeename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_username.SelectedValue))
            {
                int userId = Convert.ToInt32(ddl_username.SelectedValue);
                BindEmployeeCodeAndDesignation(userId);
                BindEmployeeSalary(userId);
            }
            else
            {
                ddl_username.Text = "";
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
            }
        }

        protected void ddl_employeeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_empcode.SelectedValue))
            {
                int userId = Convert.ToInt32(ddl_empcode.SelectedValue);
                ListItem selectedUser = ddl_username.Items.FindByValue(ddl_empcode.SelectedValue);
                if (selectedUser != null)
                {
                    ddl_username.SelectedValue = ddl_empcode.SelectedValue;
                }
                BindEmployeeCodeAndDesignation(userId);
                BindEmployeeSalary(userId);
            }
        }
        private void BindEmployeeSalary(int userId)
        {
            SalarySlipBL bl = new SalarySlipBL();
            var data = bl.GetSalaryMasterByUserId(userId);
            if (data == null || data.Count == 0)
            {
                UserDetailsBL userBL = new UserDetailsBL();
                string employeeCode = string.Empty;
                var mergedUser = userBL.ViewAllUsers().FirstOrDefault(x => x.UserId == userId);
                if (mergedUser != null)
                {
                    employeeCode = Convert.ToString(mergedUser.EmployeeCode ?? string.Empty).Trim();
                }

                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    data = bl.GetSalaryMasterByEmployeeCode(employeeCode);
                }
            }

            if (data != null && data.Count > 0)
            {
                var master = data[0];

                decimal basic = master.BasicSalary;
                decimal hra = master.HouseRentAllowance;
                decimal special = master.SpecialAllowance;
               // decimal lta = master.LeaveTravelAllowance; // optional
                decimal professionalTax = master.ProfessionalTax;

                txtBasicSalary.Text = basic.ToString("0.00");
                txtHRA.Text = hra.ToString("0.00");
                txtSpecial.Text = special.ToString("0.00");
                txtProfessionalTax.Text = professionalTax.ToString("0.00");

                decimal totalEarnings = basic + hra + special;

                decimal totalDeductions = professionalTax;

                decimal netPay = totalEarnings - totalDeductions;

                txtTotalEarnings.Text = totalEarnings.ToString("0.00");
                txtTotalDeductions.Text = totalDeductions.ToString("0.00");
                txtNetPay.Text = netPay.ToString("0.00");

                ViewState["FullBasic"] = basic;
                ViewState["FullHRA"] = hra;
                ViewState["FullSpecial"] = special;
                //ViewState["FullLTA"] = lta;
                ViewState["FullPT"] = professionalTax;
            }
            else
            {
                ClearSalaryFields();
            }
        }
        private void ClearSalaryFields()
        {
            txtBasicSalary.Text = "0.00";
            txtHRA.Text = "0.00";
            txtSpecial.Text = "0.00";
            txtProfessionalTax.Text = "0.00";

            txtTotalEarnings.Text = "0.00";
            txtTotalDeductions.Text = "0.00";
            txtNetPay.Text = "0.00";
        }

        

        protected void btnshowApprisal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddl_username.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "UserValidation",
                    "showsalarySavedMessage('error', 'Please select a Employee Name');",
                    true
                );
                return;
            }
            int userId = Convert.ToInt32(ddl_username.SelectedValue);
            SalarySlipBL bl = new SalarySlipBL();

            DateTime? lastDate = bl.GetLastAppraisalDate(userId);

            if (lastDate.HasValue)
            {
                txtLastAppraisalDate.Text = lastDate.Value.ToString("dd-MM-yyyy");
            }

            pnlAppraisal.Visible = true;
            btnshowApprisal.Visible = false;
        }

        private void BindSavedComponentsForEdit()
        {
            try
            {
                string mode = Convert.ToString(Request.QueryString["mode"]);
                if (!string.Equals(mode, "edit", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                int userId;
                int month;
                int year;
                if (!int.TryParse(Convert.ToString(Request.QueryString["user_id"]), out userId)
                    || !int.TryParse(Convert.ToString(Request.QueryString["month"]), out month)
                    || !int.TryParse(Convert.ToString(Request.QueryString["year"]), out year))
                {
                    return;
                }

                SalarySlipBL bl = new SalarySlipBL();
                var components = bl.GetRemunerationComponents(userId, month, year);
                if (components == null || components.Count == 0)
                {
                    return;
                }

                hfDynamicComponents.Value = new JavaScriptSerializer().Serialize(components);

                string js = @"
                    (function(){
                        var data = " + hfDynamicComponents.Value + @";
                        data.forEach(function(c){
                            if (typeof createDynamicRow === 'function') {
                                createDynamicRow(c.ComponentName, parseFloat(c.Amount || 0), !!c.IsDeduction);
                            }
                        });
                        if (typeof calculateSalary === 'function') {
                            calculateSalary();
                        }
                    })();";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadSavedComponents", js, true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Remunerationform", "BindSavedComponentsForEdit", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
    

        private void BindEmployeeCodeAndDesignation(int userId)
        {
            UserDetailsBL userBL = new UserDetailsBL();
            var userData = userBL.ViewAllUsers().Where(x => x.UserId == userId).ToList();
            if (userData == null || userData.Count == 0)
            {
                userData = userBL.GetUserDetails(userId);
            }
            if (userData != null && userData.Count > 0)
            {
                var emp = userData.FirstOrDefault();
                ddl_empcode.Items.Clear();
                ddl_empcode.Items.Add(new ListItem(emp.EmployeeCode, userId.ToString()));
                ddl_empcode.SelectedValue = userId.ToString();

                ddldesign.Items.Clear();
                string designation = string.IsNullOrWhiteSpace(emp.designation_name) ? "-- Not Found --" : emp.designation_name;
                ddldesign.Items.Add(new ListItem(designation, designation));
                ddldesign.SelectedValue = designation;

                HttpContext.Current.Session["EmployeeCode"] = emp.EmployeeCode;
                HttpContext.Current.Session["CompanyId"] = emp.company_id;
            }
            else
            {
                ddl_empcode.Items.Clear();
                ddl_empcode.Items.Insert(0, new ListItem("-- Not Found --", ""));
                ddldesign.Items.Clear();
                ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
            }
        }

     
        protected void btnShowAdditional_Click(object sender, EventArgs e)
        {
            pnlAdditionalComponents.Visible = true;
        }

        protected void btnCloseAdditional_Click(object sender, EventArgs e)
        {
            pnlAdditionalComponents.Visible = false;
        }

        protected void btnCloseAppraisal_Click(object sender, EventArgs e)
        {
            pnlAppraisal.Visible = false;
            btnshowApprisal.Visible = true;
        }
        protected void btnAddSalaryComponent_Click(object sender, EventArgs e)
        {
            pnlAdditionalComponents.Visible = true; 

            List<SalaryComponent> list =
                ViewState["Components"] as List<SalaryComponent>
                ?? new List<SalaryComponent>();

            if (string.IsNullOrEmpty(ddl_component.SelectedValue))
                return;

            string componentName = ddl_component.SelectedItem.Text;

            if (list.Any(x => x.ComponentName == componentName))
                return;

            bool isDeduction =
                componentName == "Provident Fund"
                || componentName == "Other Deduction";

            list.Add(new SalaryComponent
            {
                ComponentName = componentName,
                Amount = 0,
                IsDeduction = isDeduction
            });

            ViewState["Components"] = list;

            BindComponentRepeater();
            ScriptManager.RegisterStartupScript(
       this,
       this.GetType(),
       "recalcSalary",
       "calculateSalary();",
       true
   );
        }

        private void BindComponentRepeater()
        {
            var list = ViewState["Components"] as List<SalaryComponent>
                       ?? new List<SalaryComponent>();

            rptComponents.DataSource = list;
            rptComponents.DataBind();

        }

        protected void RemoveComponent(object sender, CommandEventArgs e)
        {
            pnlAdditionalComponents.Visible = true; 

            var list = ViewState["Components"] as List<SalaryComponent>;
            if (list == null) return;

            string componentName = e.CommandArgument.ToString();

            list.RemoveAll(x => x.ComponentName == componentName);

            ViewState["Components"] = list;

            BindComponentRepeater();
            ddl_component.ClearSelection();
            ddl_component.SelectedIndex = 0;
            ScriptManager.RegisterStartupScript(
       this,
       this.GetType(),
       "recalcSalary",
       "calculateSalary();",
       true
   );
        }
        protected void btn_saveremuneration_click(object sender, EventArgs e)
        {
            try
            {
                SalarySlipDO slip = new SalarySlipDO();

                if (string.IsNullOrWhiteSpace(ddl_username.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "UserValidation",
                        "showsalarySavedMessage('error', 'Please select employee.');", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlMonth.SelectedValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MonthValidation",
                        "showsalarySavedMessage('error', 'Please select month.');", true);
                    return;
                }

                int yearValue;
                if (!int.TryParse(ddlYear.SelectedValue, out yearValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "YearValidation",
                        "showsalarySavedMessage('error', 'Please select year.');", true);
                    return;
                }

                slip.Month = ddlMonth.SelectedValue;
                slip.Year = yearValue;
                int userIdValue;
                if (!int.TryParse(ddl_username.SelectedValue, out userIdValue))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "UserIdValidation",
                        "showsalarySavedMessage('error', 'Invalid employee selection.');", true);
                    return;
                }
                slip.UserId = userIdValue;

                // Always derive name/code/company/designation from latest user master data
                // to avoid invalid session fallbacks for newly created users.
                UserDetailsBL userBL = new UserDetailsBL();
                var userData = userBL.GetUserDetails(userIdValue);
                var selectedUser = (userData != null && userData.Count > 0) ? userData[0] : null;
                if (selectedUser == null)
                {
                    selectedUser = userBL.ViewAllUsers().FirstOrDefault(x => x.UserId == userIdValue);
                }

                if (selectedUser == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "UserMasterValidation",
                        "showsalarySavedMessage('error', 'Employee master data not found. Please reselect employee.');", true);
                    return;
                }

                slip.Username = string.IsNullOrWhiteSpace(selectedUser.user_fullname) ? ddl_username.SelectedItem.Text : selectedUser.user_fullname;
                slip.DesignationName = string.IsNullOrWhiteSpace(selectedUser.designation_name) ? ddldesign.SelectedItem.Text : selectedUser.designation_name;

                int employeeCodeValue;
                if (!int.TryParse(selectedUser.EmployeeCode, out employeeCodeValue) || employeeCodeValue <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "EmpCodeValidation",
                        "showsalarySavedMessage('error', 'Employee Code is missing for this user. Please update user details first.');", true);
                    return;
                }
                slip.employeecode = employeeCodeValue;

                if (selectedUser.company_id <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CompanyValidation",
                        "showsalarySavedMessage('error', 'Company is missing for this user. Please update user details first.');", true);
                    return;
                }
                slip.company_id = selectedUser.company_id;

                decimal basicSalary;
                if (!decimal.TryParse(txtBasicSalary.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out basicSalary) &&
                    !decimal.TryParse(txtBasicSalary.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out basicSalary))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "BasicValidation",
                        "showsalarySavedMessage('error', 'Invalid Basic Salary format.');", true);
                    return;
                }

                decimal hra;
                if (!decimal.TryParse(txtHRA.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out hra) &&
                    !decimal.TryParse(txtHRA.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out hra))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "HraValidation",
                        "showsalarySavedMessage('error', 'Invalid HRA format.');", true);
                    return;
                }

                decimal special;
                if (!decimal.TryParse(txtSpecial.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out special) &&
                    !decimal.TryParse(txtSpecial.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out special))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SpecialValidation",
                        "showsalarySavedMessage('error', 'Invalid Special Allowance format.');", true);
                    return;
                }

                decimal professionalTax;
                if (!decimal.TryParse(txtProfessionalTax.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out professionalTax) &&
                    !decimal.TryParse(txtProfessionalTax.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out professionalTax))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "TaxValidation",
                        "showsalarySavedMessage('error', 'Invalid Professional Tax format.');", true);
                    return;
                }

                slip.BasicSalary = basicSalary;
                slip.HouseRentAllowance = hra;
                slip.SpecialAllowance = special;
                slip.ProfessionalTax = professionalTax;
                DateTime parsedDate;
                if (string.IsNullOrWhiteSpace(txtLastAppraisalDate.Text))
                {
                    slip.LastAppraisalDate = null;
                }
                else if (DateTime.TryParseExact(txtLastAppraisalDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    slip.LastAppraisalDate = parsedDate;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "DateValidation1",
                        "showsalarySavedMessage('error', 'Please enter Last Appraisal Date in dd-MM-yyyy format.');",
                        true
                    );
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCurrentAppraisalDate.Text))
                {
                    slip.CurrentAppraisalDate = null;
                }
                else if (DateTime.TryParseExact(txtCurrentAppraisalDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    slip.CurrentAppraisalDate = parsedDate;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "DateValidation2",
                        "showsalarySavedMessage('error', 'Please enter Current Appraisal Date in dd-MM-yyyy format.');",
                        true
                    );
                    return;
                }


                decimal appraisalPercentage;
                if (!decimal.TryParse(string.IsNullOrWhiteSpace(txtAppraisalPercent.Text) ? "0" : txtAppraisalPercent.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out appraisalPercentage) &&
                    !decimal.TryParse(string.IsNullOrWhiteSpace(txtAppraisalPercent.Text) ? "0" : txtAppraisalPercent.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out appraisalPercentage))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AppraisalValidation",
                        "showsalarySavedMessage('error', 'Invalid appraisal percentage format.');", true);
                    return;
                }
                slip.AppraisalPercentage = appraisalPercentage;

                decimal incrementAmount;
                if (!decimal.TryParse(string.IsNullOrWhiteSpace(hfIncrementAmount.Value) ? "0" : hfIncrementAmount.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out incrementAmount) &&
                    !decimal.TryParse(string.IsNullOrWhiteSpace(hfIncrementAmount.Value) ? "0" : hfIncrementAmount.Value, NumberStyles.Any, CultureInfo.CurrentCulture, out incrementAmount))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "IncrementValidation",
                        "showsalarySavedMessage('error', 'Invalid increment amount format.');", true);
                    return;
                }
                slip.IncrementAmount = incrementAmount;

                List<SalaryComponent> components = GetComponentsFromRepeater();


                decimal bonus = 0;
                decimal incentive = 0;
                decimal others = 0;
                decimal lta = 0;
                decimal dynamicEarnings = 0;
                decimal dynamicDeductions = 0;

                foreach (var c in components)
                {
                    if (c == null || string.IsNullOrWhiteSpace(c.ComponentName))
                    {
                        continue;
                    }

                    if (c.IsDeduction)
                    {
                        dynamicDeductions += c.Amount;
                    }
                    else
                    {
                        dynamicEarnings += c.Amount;
                    }

                    // Keep legacy fields populated for existing SP/table columns.
                    switch ((c.ComponentName ?? string.Empty).Trim().ToUpperInvariant())
                    {
                        case "BONUS":
                            bonus += c.Amount;
                            break;
                        case "INCENTIVE":
                            incentive += c.Amount;
                            break;
                        case "LEAVE TRAVEL ALLOWANCE":
                            lta += c.Amount;
                            break;
                        case "OTHER DEDUCTION":
                            others += c.Amount;
                            break;
                    }
                }

                slip.TotalEarnings = slip.BasicSalary + slip.HouseRentAllowance + slip.SpecialAllowance + dynamicEarnings;
                slip.TotalDeductions = slip.ProfessionalTax + dynamicDeductions;
                slip.NetPay = slip.TotalEarnings - slip.TotalDeductions;


                SalarySlipBL bl = new SalarySlipBL();
                var result = bl.SaveRenumerationSalaryDetails( slip, lta, bonus, incentive, others );


                if (result != null && result.Count > 0)
                {
                    string status = Convert.ToString(result[0].Status ?? string.Empty).Trim();
                    string remark = Convert.ToString(result[0].Remarks ?? string.Empty).Trim();

                    if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        int insertedBy = 0;
                        int.TryParse(Convert.ToString(Session["userId"]), out insertedBy);

                        int monthValue = 0;
                        int.TryParse(slip.Month, out monthValue);

                        if (monthValue > 0 && slip.Year > 0)
                        {
                            bl.SaveRemunerationComponents(slip.UserId, monthValue, slip.Year, components, insertedBy);
                        }
                    }

                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "UserSavedScript",
                        $"showsalarySavedMessage('{status}', '{remark}');",
                        true
                    );
                }

            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ErrorMsg",
                      "alert('Unexpected error: " + ex.Message + "');", true);
            }
        }

        private List<SalaryComponent> GetComponentsFromRepeater()
        {
            var list = new List<SalaryComponent>();

            // 1) Read dynamic components sent from client-side modal rows.
            if (!string.IsNullOrWhiteSpace(hfDynamicComponents.Value))
            {
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var dynamicList = js.Deserialize<List<SalaryComponent>>(hfDynamicComponents.Value);
                    if (dynamicList != null)
                    {
                        list.AddRange(dynamicList);
                    }
                }
                catch
                {
                    // fallback to legacy repeater parsing below
                }
            }

            // 2) Merge legacy repeater components (if any).
            var viewStateList = ViewState["Components"] as List<SalaryComponent> ?? new List<SalaryComponent>();
            foreach (var item in viewStateList)
            {
                if (!list.Any(x => string.Equals(x.ComponentName, item.ComponentName, StringComparison.OrdinalIgnoreCase)))
                {
                    list.Add(item);
                }
            }

            foreach (RepeaterItem item in rptComponents.Items)
            {
                //  Find amount textbox
                TextBox txtAmount = item.FindControl("txtAmount") as TextBox;
                if (txtAmount == null) continue;

                decimal amount = 0;
                decimal.TryParse(txtAmount.Text, out amount);

                //  Get component name from ViewState list using index
                int index = item.ItemIndex;

                if (index >= 0 && index < list.Count)
                {
                    list[index].Amount = amount;
                }
            }

            ViewState["Components"] = list;
            return list;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            //Dropdowns
            ddl_username.ClearSelection();
            ddl_username.SelectedIndex = 0;

            ddldesign.Items.Clear();
            ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));

            ddlMonth.ClearSelection();
            ddlMonth.SelectedIndex = 0;

            ddlYear.ClearSelection();
            ddlYear.SelectedIndex = 0;

            // Salary Fields
            txtBasicSalary.Text = "0.00";
            txtHRA.Text = "0.00";
            txtSpecial.Text = "0.00";
            txtProfessionalTax.Text = "0.00";

            txtTotalEarnings.Text = "0.00";
            txtTotalDeductions.Text = "0.00";
            txtNetPay.Text = "0.00";

            //Appraisal
            txtLastAppraisalDate.Text = "";
            txtCurrentAppraisalDate.Text = "";
            txtAppraisalPercent.Text = "";
            txtIncrementAmount.Text = "";
            hfIncrementAmount.Value = "";

            pnlAppraisal.Visible = false;
            btnshowApprisal.Visible = true;

            ViewState["Components"] = null;
            rptComponents.DataSource = null;
            rptComponents.DataBind();


            ddl_component.ClearSelection();
            ddl_component.SelectedIndex = 0;

            Session["EmployeeCode"] = null;
            Session["CompanyId"] = null;

            upAdditionalComponents.Update();

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "resetMsg",
                "showsalarySavedMessage('info','Form reset successfully');",
                true
            );
        }

    }
}
