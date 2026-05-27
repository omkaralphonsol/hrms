using System;

namespace DataObject
{
    public class EmployeeLeaveListDO
    {
        public int leave_id { get; set; }
        public int emp_id { get; set; }
        public string emp_name { get; set; }
        public string approval_status { get; set; }
        public string request_date { get; set; }
    }

    public class EmployeeLeaveDetailDO
    {
        public int leave_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string leave_description { get; set; }
        public int approval_status { get; set; }
        public int approval_status_id { get; set; }
        public string approval_status_text { get; set; }
        public DateTime created { get; set; }
        public int lookupId { get; set; }
        public string leave_daytype { get; set; }
        public int? leaves_types_id { get; set; }
        public string leaves_types { get; set; }
        public string leavefromtime { get; set; }
        public string leavetotime { get; set; }
        public string rejection_remark { get; set; }
    }
}
