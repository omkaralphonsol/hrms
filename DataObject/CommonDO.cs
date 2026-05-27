using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class CommonDO
    {
    }
    public class ResponseDO
    {
        public string Result { get; set; }
        public String message { get; set; }
        public string Remarks { get; set; }
        public string login_time { get; set; }
        public string logout_time { get; set; }

    }
    public class DropDownData
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AvailabilityId { get; set; }
        public string ComponentType { get; set; }


    }
    public class DropDown
    {
        public int lookupid { get; set; }
        public string lookupText { get; set; }


    }

    public class UsernameResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }

        
        public List<DropDownData> UserNameList { get; set; }
    }

    public class EmployeeCodeResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }


        public List<EmployeeCodeDO> EmployeeCodeList { get; set; }
    }
    public class EmployeeCodeDO
    {
        public int EmployeeCode { get; set; }
        public string EmpCode { get; set; }
    }
    public class EmployeeNameResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }


        public List<DropDownData> EmployeeNameList { get; set; }
    }

    public class DesignationResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }


        public List<DropDownData> DesignationList { get; set; }
    }
}
