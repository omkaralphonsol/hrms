using DataObject;
using ProcessModel;
using System;

namespace HRMS.View.Modules
{
    public partial class EmployeeLeaveView : System.Web.UI.Page
    {
        protected string UserId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                int leaveId = 0;
                int.TryParse(Convert.ToString(Request.QueryString["leaveId"]), out leaveId);
                BindLeaveData(leaveId);
            }
        }

        private void BindLeaveData(int leaveId)
        {
            try
            {
                if (leaveId <= 0)
                {
                    return;
                }

                EmployeeLeaveBL leaveBL = new EmployeeLeaveBL();
                EmployeeLeaveDetailDO detail = leaveBL.GetLeaveRequestById(leaveId);
                if (detail == null)
                {
                    txtDescription.Text = "No leave data found for Leave Id: " + leaveId;
                    return;
                }

                txtStartDate.Text = detail.start_date;
                txtEndDate.Text = detail.end_date;
                txtApprovalStatus.Text = !string.IsNullOrWhiteSpace(detail.approval_status_text)
                    ? detail.approval_status_text
                    : GetVerificationStatusText(detail.approval_status);
                txtLookupId.Text = !string.IsNullOrWhiteSpace(detail.leave_daytype)
                    ? detail.leave_daytype
                    : GetLeaveDayTypeText(detail.lookupId);
                txtLeaveTypeId.Text = !string.IsNullOrWhiteSpace(detail.leaves_types)
                    ? detail.leaves_types
                    : (detail.leaves_types_id.HasValue ? Convert.ToString(detail.leaves_types_id.Value) : "N/A");
                txtApprovedFromDate.Text = detail.start_date;
                txtApprovedToDate.Text = detail.end_date;
                txtDescription.Text = detail.leave_description;
                txtRejectionRemark.Text = detail.rejection_remark;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveView", "BindLeaveData", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/EmployeeLeaveList.aspx", false);
        }

        private string GetVerificationStatusText(int statusId)
        {
            switch (statusId)
            {
                case 1: return "Accepted";
                case 2: return "Rejected";
                case 3: return "Pending";
                default: return Convert.ToString(statusId);
            }
        }

        private string GetLeaveDayTypeText(int dayTypeId)
        {
            switch (dayTypeId)
            {
                case 1: return "Full Day";
                case 2: return "First Half";
                case 3: return "Second Half";
                default: return Convert.ToString(dayTypeId);
            }
        }
    }
}
