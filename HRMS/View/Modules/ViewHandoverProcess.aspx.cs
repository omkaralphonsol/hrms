using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ViewHandoverProcess : System.Web.UI.Page
    {
        protected int ResignationId;
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
                //if (Request.QueryString["rid"] == null)
                //{
                //    Response.Redirect("HandoverProcessList.aspx");
                //    return;
                //}

                //ResignationId = Convert.ToInt32(Request.QueryString["rid"]);
                //hfResignationId.Value = ResignationId.ToString();
                if (Request.QueryString["rid"] == null || Request.QueryString["uid"] == null)
                {
                    Response.Redirect("HandoverProcessList.aspx");
                    return;
                }

                int resignationId = Convert.ToInt32(Request.QueryString["rid"]);
                int userIDs = Convert.ToInt32(Request.QueryString["uid"]);

                hfResignationId.Value = resignationId.ToString();
                hfUserId.Value = userIDs.ToString(); 

                   LoadExistingHandover(); 
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!(chkPendriveBackup.Checked || chkLaptopCharger.Checked || chkContactDetails.Checked || chkDiary.Checked || chkIdCard.Checked))
            {
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "Validation",
                    "showSavedMessage('Error','Please select at least one item to proceed.');",
                    true
                );
                return; 
            }
            int resignationId = Convert.ToInt32(hfResignationId.Value);
            int userIds = Convert.ToInt32(hfUserId.Value);

            int UserId = Convert.ToInt32(Session["userId"]);

            HandoverProcessDO obj = new HandoverProcessDO
            {
                EmployeeResignationId = resignationId,
                UserId = userIds,

                PendriveBackup = chkPendriveBackup.Checked,
                LaptopWithCharger = chkLaptopCharger.Checked,
                ContactDetailsShared = chkContactDetails.Checked,
                DiarySubmitted = chkDiary.Checked,
                IDCard = chkIdCard.Checked,
                HR_Remark = txtRemark.Text.Trim(),
                //Status = status,
                InsertedBy = UserId
            };

            HandoverprocessBL bl = new HandoverprocessBL();
            bl.SaveHandoverProcess(obj);

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "Saved",
                "showSavedMessage('Success','Handover saved successfully');",
                true
            );
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            chkPendriveBackup.Checked = false;
            chkLaptopCharger.Checked = false;
            chkContactDetails.Checked = false;
            chkDiary.Checked = false;
            chkIdCard.Checked = false;
            txtRemark.Text = string.Empty;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/HandoverProcess.aspx", false);
        }

        private void LoadExistingHandover()
        {
            int resignationId = Convert.ToInt32(hfResignationId.Value);

            HandoverprocessBL bl = new HandoverprocessBL();
            HandoverProcessDO data = bl.GetHandoverByResignationId(resignationId);

            if (data != null)
            {
                chkPendriveBackup.Checked = data.PendriveBackup;
                chkLaptopCharger.Checked = data.LaptopWithCharger;
                chkContactDetails.Checked = data.ContactDetailsShared;
                chkDiary.Checked = data.DiarySubmitted;
                txtRemark.Text = data.HR_Remark;
                chkIdCard.Checked = Convert.ToInt32(data.IDCard) == 1;



                //// Optional: lock UI if already approved
                //if (data.Status == "Approved")
                //{
                //    DisableControls();
                //}
            }
        }
        private void DisableControls()
        {
            chkPendriveBackup.Enabled = false;
            chkLaptopCharger.Enabled = false;
            chkContactDetails.Enabled = false;
            chkDiary.Enabled = false;

            //txtRemark.ReadOnly = true;

            //btnApprove.Visible = false;
            //btnReject.Visible = false;
        }


    }
}