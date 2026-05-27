using DataObject;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ProcessModel
{
    public class SalarySlipBL
    {


        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        public List<SalarySlipDO> SaveSalaryDetails(SalarySlipDO salary)
        {
            List<SalarySlipDO> listdata = new List<SalarySlipDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                string userId = HttpContext.Current.Session["userId"] != null
                            ? HttpContext.Current.Session["userId"].ToString()
                            : "";

                // First persist/update employee master salary profile, then save month-wise salary slip.
                TryUpsertEmployeeMasterSalary(salary, userId);

                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertSalary"));
                mysqlParameters.Add(DataClass.GetParameter("p_employeecode", salary.employeecode));
                mysqlParameters.Add(DataClass.GetParameter("p_username", salary.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_month", salary.Month));
                mysqlParameters.Add(DataClass.GetParameter("p_year", salary.Year));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_name", salary.DesignationName));
                mysqlParameters.Add(DataClass.GetParameter("p_days_paid", salary.DaysPaid));
                mysqlParameters.Add(DataClass.GetParameter("p_days_present", salary.DaysPresent));
                mysqlParameters.Add(DataClass.GetParameter("p_days_absent", salary.DaysAbsent));
                mysqlParameters.Add(DataClass.GetParameter("p_basic_salary", salary.BasicSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_house_rent_allowance", salary.HouseRentAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_special_allowance", salary.SpecialAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_leave_travel_allowance", salary.LeaveTravelAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_professional_tax", salary.ProfessionalTax));
                mysqlParameters.Add(DataClass.GetParameter("p_total_earnings", salary.TotalEarnings));
                mysqlParameters.Add(DataClass.GetParameter("p_total_deductions", salary.TotalDeductions));
                mysqlParameters.Add(DataClass.GetParameter("p_net_pay", salary.NetPay));
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", salary.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", userId));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", salary.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_bonus", salary.Bonus));
                mysqlParameters.Add(DataClass.GetParameter("p_incentive", salary.Incentive));
                mysqlParameters.Add(DataClass.GetParameter("p_Overtime", salary.Overtime));





                listdata = getDrtolistParam.getdatafromreder<SalarySlipDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_InsertSalarySlip")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "SaveSalaryDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        private void TryUpsertEmployeeMasterSalary(SalarySlipDO salary, string insertedBy)
        {
            try
            {
                int monthValue = 0;
                int.TryParse(Convert.ToString(salary.Month), out monthValue);
                int yearValue = salary.Year > 0 ? salary.Year : DateTime.Now.Year;
                decimal monthlyCtc = salary.TotalEarnings;
                decimal yearlyCtc = salary.TotalEarnings * 12;

                List<MySqlParameter> masterParams = new List<MySqlParameter>();
                masterParams.Add(DataClass.GetParameter("@p_user_id", salary.UserId));
                masterParams.Add(DataClass.GetParameter("@p_employeeCode", salary.employeecode));
                masterParams.Add(DataClass.GetParameter("@p_employeeName", salary.Username));
                masterParams.Add(DataClass.GetParameter("@p_month", monthValue));
                masterParams.Add(DataClass.GetParameter("@p_year", yearValue));
                masterParams.Add(DataClass.GetParameter("@p_designation", salary.DesignationName));
                masterParams.Add(DataClass.GetParameter("@p_DaysPaid", salary.DaysPaid));
                masterParams.Add(DataClass.GetParameter("@p_DaysPresent", salary.DaysPresent));
                masterParams.Add(DataClass.GetParameter("@p_DaysAbsent", salary.DaysAbsent));
                masterParams.Add(DataClass.GetParameter("@p_BasicSalary", salary.BasicSalary));
                masterParams.Add(DataClass.GetParameter("@p_HouseRentAllowance", salary.HouseRentAllowance));
                masterParams.Add(DataClass.GetParameter("@p_SpecialAllowance", salary.SpecialAllowance));
                masterParams.Add(DataClass.GetParameter("@p_LeaveTravelAllowance", salary.LeaveTravelAllowance));
                masterParams.Add(DataClass.GetParameter("@p_TotalEarnings", salary.TotalEarnings));
                masterParams.Add(DataClass.GetParameter("@p_MonthlyCTC", monthlyCtc));
                masterParams.Add(DataClass.GetParameter("@p_YearlyCTC", yearlyCtc));
                masterParams.Add(DataClass.GetParameter("@p_ProfessionalTax", salary.ProfessionalTax));
                masterParams.Add(DataClass.GetParameter("@p_TotalDeductions", salary.TotalDeductions));
                masterParams.Add(DataClass.GetParameter("@p_NetPay", salary.NetPay));
                masterParams.Add(DataClass.GetParameter("@p_inserted_by", insertedBy));

                DataClass.GetDataReaderFromSpWithParam(masterParams, DBName, "sp_upsert_employee_master_salary");
            }
            catch (Exception ex)
            {
                // Keep salary slip save path working even if SP is not deployed yet.
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "TryUpsertEmployeeMasterSalary",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }
        }

        public List<BioxiaSalarySlipDO> SaveBioxiaSalaryDetails(BioxiaSalarySlipDO salary)
        {
            List<BioxiaSalarySlipDO> listdata = new List<BioxiaSalarySlipDO>();
            getDrtolist getDrtolistParam = new getDrtolist();

            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
            {
                DataClass.GetParameter("p_type", "InsertBioxiaSalary"),
                DataClass.GetParameter("p_employeecode", salary.employeecode),
                DataClass.GetParameter("p_username", salary.Username),
                DataClass.GetParameter("p_month", salary.Month),
                DataClass.GetParameter("p_year", salary.Year),
                DataClass.GetParameter("p_designation_name", salary.DesignationName),
                DataClass.GetParameter("p_department", salary.Department),
                DataClass.GetParameter("p_branch", salary.Branch),
                DataClass.GetParameter("p_grade", salary.Grade),
                DataClass.GetParameter("p_division", salary.Division),
                DataClass.GetParameter("p_ESIC_No", salary.ESIC_No),
                DataClass.GetParameter("p_PF_No", salary.PF_No),
                DataClass.GetParameter("p_JoiningDate", salary.JoiningDate),
                DataClass.GetParameter("p_days_paid", salary.DaysPaid),
                //DataClass.GetParameter("p_days_present", salary.DaysPresent),
                DataClass.GetParameter("p_days_absent", salary.DaysAbsent),
                DataClass.GetParameter("p_basic_salary", salary.BasicSalary),
                DataClass.GetParameter("p_house_rent_allowance", salary.HouseRentAllowance),
                DataClass.GetParameter("p_special_allowance", salary.SpecialAllowance),
                DataClass.GetParameter("p_leave_travel_allowance", salary.LeaveTravelAllowance),
                DataClass.GetParameter("p_MobileReimbursement", salary.MobileReimbursement),
                DataClass.GetParameter("p_Bonus", salary.Bonus),
                DataClass.GetParameter("p_Incentive", salary.Incentive),
                DataClass.GetParameter("p_Others", salary.Others),
                DataClass.GetParameter("p_taxOthers", salary.taxOthers),
                DataClass.GetParameter("p_GrossPay", salary.GrossPay),
                DataClass.GetParameter("p_professional_tax", salary.ProfessionalTax),
                DataClass.GetParameter("p_ProfessionalTaxDeducted", salary.ProfessionalTaxDeducted),
                 DataClass.GetParameter("p_tds", salary.TDS),
                DataClass.GetParameter("p_total_earnings", salary.TotalEarnings),
                DataClass.GetParameter("p_total_deductions", salary.TotalDeductions),
                DataClass.GetParameter("p_net_pay", salary.finalNetpay),
                DataClass.GetParameter("p_inserted_by", salary.InsertedBy)
            };

                listdata = getDrtolistParam.getdatafromreder<BioxiaSalarySlipDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_InsertSalarySlip_Bioxia")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "SaveBioxiaSalaryDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public List<BioxiaSalarySlipDO> SaveImsetSalaryDetails(BioxiaSalarySlipDO salary)
        {
            List<BioxiaSalarySlipDO> listdata = new List<BioxiaSalarySlipDO>();
            getDrtolist getDrtolistParam = new getDrtolist();

            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
            {
                DataClass.GetParameter("p_type", "InsertBioxiaSalary"),
                DataClass.GetParameter("p_empname", salary.Username),
                DataClass.GetParameter("p_month", salary.Month),
                DataClass.GetParameter("p_year", salary.Year),
                DataClass.GetParameter("p_designation", salary.DesignationName),
                DataClass.GetParameter("p_department", salary.Department),
                DataClass.GetParameter("p_branch", salary.Branch),
                DataClass.GetParameter("p_grade", salary.Grade),

                DataClass.GetParameter("p_days_paid", salary.DaysPaid),
                //DataClass.GetParameter("p_days_present", salary.DaysPresent),
                DataClass.GetParameter("p_days_absent", salary.DaysAbsent),
                DataClass.GetParameter("p_basic_salary", salary.BasicSalary),
                DataClass.GetParameter("p_house_rent_allowance", salary.HouseRentAllowance),
                DataClass.GetParameter("p_special_allowance", salary.SpecialAllowance),
                DataClass.GetParameter("p_leave_travel_allowance", salary.LeaveTravelAllowance),
                DataClass.GetParameter("p_mobile_reimbursement", salary.MobileReimbursement),
                DataClass.GetParameter("p_bonus", salary.Bonus),
                DataClass.GetParameter("p_incentive", salary.Incentive),
                DataClass.GetParameter("p_others_earning", salary.Others),
                DataClass.GetParameter("p_taxOthers", salary.taxOthers),
                DataClass.GetParameter("p_grosspay", salary.GrossPay),
                DataClass.GetParameter("p_professional_tax", salary.ProfessionalTax),
                DataClass.GetParameter("p_tds", salary.TDS),
                DataClass.GetParameter("p_total_earnings", salary.TotalEarnings),
                DataClass.GetParameter("p_total_deductions", salary.TotalDeductions),
                DataClass.GetParameter("p_final_net_pay", salary.finalNetpay),
                DataClass.GetParameter("p_inserted_by", salary.InsertedBy),
                DataClass.GetParameter("p_company_id", salary.company_id),
                DataClass.GetParameter("p_employeecode", salary.employeecode),

            };

                listdata = getDrtolistParam.getdatafromreder<BioxiaSalarySlipDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_insert_salary_slip_Imset")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "SaveImsetSalaryDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public List<SalarySlipDO> GetSalaryMasterByUserId(int userId)
        {
            List<SalarySlipDO> listdata = new List<SalarySlipDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));

                listdata = getDrtolistParam.getdatafromreder<SalarySlipDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_employee_master_salary")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalarySlipBL", "GetSalaryMasterByUserId",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        public List<SalarySlipDO> GetSalaryMasterByEmployeeCode(string employeeCode)
        {
            List<SalarySlipDO> listdata = new List<SalarySlipDO>();
            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                return listdata;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT
                        user_id,
                        employeeCode AS employeecode,
                        employeeName AS username,
                        month,
                        year,
                        designation AS designation_name,
                        DaysPaid AS days_paid,
                        DaysPresent AS days_present,
                        DaysAbsent AS days_absent,
                        BasicSalary AS basic_salary,
                        HouseRentAllowance AS house_rent_allowance,
                        SpecialAllowance AS special_allowance,
                        LeaveTravelAllowance AS leave_travel_allowance,
                        ProfessionalTax AS professional_tax,
                        TotalEarnings AS total_earnings,
                        TotalDeductions AS total_deductions,
                        NetPay AS net_pay
                    FROM employee_salary_master
                    WHERE employeeCode = @p_employee_code
                    ORDER BY inserted_date DESC
                    LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@p_employee_code", employeeCode);
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listdata.Add(new SalarySlipDO
                            {
                                UserId = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
                                employeecode = dr["employeecode"] != DBNull.Value ? Convert.ToInt32(dr["employeecode"]) : 0,
                                Username = dr["username"] != DBNull.Value ? Convert.ToString(dr["username"]) : string.Empty,
                                Month = dr["month"] != DBNull.Value ? Convert.ToString(dr["month"]) : string.Empty,
                                Year = dr["year"] != DBNull.Value ? Convert.ToInt32(dr["year"]) : 0,
                                DesignationName = dr["designation_name"] != DBNull.Value ? Convert.ToString(dr["designation_name"]) : string.Empty,
                                DaysPaid = dr["days_paid"] != DBNull.Value ? Convert.ToInt32(dr["days_paid"]) : 0,
                                DaysPresent = dr["days_present"] != DBNull.Value ? Convert.ToInt32(dr["days_present"]) : 0,
                                DaysAbsent = dr["days_absent"] != DBNull.Value ? Convert.ToInt32(dr["days_absent"]) : 0,
                                BasicSalary = dr["basic_salary"] != DBNull.Value ? Convert.ToDecimal(dr["basic_salary"]) : 0,
                                HouseRentAllowance = dr["house_rent_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["house_rent_allowance"]) : 0,
                                SpecialAllowance = dr["special_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["special_allowance"]) : 0,
                                LeaveTravelAllowance = dr["leave_travel_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["leave_travel_allowance"]) : 0,
                                ProfessionalTax = dr["professional_tax"] != DBNull.Value ? Convert.ToDecimal(dr["professional_tax"]) : 0,
                                TotalEarnings = dr["total_earnings"] != DBNull.Value ? Convert.ToDecimal(dr["total_earnings"]) : 0,
                                TotalDeductions = dr["total_deductions"] != DBNull.Value ? Convert.ToDecimal(dr["total_deductions"]) : 0,
                                NetPay = dr["net_pay"] != DBNull.Value ? Convert.ToDecimal(dr["net_pay"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalarySlipBL", "GetSalaryMasterByEmployeeCode",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }
        public List<RemunerationDO> GetRemunerationDetails(string type, int? month, int? year)
        {
            List<RemunerationDO> listdata = new List<RemunerationDO>();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_Type", type));
                mysqlParameters.Add(DataClass.GetParameter("@p_Month", month));
                mysqlParameters.Add(DataClass.GetParameter("@p_Year", year));

                using (var reader = DataClass.GetDataReaderFromSpWithParam(
                    mysqlParameters,
                    DBName,
                    "sp_get_remuneration_details"))
                {
                    while (reader.Read())
                    {
                        RemunerationDO item = new RemunerationDO
                        {
                            employeeCode = reader["employeeCode"] != DBNull.Value ? Convert.ToInt32(reader["employeeCode"]) : 0,
                            Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : string.Empty,
                            MonthlyCTC = reader["MonthlyCTC"] != DBNull.Value
    ? Convert.ToDecimal(reader["MonthlyCTC"])
    : (decimal?)null,

                            YearlyCTC = reader["YearlyCTC"] != DBNull.Value
    ? Convert.ToDecimal(reader["YearlyCTC"])
    : (decimal?)null,

                        };

                        listdata.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "RemunerationBL",
                    "GetRemunerationDetails",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public List<SalarySlipDO> SaveRenumerationSalaryDetails(
        SalarySlipDO salary,
        decimal lta,
        decimal bonus,
        decimal incentive,
        decimal others)
        {
            List<SalarySlipDO> listdata = new List<SalarySlipDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                string userId = HttpContext.Current.Session["userId"]?.ToString() ?? "";

                // Ensure master salary row exists/updates for first-time employee before month-wise slip insert.
                TryUpsertEmployeeMasterSalary(salary, userId);

                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertSalaryRemuneration"));
                mysqlParameters.Add(DataClass.GetParameter("p_employeecode", salary.employeecode));
                mysqlParameters.Add(DataClass.GetParameter("p_username", salary.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_month", salary.Month));
                mysqlParameters.Add(DataClass.GetParameter("p_year", salary.Year));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_name", salary.DesignationName));

                mysqlParameters.Add(DataClass.GetParameter("p_basic_salary", salary.BasicSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_house_rent_allowance", salary.HouseRentAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_special_allowance", salary.SpecialAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_leave_travel_allowance", lta));

                mysqlParameters.Add(DataClass.GetParameter("p_bonus", bonus));
                mysqlParameters.Add(DataClass.GetParameter("p_incentive", incentive));
                mysqlParameters.Add(DataClass.GetParameter("p_others", others));

                mysqlParameters.Add(DataClass.GetParameter("p_professional_tax", salary.ProfessionalTax));
                mysqlParameters.Add(DataClass.GetParameter("p_total_earnings", salary.TotalEarnings));
                mysqlParameters.Add(DataClass.GetParameter("p_total_deductions", salary.TotalDeductions));
                mysqlParameters.Add(DataClass.GetParameter("p_net_pay", salary.NetPay));

                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", userId));
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", salary.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", salary.company_id));

                mysqlParameters.Add(DataClass.GetParameter("@p_last_appraisal_date", salary.LastAppraisalDate));
                mysqlParameters.Add(DataClass.GetParameter("@p_current_appraisal_date", salary.CurrentAppraisalDate));
                mysqlParameters.Add(DataClass.GetParameter("@p_appraisal_percent", salary.AppraisalPercentage));
                mysqlParameters.Add(DataClass.GetParameter("@p_increment_amount", salary.IncrementAmount));





                listdata = getDrtolistParam.getdatafromreder<SalarySlipDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "SP_InsertSalarySlip_ForRemuneration")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "SaveRenumerationSalaryDetails",
                    ex.Message + " | " + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public DateTime? GetLastAppraisalDate(int userId)
        {
            DateTime? lastDate = null;

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            using (MySqlCommand cmd = new MySqlCommand("sp_get_last_appraisal_date", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_user_id", userId);

                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr["last_appraisal_date"] != DBNull.Value)
                            lastDate = Convert.ToDateTime(dr["last_appraisal_date"]);
                    }
                }
            }

            return lastDate;
        }

        public SalarySlipDO GetLatestSavedSlipByUserId(int userId)
        {
            SalarySlipDO slip = null;
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand(@"
                    SELECT
                        user_id,
                        employeecode,
                        username,
                        month,
                        year,
                        designation_name,
                        days_paid,
                        days_present,
                        days_absent,
                        basic_salary,
                        house_rent_allowance,
                        special_allowance,
                        leave_travel_allowance,
                        professional_tax,
                        total_earnings,
                        total_deductions,
                        net_pay
                    FROM salary_slip_details
                    WHERE user_id = @p_user_id
                    ORDER BY inserted_date DESC
                    LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            slip = new SalarySlipDO
                            {
                                UserId = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
                                employeecode = dr["employeecode"] != DBNull.Value ? Convert.ToInt32(dr["employeecode"]) : 0,
                                Username = dr["username"] != DBNull.Value ? dr["username"].ToString() : string.Empty,
                                Month = dr["month"] != DBNull.Value ? dr["month"].ToString() : string.Empty,
                                Year = dr["year"] != DBNull.Value ? Convert.ToInt32(dr["year"]) : 0,
                                DesignationName = dr["designation_name"] != DBNull.Value ? dr["designation_name"].ToString() : string.Empty,
                                DaysPaid = dr["days_paid"] != DBNull.Value ? Convert.ToInt32(dr["days_paid"]) : 0,
                                DaysPresent = dr["days_present"] != DBNull.Value ? Convert.ToInt32(dr["days_present"]) : 0,
                                DaysAbsent = dr["days_absent"] != DBNull.Value ? Convert.ToInt32(dr["days_absent"]) : 0,
                                BasicSalary = dr["basic_salary"] != DBNull.Value ? Convert.ToDecimal(dr["basic_salary"]) : 0,
                                HouseRentAllowance = dr["house_rent_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["house_rent_allowance"]) : 0,
                                SpecialAllowance = dr["special_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["special_allowance"]) : 0,
                                LeaveTravelAllowance = dr["leave_travel_allowance"] != DBNull.Value ? Convert.ToDecimal(dr["leave_travel_allowance"]) : 0,
                                ProfessionalTax = dr["professional_tax"] != DBNull.Value ? Convert.ToDecimal(dr["professional_tax"]) : 0,
                                TotalEarnings = dr["total_earnings"] != DBNull.Value ? Convert.ToDecimal(dr["total_earnings"]) : 0,
                                TotalDeductions = dr["total_deductions"] != DBNull.Value ? Convert.ToDecimal(dr["total_deductions"]) : 0,
                                NetPay = dr["net_pay"] != DBNull.Value ? Convert.ToDecimal(dr["net_pay"]) : 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "GetLatestSavedSlipByUserId",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return slip;
        }

        public void SaveRemunerationComponents(int employeeId, int month, int year, List<SalaryComponent> components, int insertedBy)
        {
            if (components == null || components.Count == 0)
            {
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    con.Open();

                    foreach (var component in components)
                    {
                        if (component == null || string.IsNullOrWhiteSpace(component.ComponentName))
                        {
                            continue;
                        }

                        int componentId = GetOrCreateComponentIdByName(con, component.ComponentName, component.IsDeduction, insertedBy);
                        if (componentId <= 0)
                        {
                            continue;
                        }

                        using (MySqlCommand cmd = new MySqlCommand("sp_save_remuneration_component", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@p_employee_id", employeeId);
                            cmd.Parameters.AddWithValue("@p_month", month);
                            cmd.Parameters.AddWithValue("@p_year", year);
                            cmd.Parameters.AddWithValue("@p_component_id", componentId);
                            cmd.Parameters.AddWithValue("@p_amount", component.Amount);
                            cmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "SaveRemunerationComponents",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace,
                    UserId
                );
            }
        }

        private int GetOrCreateComponentIdByName(MySqlConnection con, string componentName, bool isDeduction, int insertedBy)
        {
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT salary_comp_master_id FROM salarycomponent_master WHERE componentname = @p_component_name AND is_active = 1 LIMIT 1;", con))
            {
                cmd.Parameters.AddWithValue("@p_component_name", componentName);
                object value = cmd.ExecuteScalar();
                if (value != null && value != DBNull.Value)
                {
                    int componentId;
                    if (int.TryParse(Convert.ToString(value), out componentId))
                    {
                        return componentId;
                    }
                }
            }

            // Auto-create component in master if not found, with full metadata for dynamic payroll scalability.
            string componentCode = BuildComponentCode(componentName);
            string componentType = isDeduction ? "DEDUCTION" : "EARNING";
            using (MySqlCommand insertCmd = new MySqlCommand(
                @"INSERT INTO salarycomponent_master
                  (componentname, componentcode, component_type, is_active, inserted_by, inserted_date)
                  VALUES
                  (@p_component_name, @p_component_code, @p_component_type, 1, @p_inserted_by, NOW());", con))
            {
                insertCmd.Parameters.AddWithValue("@p_component_name", componentName);
                insertCmd.Parameters.AddWithValue("@p_component_code", componentCode);
                insertCmd.Parameters.AddWithValue("@p_component_type", componentType);
                insertCmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);
                insertCmd.ExecuteNonQuery();
                return Convert.ToInt32(insertCmd.LastInsertedId);
            }
        }

        public UserDetailsDO InsertSalaryComponent(string componentName, string componentType, int insertedBy)
        {
            UserDetailsDO response = new UserDetailsDO
            {
                Status = "Failed",
                Remarks = "Unable to save component."
            };

            try
            {
                string safeName = Convert.ToString(componentName ?? string.Empty).Trim();
                string safeType = Convert.ToString(componentType ?? string.Empty).Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(safeName))
                {
                    response.Remarks = "Component name is required.";
                    return response;
                }
                if (safeType != "EARNING" && safeType != "DEDUCTION")
                {
                    response.Remarks = "Invalid component type.";
                    return response;
                }

                // Preferred path: use DB stored procedure for standardized inserts.
                try
                {
                    var spParams = new List<MySqlParameter>
                    {
                        DataClass.GetParameter("@p_componentname", safeName),
                        DataClass.GetParameter("@p_component_type", safeType),
                        DataClass.GetParameter("@p_inserted_by", insertedBy)
                    };

                    using (var reader = DataClass.GetDataReaderFromSpWithParam(spParams, DBName, "Sp_InsertSalaryComponent"))
                    {
                        if (reader != null && reader.Read())
                        {
                            response.Status = reader["Status"] != DBNull.Value ? Convert.ToString(reader["Status"]) : "Failed";
                            response.Remarks = reader["Remarks"] != DBNull.Value ? Convert.ToString(reader["Remarks"]) : string.Empty;
                            if (HasColumn(reader, "component_id") && reader["component_id"] != DBNull.Value)
                            {
                                response.UserId = Convert.ToInt32(reader["component_id"]);
                            }
                            return response;
                        }
                    }
                }
                catch
                {
                    // Fallback to direct insert path below.
                }

                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand checkCmd = new MySqlCommand(
                    "SELECT salary_comp_master_id FROM salarycomponent_master WHERE LOWER(componentname)=LOWER(@p_component_name) AND is_active=1 LIMIT 1;", con))
                {
                    checkCmd.Parameters.AddWithValue("@p_component_name", safeName);
                    con.Open();
                    object existing = checkCmd.ExecuteScalar();
                    if (existing != null && existing != DBNull.Value)
                    {
                        response.Status = "Success";
                        response.Remarks = "Component already exists.";
                        response.UserId = Convert.ToInt32(existing); // carry component id to caller
                        return response;
                    }

                    string code = BuildComponentCode(safeName);
                    using (MySqlCommand insertCmd = new MySqlCommand(
                        @"INSERT INTO salarycomponent_master
                          (componentname, componentcode, component_type, is_active, inserted_by, inserted_date)
                          VALUES
                          (@p_component_name, @p_component_code, @p_component_type, 1, @p_inserted_by, NOW());", con))
                    {
                        insertCmd.Parameters.AddWithValue("@p_component_name", safeName);
                        insertCmd.Parameters.AddWithValue("@p_component_code", code);
                        insertCmd.Parameters.AddWithValue("@p_component_type", safeType);
                        insertCmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);
                        insertCmd.ExecuteNonQuery();
                    }

                    using (MySqlCommand idCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", con))
                    {
                        object insertedIdObj = idCmd.ExecuteScalar();
                        int insertedId = insertedIdObj != null && insertedIdObj != DBNull.Value ? Convert.ToInt32(insertedIdObj) : 0;
                        response.Status = "Success";
                        response.Remarks = "Component created successfully.";
                        response.UserId = insertedId; // carry component id to caller
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "InsertSalaryComponent",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return response;
        }

        private bool HasColumn(IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private string BuildComponentCode(string componentName)
        {
            string code = Convert.ToString(componentName ?? string.Empty).Trim().ToUpperInvariant();
            code = Regex.Replace(code, @"[^A-Z0-9]+", "_");
            code = Regex.Replace(code, "_{2,}", "_").Trim('_');
            return string.IsNullOrWhiteSpace(code) ? "COMPONENT" : code;
        }

        public List<SalaryComponent> GetRemunerationComponents(int employeeId, int month, int year)
        {
            List<SalaryComponent> components = new List<SalaryComponent>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_remuneration_components", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_employee_id", employeeId);
                    cmd.Parameters.AddWithValue("@p_month", month);
                    cmd.Parameters.AddWithValue("@p_year", year);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string componentName = dr["component_name"] != DBNull.Value
                                ? dr["component_name"].ToString()
                                : string.Empty;
                            decimal amount = dr["amount"] != DBNull.Value
                                ? Convert.ToDecimal(dr["amount"])
                                : 0;

                            string componentType = dr["component_type"] != DBNull.Value
                                ? Convert.ToString(dr["component_type"])
                                : string.Empty;
                            bool isDeduction = string.Equals(componentType, "DEDUCTION", StringComparison.OrdinalIgnoreCase);

                            components.Add(new SalaryComponent
                            {
                                ComponentName = componentName,
                                Amount = amount,
                                IsDeduction = isDeduction
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlipBL",
                    "GetRemunerationComponents",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return components;
        }


    }
}
