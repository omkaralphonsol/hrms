using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class SalarySlipDO
    {
        public int UserId { get; set; }

        public int employeecode { get; set; }
        public string Username { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string DesignationName { get; set; }
        public int DaysPaid { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal LeaveTravelAllowance { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public int InsertedBy { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int company_id { get; set; }
        public decimal Bonus { get; set; }
        public decimal Incentive { get; set; }
        public decimal Others { get; set; }

        public decimal? AppraisalPercentage { get; set; }
        public decimal? IncrementAmount { get; set; }

        public DateTime? LastAppraisalDate { get; set; }
        public DateTime? CurrentAppraisalDate { get; set; }

        public decimal Overtime { get; set; }

    }

    public class userEmployeecodeanddesignDO
    {
        public string EmployeeCode { get; set; }
        public int UserId { get; set; }
        public string designation { get; set; }

        public int company_id { get; set; }



    }
    public class userEmployeecodeanddesigresponseDO
    {
        public bool Success { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
        public List<userEmployeecodeanddesignDO> EmployeeCodeAndDesignationList { get; set; }
    }
    public class BioxiaSalarySlipDO
    {
        public int UserId { get; set; }
        public int employeecode { get; set; }
        public string Username { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string DesignationName { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Grade { get; set; }
        public string Division { get; set; }
        public string ESIC_No { get; set; }
        public string PF_No { get; set; }
        public DateTime JoiningDate { get; set; }

        public int DaysPaid { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent { get; set; }

        // Earnings
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllowance { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal LeaveTravelAllowance { get; set; }
        public decimal MobileReimbursement { get; set; }
        public decimal Bonus { get; set; }
        public decimal Incentive { get; set; }
        public decimal Others { get; set; }
        public decimal GrossPay { get; set; }

        // Deductions
        public decimal ProfessionalTax { get; set; }
        public decimal ProfessionalTaxDeducted { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal finalNetpay { get; set; }
        public decimal Netpay { get; set; }

        public int InsertedBy { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

        public decimal taxOthers { get; set; }

        public decimal TDS { get; set; }

        public int company_id { get; set; }
    }
    public class RemunerationDO
    {
        public int UserId { get; set; }
        public int employeeCode { get; set; }
        public string Username { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }

        public decimal? MonthlyCTC { get; set; }
        public decimal? YearlyCTC { get; set; }
    }

    [Serializable]   // ⭐ REQUIRED for ViewState
    public class SalaryComponent
    {
        public string ComponentName { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeduction { get; set; }
    }
    public class AppraisalDO
    {
        public DateTime? last_appraisal_date { get; set; }
    }

}
