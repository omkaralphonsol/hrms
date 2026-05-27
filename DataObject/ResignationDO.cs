using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class ResignationDO
    {
        public int EmployeeResignationId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public DateTime resignation_date { get; set; }
        public int notice_period_days { get; set; }
        public DateTime last_working_date { get; set; }
        public string reason { get; set; }
        public string hr_status { get; set; }

        public string last_working_date_display { get; set; }


    }

    public class ResignationResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
        public List<ResignationDO> ResignationList { get; set; }
    }
    public class ResignationActionResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
    }

    public class HandOverDO
    {
        public int EmployeeResignationId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public DateTime resignation_date { get; set; }
        public DateTime last_working_date { get; set; }
        public string hr_status { get; set; }

        public string last_working_date_display { get; set; }


    }
    public class HandOverResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
        public List<HandOverDO> HandOverProcessList { get; set; }
    }
    public class HandoverProcessDO
    {
        public int EmployeeResignationId { get; set; }
        public int UserId { get; set; }

        public bool PendriveBackup { get; set; }
        public bool LaptopWithCharger { get; set; }
        public bool ContactDetailsShared { get; set; }
        public bool DiarySubmitted { get; set; }
        public int ID_Card { get; set; }   // <-- map DB column as int

        public bool IDCard
        {
            get { return ID_Card == 1; }
            set { ID_Card = value ? 1 : 0; }
        }

        public string HR_Remark { get; set; }
        public string Status { get; set; }   
        public string Remarks { get; set; }  
        public int InsertedBy { get; set; }
    }

    public class TerminationProcessDO
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime TerminationDate { get; set; }
        //public int TerminationReasonId { get; set; }
        //public string Remark { get; set; }
        public int InsertedBy { get; set; }

        public string termination_reason { get; set; }

        public int? PerformanceRating { get; set; }
        public int? NoticePeriodDays { get; set; }
        public string TerminationLetter { get; set; }

        public DateTime? ResponseDeadline { get; set; }
        public string NoticeLetter { get; set; }

        public string EmployeeEmail { get; set; }
        public string EmployeeName { get; set; }
        public string notice_status { get; set; }

    }

}
