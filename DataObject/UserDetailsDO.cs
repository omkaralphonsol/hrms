using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class UserDetailsDO
    {
        public int UserId { get; set; }
        public int? usernameId { get; set; }
        public int? empcodeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Username { get; set; }

        public string user_fullname { get; set; }

        public string roledescription { get; set; }

        public string user_mail_id { get; set; }

        public string contact_detail { get; set; }

        public string user_type { get; set; }

        public bool Isactive { get; set; }

        public DateTime ActivatedDate { get; set; }

        public string UserStatusflag { get; set; }

        public DateTime DeactivatedDate { get; set; }

        public bool PassResetflag { get; set; }

        public int WrongPassCount { get; set; }

        public int Insertedby { get; set; }

        public DateTime? Inserteddate { get; set; }

        public int? Updatedby { get; set; }

        public DateTime Updateddate { get; set; }

        public bool AllowmultipleRoles { get; set; }

        //public string EmpCode { get; set; }
        //public int designation_id { get; set; }
        public int CompanyId { get; set; }

        public string user_role { get; set; }
        public string password { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string searchbyType { get; set; }
        public Int32 designation_id { get; set; }
        public string designation_name { get; set; }
        public string searchValue { get; set; }

        public DateTime? TerminationDate { get; set; }

        public string notice_status { get; set; }
        public DateTime? ResponseDeadline { get; set; }

        public Int32 company_id { get; set; }
        public string company_name { get; set; }

        public int ESIC_no { get; set; }

        public int PF_no { get; set; }

        public string department { get; set; }

        public string branch { get; set; }

        public string division { get; set; }

        public DateTime date_of_joining { get; set; }

        public int probation_period_months { get; set; }

        public string reporting_manager { get; set; }

        public string employee_type { get; set; }

    }
    public class UpdateProbationRequest
    {
        public int UserId { get; set; }
        public int ProbationFlag { get; set; }
        public string Remark { get; set; }   // <-- ADD THIS

        public string DateOfExtended { get; set; }   // <-- ADD THIS


    }
    public class userProbationflagResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }
    }
    public class UserDateForProbationPeriodDO
    {
        public int EmpleaveDetailsId { get; set; }
        public DateTime InsertedDate { get; set; }
        public int SixMonthCompleted { get; set; }

        // Response message (SUCCESS / FAILED / INFO)
        public string ResponseMessage { get; set; }
    }
    public class userDateResponseDataDO
    {
        //public int User_Id { get; set; }
        //public string Username { get; set; }
        //public string User_fullname { get; set; }
        //public string User_Email { get; set; }
        //public string Contact_No { get; set; }
        //public string User_Role { get; set; }

        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }

        public List<UserDateForProbationPeriodDO> UsersprobationperiodDateList { get; set; }
    }

    public class UserEmployeeTypeDO
    {
        public string employee_type { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class UserEmployeeTypeResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }

        public List<UserEmployeeTypeDO> EmployeeTypeList { get; set; }
    }

}
