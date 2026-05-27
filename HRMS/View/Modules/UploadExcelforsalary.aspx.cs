using DataObject;
using ExcelDataReader;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class UploadExcelforsalary : System.Web.UI.Page
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


            }
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fileUpload.HasFile)
            {
                lblMessage.Text = "Please select an Excel file.";
                return;
            }

            string fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
            if (fileExt != ".xlsx" && fileExt != ".xls")
            {
                lblMessage.Text = "Only Excel File Allowed (.xlsx, .xls)";
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = fileUpload.FileContent)
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                DataTable dt = result.Tables[0]; // First Sheet

                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] == null || row[0].ToString().Trim() == "")
                        continue;

                    int user_id = 0;
                    if (row[0] != null)
                    {
                        int.TryParse(row[0].ToString().Trim(), out user_id);
                    }

                    string employeeName = row[1]?.ToString().Trim();

                    int absentDays = 0;
                    if (row[2] != null)
                    {
                        int.TryParse(row[2].ToString().Trim(), out absentDays);
                    }
                    decimal? overtime = null;

                    if (dt.Columns.Count > 3)
                    {
                        string overtimeText = row[3]?.ToString().Trim();

                        if (!string.IsNullOrEmpty(overtimeText))
                        {
                            decimal temp;
                            if (decimal.TryParse(overtimeText, out temp))
                            {
                                overtime = temp;
                            }
                            else
                            {
                                overtime = null; // invalid value safely ignored
                            }
                        }
                    }

                    decimal? bonus = null;
                    if (dt.Columns.Count > 4)
                    {
                        string bonusText = row[4]?.ToString().Trim();

                        if (!string.IsNullOrEmpty(bonusText))
                        {
                            decimal temp;
                            if (decimal.TryParse(bonusText, out temp))
                            {
                                bonus = temp;
                            }
                            else
                            {
                                bonus = null; 
                            }
                        }
                    }
                    decimal? incentive = null;
                    if (dt.Columns.Count > 5)
                    {
                        string IncentiveText = row[5]?.ToString().Trim();

                        if (!string.IsNullOrEmpty(IncentiveText))
                        {
                            decimal temp;
                            if (decimal.TryParse(IncentiveText, out temp))
                            {
                                incentive = temp;
                            }
                            else
                            {
                                incentive = null; 
                            }
                        }
                    }

                    // ✅ Save salary
                    SaveSalaryFromExcel(
                        user_id,
                        employeeName,
                        absentDays,
                        overtime,
                        bonus,
                        incentive
                    );
                }

            }

            ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
                                  "showUserSavedMessage('Success', 'Files uploaded successfully and saved salary slip Data!');" +
                                          "setTimeout(function(){ window.location.href = 'UploadExcelforsalary.aspx'; }, 3000);", true);
        }



        //private void SaveSalaryFromExcel(int userId, string name, int absentDays)
        //{
        //    SalarySlipBL bl = new SalarySlipBL();

        //    // 🔎 Fetch Salary Master Data
        //    var masterData = bl.GetSalaryMasterByUserId(userId);

        //    if (masterData.Count == 0) return;

        //    decimal fullBasicSalary = masterData[0].BasicSalary;
        //    string designation = masterData[0].DesignationName;
        //    int employeecode = masterData[0].employeecode;
        //    decimal ProfessionalTax = masterData[0].ProfessionalTax;

        //    int totalWorkingDays = 30;
        //    int presentDays = totalWorkingDays - absentDays;

        //    SalarySlipDO slip = new SalarySlipDO();

        //    slip.UserId = userId;
        //    slip.employeecode = employeecode;
        //    slip.Username = name;
        //    slip.DesignationName = designation;
        //    slip.DaysAbsent = absentDays;
        //    slip.DaysPresent = presentDays;
        //    slip.DaysPaid = presentDays;

        //    slip.Month = DateTime.Now.Month.ToString();
        //    slip.Year = DateTime.Now.Year;

        //    slip.BasicSalary = fullBasicSalary * presentDays / totalWorkingDays;
        //    slip.HouseRentAllowance = slip.BasicSalary * 0.40m;
        //    slip.SpecialAllowance = slip.BasicSalary * 0.20m;
        //    slip.LeaveTravelAllowance = slip.BasicSalary * 0.10m;
        //    slip.TotalEarnings = slip.BasicSalary + slip.HouseRentAllowance + slip.SpecialAllowance + slip.LeaveTravelAllowance;

        //    slip.ProfessionalTax = ProfessionalTax;
        //    slip.TotalDeductions = slip.ProfessionalTax;

        //    slip.NetPay = slip.TotalEarnings - slip.TotalDeductions;

        //    bl.SaveSalaryDetails(slip);
        //}
        private void SaveSalaryFromExcel(
    int userId,
    string name,
    int absentDays,
    decimal? overtimeAmount,
    decimal? bonusAmount,
    decimal? incentiveAmount
)
        {
            SalarySlipBL bl = new SalarySlipBL();

            var masterData = bl.GetSalaryMasterByUserId(userId);
            if (masterData.Count == 0) return;

            decimal fullBasicSalary = masterData[0].BasicSalary;
            string designation = masterData[0].DesignationName;
            int employeecode = masterData[0].employeecode;
            decimal professionalTax = masterData[0].ProfessionalTax;

            int totalWorkingDays = 30;
            int presentDays = totalWorkingDays - absentDays;

            decimal overtime = overtimeAmount ?? 0;
            decimal bonus = bonusAmount ?? 0;
            decimal incentive = incentiveAmount ?? 0;

            SalarySlipDO slip = new SalarySlipDO
            {
                UserId = userId,
                employeecode = employeecode,
                Username = name,
                DesignationName = designation,

                DaysAbsent = absentDays,
                DaysPresent = presentDays,
                DaysPaid = presentDays,

                Month = DateTime.Now.Month.ToString(),
                Year = DateTime.Now.Year,

                BasicSalary = fullBasicSalary * presentDays / totalWorkingDays
            };

            slip.HouseRentAllowance = slip.BasicSalary * 0.40m;
            slip.SpecialAllowance = slip.BasicSalary * 0.20m;
            slip.LeaveTravelAllowance = slip.BasicSalary * 0.10m;

            // ➕ Additional Components
            slip.Overtime = overtime;
            slip.Bonus = bonus;
            slip.Incentive = incentive;

            slip.TotalEarnings =
                slip.BasicSalary +
                slip.HouseRentAllowance +
                slip.SpecialAllowance +
                slip.LeaveTravelAllowance +
                overtime +
                bonus +
                incentive;

            slip.ProfessionalTax = professionalTax;
            slip.TotalDeductions = professionalTax;

            slip.NetPay = slip.TotalEarnings - slip.TotalDeductions;

            bl.SaveSalaryDetails(slip);
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {

                fileUpload.Attributes.Clear();


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUser", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

    }
}