using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class adduserdocuments : System.Web.UI.Page
    {
        protected string UserId = null;
       // private object uploadedFiles;
        private static List<FileAttachment> uploadedFiles = new List<FileAttachment>();
        private static List<FileAttachment> ViewFiles = new List<FileAttachment>();

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
                //BindUserData(userId);
                BindDocuments();
            }
        }

        public void BindDocuments()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownDocuments();
                if (list1 != null && list1.Count > 0)
                {
                    ddldocument.DataSource = list1;
                    ddldocument.DataTextField = "Text";
                    ddldocument.DataValueField = "Id";
                }
                else
                {
                    ddldocument.DataSource = null;
                }

                ddldocument.DataBind();
                ddldocument.Items.Insert(0, new ListItem("-- Please Select --", ""));

                // Optional: Set default selected value (if required)
                //string defaultValue = "17"; // e.g., Technician
                //ListItem item = ddldesign.Items.FindByValue(defaultValue);
                //if (item != null)
                //{
                //    ddldesign.SelectedValue = defaultValue;
                //}
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduserdocuments", "BindDocuments", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessage.Text = "";
                string selectedId = ddldocument.SelectedValue;
                string referencenumber = txt_reference.Text;
                string emailID = txt_email.Text;

                if (string.IsNullOrEmpty(selectedId))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
                        "showUserSavedMessage('error', 'Document is required');", true);

                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                if (string.IsNullOrEmpty(referencenumber))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
                        "showUserSavedMessage('error', 'Reference Number is required');", true);

                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (string.IsNullOrEmpty(emailID))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
                        "showUserSavedMessage('error', 'Email is required');", true);

                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (fileUpload.HasFiles)
                {
                    int maxFileSize = 5 * 1024 * 1024; 
                    string basepath = @"D:\documents\";

                    if (!Directory.Exists(basepath))
                        Directory.CreateDirectory(basepath);

                    List<FileAttachment> uploadedFiles = Session["UploadedFiles"] as List<FileAttachment> ?? new List<FileAttachment>();
                    List<FileAttachment> ViewFiles = Session["ViewUploadedFiles"] as List<FileAttachment> ?? new List<FileAttachment>();

                    string userIdStr = Request.QueryString["user_id"];
                    if (string.IsNullOrEmpty(userIdStr))
                    {
                        lblMessage.Text = "User ID is missing in URL.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                    int userId = Convert.ToInt32(userIdStr);

                    userDocumentBL userDocBL = new userDocumentBL();

                    foreach (HttpPostedFile uploadedFile in fileUpload.PostedFiles)
                    {
                        try
                        {
                            if (uploadedFile.ContentLength > maxFileSize)
                            {
                                lblMessage.Text = "File must be less than 5MB.";
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                                return;
                            }

                            string originalFileName = Path.GetFileName(uploadedFile.FileName);
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                            string fileExt = Path.GetExtension(originalFileName);
                            string filePath = Path.Combine(basepath, originalFileName);

                            if (!uploadedFiles.Any(f => f.FileName.Equals(fileNameWithoutExt, StringComparison.OrdinalIgnoreCase)))
                            {
                                
                                uploadedFile.SaveAs(filePath);

                              
                                FileAttachment file = new FileAttachment
                                {
                                    FileName = fileNameWithoutExt,
                                    FilePath = filePath,           
                                    DocumentMasterId = Convert.ToInt32(selectedId),
                                    ReferenceNumber=referencenumber,
                                    EmailId=emailID

                                };

                               
                                userDocBL.SaveUserDocument(userId, file, fileExt, basepath);

                            
                                uploadedFiles.Add(file);
                                ViewFiles.Add(new FileAttachment
                                {
                                    FileName = originalFileName, 
                                    FilePath = "/documents/" + originalFileName
                                });
                            }
                        }
                        catch (Exception fileEx)
                        {
                            lblMessage.Text = "Error uploading file: " + uploadedFile.FileName + " - " + fileEx.Message;
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                    }

                    Session["UploadedFiles"] = uploadedFiles;
                    Session["ViewUploadedFiles"] = ViewFiles;

                    ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
                        "showUserSavedMessage('Success', 'Files uploaded successfully!');" +
                                "setTimeout(function(){ window.location.href = 'useruploaddocuments.aspx'; }, 3000);", true);

                  
                    // BindGrid();
                }
                else
                {
                    lblMessage.Text = "No files selected to upload.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("adduserdocuments", "btnUpload_Click",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }
        //protected void gvAttachments_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "DeleteFile")
        //    {
        //        try
        //        {
        //            int fileId = Convert.ToInt32(e.CommandArgument);

        //            FileAttachment fileToRemove = uploadedFiles.FirstOrDefault(f => f.FileId == fileId);
        //            if (fileToRemove != null)
        //            {
        //                uploadedFiles.Remove(fileToRemove);

        //                string filePath = Server.MapPath(fileToRemove.FilePath);
        //                if (File.Exists(filePath))
        //                {
        //                    File.Delete(filePath);
        //                }

        //                BindGrid();
        //            }

        //            FileAttachment fileToRemove1 = ViewFiles.FirstOrDefault(f => f.FileId == fileId);
        //            if (fileToRemove1 != null)
        //            {
        //                ViewFiles.Remove(fileToRemove1);

        //                string filePath = Server.MapPath(fileToRemove1.FilePath);
        //                if (File.Exists(filePath))
        //                {
        //                    File.Delete(filePath);
        //                }

        //                BindGrid();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            CommonBL errorlog = new CommonBL();
        //            errorlog.fnStoreErrorLog("RaiseTicket", "gvAttachments_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //        }
        //    }
        //}
        //private void BindGrid()
        //{
        //    try
        //    {
        //        List<FileAttachment> viewFiles = Session["ViewUploadedFiles"] as List<FileAttachment> ?? new List<FileAttachment>();
        //        gvAttachments.DataSource = viewFiles;
        //        gvAttachments.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("RaiseTicket", "BindGrid", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

        //    }
        //}

        //protected void SubmitButtonClick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Get user_id from query string
        //        string userIdStr = Request.QueryString["user_id"];
        //        if (string.IsNullOrEmpty(userIdStr))
        //        {
        //            lblMessage.Text = "User ID is missing in URL.";
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //            return;
        //        }

        //        int userId = Convert.ToInt32(userIdStr); // convert to int

        //        // Get selected document type
        //        string selectedId = ddldocument.SelectedValue;
        //        if (string.IsNullOrEmpty(selectedId))
        //        {
        //            lblMessage.Text = "Please select a document type.";
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //            return;
        //        }

        //        if (Session["UploadedFiles"] != null)
        //        {
        //            List<FileAttachment> files = (List<FileAttachment>)Session["UploadedFiles"];
        //            userDocumentBL userDocBL = new userDocumentBL();

        //            foreach (var file in files)
        //            {
        //                try
        //                {
        //                    file.DocumentMasterId = Convert.ToInt32(selectedId);

        //                    // Keep full path
        //                    // file.FilePath already contains full path

        //                    userDocBL.SaveUserDocument(userId, file);
        //                }
        //                catch (Exception fileEx)
        //                {
        //                    CommonBL errorlog = new CommonBL();
        //                    errorlog.fnStoreErrorLog("addUserdocuments", "SubmitButtonClick_File",
        //                        "File: " + file.FileName + " Exception: " + fileEx.Message, UserId);
        //                }
        //            }
        //            Session["UploadedFiles"] = null;
        //            Session["ViewUploadedFiles"] = null;
        //            ClientScript.RegisterStartupScript(this.GetType(), "FileSavedScript",
        //                "showUserSavedMessage('Success', 'Files uploaded successfully!');" +
        //                "setTimeout(function(){ window.location.href = 'useruploaddocuments.aspx'; }, 4000);", true);
        //        }
        //        else
        //        {
        //            lblMessage.Text = "No files to save.";
        //            lblMessage.ForeColor = System.Drawing.Color.Red;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("addUserdocuments", "SubmitButtonClick",
        //            "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
        //    }
        //}

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/useruploaddocuments.aspx", false);
        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                ddldocument.SelectedIndex = 0;
                fileUpload.Attributes.Clear();
                uploadedFiles.Clear();
                ViewFiles.Clear();

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUser", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
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