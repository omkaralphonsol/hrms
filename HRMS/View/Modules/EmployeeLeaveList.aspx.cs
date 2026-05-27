using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class EmployeeLeaveList : System.Web.UI.Page
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

                BindGridView();
            }
        }

        protected void BindGridView()
        {
            try
            {
                EmployeeLeaveBL leaveBL = new EmployeeLeaveBL();
                List<EmployeeLeaveListDO> leaves = leaveBL.GetAllLeaveRequestsForHr();
                gridview.DataSource = leaves;
                gridview.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveList", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "viewLeave")
                {
                    string[] args = Convert.ToString(e.CommandArgument).Split('|');
                    string leaveId = args.Length > 0 ? args[0] : "0";
                    Response.Redirect("EmployeeLeaveView.aspx?leaveId=" + HttpUtility.UrlEncode(leaveId) + "&mode=view", false);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveList", "gridview_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}
