using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class viewuserdocuments : System.Web.UI.Page
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

                int userId;
                if (int.TryParse(Convert.ToString(Request.QueryString["user_id"]), out userId) && userId > 0)
                {
                    hfUserId.Value = userId.ToString();
                    BindUserData(userId);
                    BindUserDocuments(userId);
                    return;
                }

                hfUserId.Value = "0";
                ClearUserDisplay();
                BindNoDocumentsMessage();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "UserIdMissing",
                    "showUserSavedMessage('Error', 'Please open this page from user list View action.');", true);
            }
        }

        private void ClearUserDisplay()
        {
            txtEmployeeCode.Text = string.Empty;
            txt_name.Text = string.Empty;
            txt_fullname.Text = string.Empty;
            txt_email.Text = string.Empty;
            txt_contact.Text = string.Empty;
        }

        private void BindNoDocumentsMessage()
        {
            RepeaterAttachments.DataSource = new List<userDocumentsDO>
            {
                new userDocumentsDO
                {
                    FileName = "No document is available for this user.",
                    UserDocDetId = 0
                }
            };
            RepeaterAttachments.DataBind();
        }
        protected List<UserDetailsDO> BindgetAPIUserData(int userId)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestObj = new { user_id = userId };

                    var json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("UserList/GetUserData", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<userResponseDataDO>(jsonString);

                        if (result.Success && result.UsersList != null)
                        {
                            users = result.UsersList.Select(u => new UserDetailsDO
                            {
                                EmployeeCode = u.EmployeeCode,
                                UserId = u.UserId,
                                Username = u.Username,
                                user_fullname = u.User_fullname,
                                user_mail_id = u.user_mail_id,
                                contact_detail = u.contact_detail,
                                //roledescription=u.roledescription
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewuserdocuments", "BindgetAPIUserData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

            return users;
        }

        private void BindUserData(int userId)
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> userDetailsList = userBL.ViewAllUsers()
                    .Where(x => x.UserId == userId)
                    .ToList();

                if (userId != 0)
                {
                    if (userDetailsList.Count > 0)
                    {
                        UserDetailsDO userDetails = userDetailsList[0];

                        txtEmployeeCode.Text = userDetails.EmployeeCode;
                        txt_name.Text = userDetails.Username;
                        txt_fullname.Text = userDetails.user_fullname;
                        txt_email.Text = userDetails.user_mail_id;
                        txt_contact.Text = userDetails.contact_detail;

                        if (Request.QueryString["mode"] == "edit")
                        {
                            txt_name.Enabled = true;
                            txt_fullname.Enabled = true;
                            txt_email.Enabled = true;
                            txt_contact.Enabled = true;
                            lbluser.Text = "Update User";
                        }
                    }
                }
                else
                {
                    if (userDetailsList.Count > 0)
                    {
                        txtEmployeeCode.Text = userDetailsList[0].EmployeeCode;
                        txtEmployeeCode.Enabled = false;
                    }
                }
            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewuserdocuments", "BindUserData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }
        }

        //private void BindUserData(int userId)
        //{
        //    try
        //    {
        //        UserDetailsBL userBAL = new UserDetailsBL();
        //        List<UserDetailsDO> userDetailsList = userBAL.GetUserDetails(userId);

        //        if (userId != 0)
        //        {
        //            if (userDetailsList.Count > 0)
        //            {
        //                UserDetailsDO userDetails = userDetailsList[0];
        //                txtEmployeeCode.Text = userDetails.EmployeeCode;
        //                txt_name.Text = userDetails.Username;
        //                txt_fullname.Text = userDetails.user_fullname;
        //                txt_email.Text = userDetails.user_mail_id;
        //                txt_contact.Text = userDetails.contact_detail;
        //                //ddldesign.SelectedValue = userDetails.designation_name;
        //                //if (userDetails.designation_id != null)
        //                //{
        //                //    string desigId = userDetails.designation_id.ToString();

        //                //    ddldesign.SelectedValue = desigId;
        //                //}

        //                //txt_password.Visible = false;
        //               // lblpass.Visible = false;

        //                if (Request.QueryString["mode"] == "edit")
        //                {
        //                    txt_name.Enabled = true;
        //                    txt_fullname.Enabled = true;
        //                    txt_email.Enabled = true;
        //                    txt_contact.Enabled = true;
        //                    // ddldesign.Enabled = true;
        //                    lbluser.Text = "Update User";
        //                    //btn_submit.Text = "Update";
        //                   // btn_submit.Visible = true;
        //                    //btn_rest.Text = "Clear";
        //                    //btn_rest.Visible = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            txtEmployeeCode.Text = userDetailsList[0].EmployeeCode;
        //            txtEmployeeCode.Enabled = false;
        //        }

        //    }
        //    catch (Exception exs)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("AddUsers", "BindUserData", "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
        //    }

        //}
        protected void btnView_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (string.IsNullOrEmpty(btn.CommandArgument))
                return;

            if (!int.TryParse(btn.CommandArgument, out int docId))
                return;

            userDocumentBL docBal = new userDocumentBL();
            userDocumentsDO file = docBal.GetUserDocumentById(docId);

            if (file != null && !string.IsNullOrEmpty(file.filepath))
            {
                string physicalPath = file.filepath;

                if (System.IO.File.Exists(physicalPath))
                {
                    string ext = System.IO.Path.GetExtension(physicalPath).ToLower();
                    string contentType;

                    switch (ext)
                    {
                        case ".pdf":
                            contentType = "application/pdf";
                            break;
                        case ".jpg":
                        case ".jpeg":
                            contentType = "image/jpeg";
                            break;
                        case ".png":
                            contentType = "image/png";
                            break;
                        case ".gif":
                            contentType = "image/gif";
                            break;
                        default:
                            contentType = "application/octet-stream"; // fallback
                            break;
                    }

                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("Content-Disposition", "inline;filename=" + System.IO.Path.GetFileName(physicalPath));
                    Response.WriteFile(physicalPath);
                    Response.End();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('File not found');", true);
                }
            }
        }




        private void BindUserDocuments(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    BindNoDocumentsMessage();
                    return;
                }

                userDocumentBL docBal = new userDocumentBL();
                List<userDocumentsDO> lstDocs = docBal.GetUserDocuments(userId);

                if (lstDocs != null && lstDocs.Count > 0)
                {
                    RepeaterAttachments.DataSource = lstDocs;
                    RepeaterAttachments.DataBind();
                }
                else
                {
                    BindNoDocumentsMessage();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewUser", "BindUserDocuments",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;

                
                if (!int.TryParse(button.CommandArgument, out int userDocId))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "AlertScript",
                        "Swal.fire({ title: 'Failed!', text: 'Invalid document ID', icon: 'error', confirmButtonText: 'OK' });", true);
                    return;
                }

                
                int userId = 0;
                if (!string.IsNullOrEmpty(Request.QueryString["user_id"]))
                {
                    int.TryParse(Request.QueryString["user_id"], out userId);
                }

                if (userId == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "AlertScript",
                        "Swal.fire({ title: 'Failed!', text: 'Invalid user ID', icon: 'error', confirmButtonText: 'OK' });", true);
                    return;
                }

              
                userDocumentBL docBL = new userDocumentBL();
                var doc = docBL.GetUserDocumentById(userDocId); 

                if (doc != null && File.Exists(doc.filepath))
                {
                    FileInfo fileInfo = new FileInfo(doc.filepath);
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={fileInfo.Name}");
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.TransmitFile(doc.filepath);
                    Response.Flush();
                    Response.End();
                }

                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "AlertScript",
                        "Swal.fire({ title: 'Failed!', text: 'File Not Found', icon: 'error', confirmButtonText: 'OK' });", true);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ViewUser", "btnDownload_Click",
                    "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);

                ClientScript.RegisterStartupScript(this.GetType(), "AlertScript",
                    "Swal.fire({ title: 'Error!', text: 'An unexpected error occurred', icon: 'error', confirmButtonText: 'OK' });", true);
            }
        }

 

        protected void confirmDeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                int UserDocDetId = 0;
                if (ViewState["DeleteDocId"] != null)
                {
                    UserDocDetId = Convert.ToInt32(ViewState["DeleteDocId"]);
                }

                userDocumentBL docBL = new userDocumentBL();
                userDocumentsDO document = docBL.DeactivateDocument(UserDocDetId);

                if (document.Status == "Success")
                {
                    int userId = Convert.ToInt32(hfUserId.Value);
                    BindUserDocuments(userId);


                    string status = document.Status;
                    string remark = document.Remarks;
                    ClientScript.RegisterStartupScript(this.GetType(),
                        "RightsSavedScript",
                        "showUserSavedMessage('" + status + "', '" + remark + "');", true);
                }
                else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(),
                            "alert", "showUserSavedMessage('Error', 'Failed to delete document');", true);
                    }
                
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "alert", $"showUserSavedMessage('Error', 'Error while deleting: {ex.Message}');", true);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int docId = Convert.ToInt32(btn.CommandArgument);

          
            ViewState["DeleteDocId"] = docId;

          
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "openDeleteModal", "openDeleteModal();", true);
        }




        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/useruploaddocuments.aspx", false);
        }

        private int GetClientIdFromSession()
        {
            int userId = 0;
            if (Session["UserId"] != null)
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            return userId;
        }
    }
}
