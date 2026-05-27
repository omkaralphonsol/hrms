using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Element;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Geom;
using Image = iText.Layout.Element.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;
using Table = iText.Layout.Element.Table;
using iText.Kernel.Pdf.Canvas;



namespace HRMS.View.Modules
{
    public partial class updateprobationperiodflag : System.Web.UI.Page
    {
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

                BindUserData(userId);
            }
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
                errorlog.fnStoreErrorLog("updateprobationperiodflag", "BindgetAPIUserData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

            return users;
        }

        private void BindUserData(int userId)
        {
            try
            {
                List<UserDetailsDO> userDetailsList = BindgetAPIUserData(userId);

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
                            //txt_name.Enabled = true;
                            //txt_fullname.Enabled = true;
                            //txt_email.Enabled = true;
                            //txt_contact.Enabled = true;
                            lbluser.Text = "Update Probation Period";
                            btn_submit.Text = "Update";

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
                errorlog.fnStoreErrorLog("updateprobationperiodflag", "BindUserData",
                    "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/viewdetailsforprobationperiod.aspx", false);
        }

        //protected void SubmitButtonClick(object sender, EventArgs e)
        //{
        //    UserDetailsDO user = new UserDetailsDO();
        //    ResponseDO response = new ResponseDO();
        //    try
        //    {


        //         if (btn_submit.Text == "Update")
        //        {
        //            user.UserId = Convert.ToInt32(Request.QueryString["user_id"]);
        //            UserDetailsBL userdetails = new UserDetailsBL();
        //            user.EmployeeCode = txtEmployeeCode.Text;
        //            user.Username = txt_name.Text;
        //            user.user_fullname = txt_fullname.Text;
        //            user.designation_id = Convert.ToInt32(ddlprobationflag.SelectedValue);
        //            user.user_mail_id = txt_email.Text;
        //            user.contact_detail = txt_contact.Text;
        //            //user.password = txt_password.Text;
        //            List<UserDetailsDO> userdata = CallUpdateProbationFlagAPI(user);
        //            if (userdata[0].Status == "Success")
        //            {
        //                string status = userdata[0].Status;
        //                string remark = userdata[0].Remarks;
        //                ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
        //                          "showUserSavedMessage('" + status + "', '" + remark + "');" +
        //                          "setTimeout(function(){ window.location.href = 'ViewUser.aspx'; }, 4000);", true);
        //                return;
        //            }
        //            else
        //            {
        //                string status = userdata[0].Status;
        //                string remark = userdata[0].Remarks;
        //                ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showUserSavedMessage('" + status + "', '" + remark + "');", true);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("updateprobationperiodflag", "SubmitButtonClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //}

        protected void SubmitButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (btn_submit.Text == "Update")
                {
                    int userId = Convert.ToInt32(Request.QueryString["user_id"]);
                    int probationFlag = Convert.ToInt32(ddlprobationflag.SelectedValue);
                    string Remark = txtRemark.Text;
                    string DateOfExtended = txtextendeddate.Text;
                    if (probationFlag == 0)
                    {
                        CallUpdateAPI(userId, probationFlag, Remark, DateOfExtended);
                        return;
                    }

                    // Get probation date
                    userDateResponseDataDO probationResponse = CallGetProbationDateAPI(userId);

                    if (!probationResponse.Success || probationResponse.UsersprobationperiodDateList == null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "msg",
                            "showUserSavedMessage('Failed','No probation data found.');", true);
                        return;
                    }

                    var record = probationResponse.UsersprobationperiodDateList.FirstOrDefault();

                    if (record == null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "msg",
                            "showUserSavedMessage('Failed','No record found.');", true);
                        return;
                    }

                    // CASE 1 → 6 MONTH COMPLETED = YES → Direct Update
                    if (record.SixMonthCompleted == 1)
                    {
                        CallUpdateAPI(userId, probationFlag, Remark, DateOfExtended);
                        return;
                    }

                    // CASE 2 → NOT COMPLETED → Show popup
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                 "openDeleteModal", "openDeleteModal();", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("updateprobationperiodflag", "SubmitButtonClick",
                    "Exception Message=" + ex.Message + " Trace=" + ex.StackTrace, UserId);
            }
        }


        protected void ddlprobationflag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlprobationflag.SelectedValue == "0")  // NO
            {
                remarkBox.Visible = true;
                RequiredFieldValidatorRemark.Enabled = true;
                divExtendedSection.Visible = true;
            }
            else
            {
                remarkBox.Visible = false;
                RequiredFieldValidatorRemark.Enabled = false;
                txtRemark.Text = "";
                divExtendedSection.Visible = false;
            }
        }


        protected void btnProbationYes_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Request.QueryString["user_id"]);

            var empTypeResult = CallGetEmployeeTypeAPI(userId);

            if (!empTypeResult.Success ||
                empTypeResult.EmployeeTypeList == null ||
                empTypeResult.EmployeeTypeList.Count == 0)
            {
                ShowMessage("Employee Type is not Found.", "error");
                return;
            }

            string empType =
                empTypeResult.EmployeeTypeList[0].employee_type;

            int probationFlag = Convert.ToInt32(ddlprobationflag.SelectedValue);

            string remark = probationFlag == 0 ? txtRemark.Text.Trim() : null;
            string dateOfExtended = probationFlag == 0 ? txtextendeddate.Text.Trim() : null;

            CallUpdateAPI(userId, probationFlag, remark, dateOfExtended);

            string employeeName = txt_fullname.Text;
            string hrEmail = "sakshi.shewale@alphonsol.com";
            string confirmationDate = DateTime.Now.ToString("dd-MM-yyyy");

            byte[] pdfBytes = null; // Declare first

            if (empType == "Internship")
            {
                pdfBytes = GenerateConfirmationLetter(employeeName, confirmationDate);
            }
            else if (empType == "Experience")
            {
                pdfBytes = GenerateConfirmationLetterofExperience(employeeName, confirmationDate);
            }
            else
            {
                ShowMessage("Invalid employee type.", "error");
                return;
            }
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                ShowMessage("PDF generation failed.", "error");
                return;
            }

            SendConfirmationLetterEmail(hrEmail, employeeName, pdfBytes);

            ShowMessage("Confirmation Letter Sent Successfully.", "Success");
        }
        //business analyst logic
        //protected void btnProbationYes_Click(object sender, EventArgs e)
        //{
        //    int userId = Convert.ToInt32(Request.QueryString["user_id"]);

        //    // 🔹 Get Employee Type
        //    var empTypeResult = CallGetEmployeeTypeAPI(userId);

        //    if (!empTypeResult.Success ||
        //        empTypeResult.EmployeeTypeList == null ||
        //        empTypeResult.EmployeeTypeList.Count == 0)
        //    {
        //        ShowMessage("Employee Type is not Found.", "error");
        //        return;
        //    }

        //    string empType = empTypeResult.EmployeeTypeList[0].employee_type;

        //    // 🔹 Get Designation
        //    var empList = BindgetAPIEmployeeCodeAndDesignationData(userId);

        //    if (empList == null || empList.Count == 0)
        //    {
        //        ShowMessage("Employee designation not found.", "error");
        //        return;
        //    }

        //    string designation = empList[0].designation?.Trim();

        //    int probationFlag = Convert.ToInt32(ddlprobationflag.SelectedValue);

        //    string remark = probationFlag == 0 ? txtRemark.Text.Trim() : null;
        //    string dateOfExtended = probationFlag == 0 ? txtextendeddate.Text.Trim() : null;

        //    // 🔹 Update probation
        //    CallUpdateAPI(userId, probationFlag, remark, dateOfExtended);

        //    string employeeName = txt_fullname.Text;
        //    string hrEmail = "sakshi.shewale@alphonsol.com";
        //    string confirmationDate = DateTime.Now.ToString("dd-MM-yyyy");

        //    byte[] pdfBytes = null;

        //    // 🔹 PDF Generation Logic
        //    if (empType == "Internship")
        //    {
        //        pdfBytes = GenerateConfirmationLetter(employeeName, confirmationDate);
        //    }
        //    else if (empType == "Experience")
        //    {
        //        if (designation.Equals("Business Coordinator", StringComparison.OrdinalIgnoreCase))
        //        {
        //            pdfBytes = GenerateConfirmationLetterofBusinessAnalystExp(
        //                employeeName, confirmationDate);
        //        }
        //        else
        //        {
        //            pdfBytes = GenerateConfirmationLetterofExperience(
        //                employeeName, confirmationDate);
        //        }
        //    }
        //    else
        //    {
        //        ShowMessage("Invalid employee type.", "error");
        //        return;
        //    }

        //    if (pdfBytes == null || pdfBytes.Length == 0)
        //    {
        //        ShowMessage("PDF generation failed.", "error");
        //        return;
        //    }

        //    SendConfirmationLetterEmail(hrEmail, employeeName, pdfBytes);

        //    ShowMessage("Confirmation Letter Sent Successfully.", "Success");
        //}


        private void ShowMessage(string msg, string type)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "alert",
                $"showUserSavedMessage('{type}','{msg}');",
                true
            );
        }
        //protected List<userEmployeecodeanddesignDO> BindgetAPIEmployeeCodeAndDesignationData(int userId)
        //{
        //    List<userEmployeecodeanddesignDO> users = new List<userEmployeecodeanddesignDO>();
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("http://103.118.17.144:813/");
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new MediaTypeWithQualityHeaderValue("application/json"));

        //            var requestObj = new { user_id = userId };
        //            var json = new JavaScriptSerializer().Serialize(requestObj);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response =
        //                client.PostAsync("UserList/GetEmployeeCodeAndDesignation", content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var jsonString = response.Content.ReadAsStringAsync().Result;
        //                JavaScriptSerializer js = new JavaScriptSerializer();

        //                var result =
        //                    js.Deserialize<userEmployeecodeanddesigresponseDO>(jsonString);

        //                if (result.Success && result.EmployeeCodeAndDesignationList != null)
        //                {
        //                    users = result.EmployeeCodeAndDesignationList.Select(u =>
        //                        new userEmployeecodeanddesignDO
        //                        {
        //                            EmployeeCode = u.EmployeeCode,
        //                            UserId = u.UserId,
        //                            designation = u.designation,
        //                            company_id = u.company_id
        //                        }).ToList();

        //                    if (users.Count > 0)
        //                    {
        //                        HttpContext.Current.Session["CompanyId"] = users[0].company_id;
        //                        HttpContext.Current.Session["CompanyIdsList"] =
        //                            users.Select(x => x.company_id).ToList();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exs)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog(
        //            "updateprobationperiodflag",
        //            "BindgetAPIEmployeeCodeAndDesignationData",
        //            "Exception Message: " + exs.Message +
        //            " StackTrace: " + exs.StackTrace,
        //            UserId);
        //    }

        //    return users;
        //}
        //private byte[] GenerateConfirmationLetterofBusinessAnalystExp(string employeeName, string confirmationDate)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        PdfWriter writer = new PdfWriter(ms);
        //        PdfDocument pdf = new PdfDocument(writer);
        //        Document document = new Document(pdf, PageSize.A4);
        //        document.SetMargins(35, 35, 60, 35);

        //        PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        //        PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        //        /* ================= HEADER (NO CHANGE) ================= */

        //        float[] headerWidths = { 1, 2 };
        //        Table headerTable = new Table(UnitValue.CreatePercentArray(headerWidths))
        //            .UseAllAvailableWidth();

        //        Image logo = new Image(ImageDataFactory.Create(
        //            Server.MapPath("../../assets/images/alphonsol_logo.png")))
        //            .SetWidth(150);

        //        Table logoRowTable = new Table(new float[] { 4, 2 })
        //            .UseAllAvailableWidth()
        //            .SetBorder(Border.NO_BORDER);

        //        logoRowTable.AddCell(
        //            new Cell().Add(logo)
        //                .SetBorder(Border.NO_BORDER)
        //                .SetTextAlignment(TextAlignment.LEFT));

        //        logoRowTable.AddCell(
        //            new Cell().Add(new Paragraph("Pvt.\u00A0Ltd.")
        //                .SetFont(bold).SetFontSize(10))
        //                .SetBorder(Border.NO_BORDER)
        //                .SetTextAlignment(TextAlignment.RIGHT)
        //                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        //        Paragraph cinPara = new Paragraph()
        //            .Add(new Text("CIN No - ").SetFont(bold).SetFontSize(10))
        //            .Add(new Text("U722200MH2022PTC381560").SetFont(normal).SetFontSize(10));

        //        Cell logoCell = new Cell()
        //            .Add(logoRowTable)
        //            .Add(cinPara)
        //            .SetBorder(Border.NO_BORDER);

        //        Paragraph rightAddress = new Paragraph()
        //            .Add("High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601")
        //            .Add("\nContact No - 9920393999")
        //            .Add("\nEmail Address - support@alphonsol.com")
        //            .Add("\nWebsite - www.alphonsol.com")
        //            .SetFont(bold)
        //            .SetFontSize(9)
        //            .SetTextAlignment(TextAlignment.RIGHT);

        //        Cell rightCell = new Cell()
        //            .Add(rightAddress)
        //            .SetBorder(Border.NO_BORDER)
        //            .SetVerticalAlignment(VerticalAlignment.MIDDLE);

        //        headerTable.AddCell(logoCell);
        //        headerTable.AddCell(rightCell);

        //        document.Add(headerTable);

        //        SolidLine orangeLine = new SolidLine(2);
        //        orangeLine.SetColor(ColorConstants.ORANGE);
        //        document.Add(new LineSeparator(orangeLine)
        //            .SetMarginTop(5)
        //            .SetMarginBottom(25));

        //        /* ================= BELOW HEADER (UPDATED) ================= */

        //        // TITLE
        //        document.Add(new Paragraph("Experience Certificate")
        //            .SetFont(bold)
        //            .SetFontSize(16)
        //            .SetTextAlignment(TextAlignment.CENTER)
        //            .SetMarginBottom(25));

        //        // DATE + REF NO
        //        Table dateRefTable = new Table(new float[] { 1, 1 })
        //            .UseAllAvailableWidth();

        //        dateRefTable.AddCell(new Cell()
        //            .Add(new Paragraph("Date: " + confirmationDate)
        //            .SetFont(bold))
        //            .SetBorder(Border.NO_BORDER)
        //            .SetTextAlignment(TextAlignment.LEFT));

        //        dateRefTable.AddCell(new Cell()
        //            .Add(new Paragraph("Ref No: APS.EL.01.928")
        //            .SetFont(bold))
        //            .SetBorder(Border.NO_BORDER)
        //            .SetTextAlignment(TextAlignment.RIGHT));

        //        document.Add(dateRefTable);

        //        // TO WHOM IT MAY CONCERN
        //        document.Add(new Paragraph("To Whom It May Concern,")
        //            .SetFont(bold)
        //            .SetUnderline()
        //            .SetMarginTop(25)
        //            .SetMarginBottom(15));

        //        // BODY
        //        // Paragraph 1 – Employment Confirmation
        //        document.Add(new Paragraph(
        //            $"This is to certify that {employeeName} was employed with Alphonsol Pvt. Ltd. as a " +
        //            $"Business Analyst from dateofjoining to lastworkingdate.")
        //            .SetFont(normal)
        //            .SetFontSize(12)
        //            .SetTextAlignment(TextAlignment.JUSTIFIED)
        //            .SetMarginBottom(12));

        //        document.Add(new Paragraph(
        //    "During the tenure with us, the employee was responsible for gathering, analyzing, and " +
        //    "documenting business requirements, conducting market research, feasibility studies, and " +
        //    "gap analyses. The role also involved collaborating with cross-functional teams including " +
        //    "Product, Engineering, Quality Assurance, and Operations, along with supporting project " +
        //    "planning, estimation, and risk assessment activities. " +
        //    $"{employeeName} consistently demonstrated strong analytical and problem-solving skills " +
        //    "and played a key role in effectively bridging business needs with technical solutions.")
        //    .SetFont(normal)
        //    .SetFontSize(12)
        //    .SetTextAlignment(TextAlignment.JUSTIFIED)
        //    .SetMarginBottom(12));

        //        document.Add(new Paragraph(
        //        $"{employeeName} was a valued member of our Business Analyst team and played a key role " +
        //        "in ensuring project success by maintaining accuracy and thoroughness " +
        //        "in her/his work.")
        //        .SetFont(normal)
        //        .SetFontSize(12)
        //        .SetTextAlignment(TextAlignment.JUSTIFIED)
        //        .SetMarginBottom(12));
        //        // Paragraph 4 – Closing
        //        document.Add(new Paragraph(
        //            "We thank the employee for the valuable contributions made to the organization and " +
        //            "wish continued success in all future professional endeavors.")
        //            .SetFont(normal)
        //            .SetFontSize(12)
        //            .SetTextAlignment(TextAlignment.JUSTIFIED)
        //            .SetMarginBottom(30));

        ////// REGARDS
        ////document.Add(new Paragraph("Regards,")
        ////    .SetFont(bold)
        ////    .SetMarginBottom(5));

        ////document.Add(new Paragraph("Meera Sawant")
        ////    .SetFont(bold));

        ////document.Add(new Paragraph("Human Resources Department.")
        ////    .SetFont(bold));

        ////document.Add(new Paragraph("Alphonsol Private Ltd.")
        ////    .SetFont(bold)
        ////    .SetFontColor(new DeviceRgb(230, 92, 0)));


        //Paragraph signPara = new Paragraph()
        //    .Add(new Text("Regards,\n")
        //        .SetFont(bold)
        //        .SetFontSize(10))

        //     .Add(new Text("Meera Sawant,\n")
        //        .SetFont(bold)
        //        .SetFontSize(10))

        //    .Add(new Text("Human Resources Department,\n")
        //        .SetFont(bold)
        //        .SetFontSize(10))

        //    .Add(new Text("Alphonsol Pvt. Ltd.")
        //        .SetFont(bold)
        //        .SetFontSize(10)
        //        .SetFontColor(new DeviceRgb(230, 92, 0)))

        //    .SetMarginTop(25);

        //document.Add(signPara);


        //        SolidLine footerLine = new SolidLine(14);
        //footerLine.SetColor(new DeviceRgb(255, 140, 0));

        //        document.Add(new LineSeparator(footerLine)
        //            .SetMarginTop(40));   // adjust spacing if needed

        //        Paragraph addressPara = new Paragraph(
        //                "Registered Address: E-1, Grand Square, Flat No. 104, Anand Nagar, G B Road, Thane (W) 400607"
        //            )
        //            .SetFont(bold)
        //            .SetFontSize(9)
        //            .SetTextAlignment(TextAlignment.LEFT)   // ⬅ LEFT CORNER
        //            .SetMarginTop(6)
        //            .SetFontColor(ColorConstants.BLACK);
        //document.Add(addressPara);

        //        document.Close();
        //        return ms.ToArray();
        //    }
        //}


        private byte[] GenerateConfirmationLetterofExperience(string employeeName, string confirmationDate)
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);
                document.SetMargins(35, 35, 60, 35);


                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // HEADER TABLE
                float[] headerWidths = { 1, 2 };
                //float[] headerWidths = { 1, 2.8f };

                Table headerTable = new Table(UnitValue.CreatePercentArray(headerWidths))
                    .UseAllAvailableWidth();

                // LEFT SIDE 
                Image logo = new Image(ImageDataFactory.Create(
                    Server.MapPath("../../assets/images/alphonsol_logo.png")))
                    .SetWidth(150);
                Table logoRowTable = new Table(new float[] { 4, 2 });
                logoRowTable.SetWidth(UnitValue.CreatePercentValue(100));
                logoRowTable.SetBorder(Border.NO_BORDER);

                // LOGO
                logoRowTable.AddCell(
                    new Cell()
                        .Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT)
                );

                logoRowTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Pvt.\u00A0Ltd.")
                            .SetFont(bold)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.RIGHT))
                        .SetBorder(Border.NO_BORDER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                Paragraph cinPara = new Paragraph()
                    .Add(new Text("CIN No - ").SetFont(bold).SetFontSize(10))
                    .Add(new Text("U722200MH2022PTC381560").SetFont(normal).SetFontSize(10))
                    .SetMarginTop(5);

                Cell logoCell = new Cell()
                    .Add(logoRowTable)
                    .Add(cinPara)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(0);

                //  RIGHT SIDE 
                Paragraph rightAddress = new Paragraph()
                    .Add(new Text(
                        "High-street\u00A0Corporate\u00A0Center,\u00A0FB-03,\u00A0Kapurbawdi\u00A0Junction,\u00A0Thane(W)\u00A0400601"
                    ))
                    .Add("\nContact No - 9920393999")
                    .Add("\nEmail Address - support@alphonsol.com")
                    .Add("\nWebsite - www.alphonsol.com")
                    .SetFont(bold)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT);


                Cell rightCell = new Cell()
                    .Add(rightAddress)
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                headerTable.AddCell(logoCell);
                headerTable.AddCell(rightCell);

                document.Add(headerTable);

                // ORANGE LINE
                SolidLine orangeLine = new SolidLine(2);
                orangeLine.SetColor(ColorConstants.ORANGE);
                document.Add(new LineSeparator(orangeLine)
                    .SetMarginTop(5)
                    .SetMarginBottom(15));

                // TITLE

                document.Add(new Paragraph(
                    new Text("Confirmation Letter")
                        .SetFont(bold)
                        .SetFontSize(16)
                        .SetUnderline(1, -3))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // DATE
                document.Add(new Paragraph("Date: " + confirmationDate)
                    .SetFont(bold)
                    .SetFontSize(12));

                // GREETING
                document.Add(new Paragraph("Dear " + employeeName + ",")
                    .SetFont(bold)
                    .SetFontSize(12)
                    .SetMarginBottom(10));

                //  BODY TEXT  

                Paragraph para = new Paragraph()
     .Add(new Text(
         "We are pleased to inform you that following the successful completion of your 6-month probation with Alphonsol Pvt Ltd, we are delighted to confirm your transition into a full-time employee of the organization from "
         + confirmationDate + "."
     ))
     .Add("\n") // New line
     .Add(new Text("Congratulations on this well-earned achievement!"))
     .Add("\n") 
     .Add(new Text(
         "We appreciate your hard work, dedication, and the valuable contributions you have made during your training period, and we look forward to your continued success and growth as part of our team."
     ))
     .SetFont(normal)
     .SetFontSize(11)
     .SetTextAlignment(TextAlignment.JUSTIFIED)
     .SetMultipliedLeading(1.4f)
     .SetMarginBottom(20);

                document.Add(para);



                //LEAVE BENEFITS

                document.Add(new Paragraph("1.   Leave Benefits:")
                    .SetFont(bold)
                    .SetFontSize(11)
                    .SetMarginBottom(5));

                document.Add(new Paragraph(
                "As a confirmed employee, you are now eligible for the following leave benefits:\n" +
                "Annual Leave: Total 21 days per year including.\n" +
                "Sick Leave: 7 days per year.\n" +
                "Casual Leave: 6 days per year.\n" +
                "Privilege leave: 8 days per year.\n" +
                "All leave requests should be submitted in advance through the Leave Management\n" +
                "System in accordance with company policy.")
                    .SetFont(normal)
                    .SetFontSize(11)
                    .SetMarginBottom(20)
                );


                //  MEDICLAIM 

                document.Add(new Paragraph("2.   Mediclaim Policy:")
                    .SetFont(bold)
                    .SetFontSize(11)
                    .SetMarginBottom(5));

                document.Add(new Paragraph(
                "In line with our commitment to your well-being, you will be covered under our Mediclaim Policy.\n" +
                "This policy provides coverage for medical expenses incurred during hospitalization,\n" +
                "as well as other eligible medical treatments, based on the terms and conditions of \n" +
                "our insurance provider,Star Health.For any questions regarding the Mediclaim policy or for\n" +
                "assistance with claims,or require any clarification regarding the benefits or policies\n " +
                "please do not hesitate to reach out.")
                    .SetFont(normal)
                    .SetFontSize(11)
                //.SetMarginBottom(25)
                );
                document.Add(new Paragraph("").SetMarginBottom(30));

                int pageNumber = pdf.GetNumberOfPages();
                float pageWidth = pdf.GetDefaultPageSize().GetWidth();
                float left = document.GetLeftMargin();
                float right = document.GetRightMargin();

                float footerBaseY = 60;

                Paragraph regardsPara = new Paragraph("Regards,")
                    .SetFont(bold)
                    .SetFontSize(11)
                    .SetFixedPosition(pageNumber, left, footerBaseY + 95, pageWidth - left - right);

                document.Add(regardsPara);

                Paragraph namePara = new Paragraph("Meera Sawant. Human Resources")
                    .SetFont(bold)
                    .SetFontSize(11)
                    .SetFixedPosition(pageNumber, left, footerBaseY + 75, pageWidth - left - right);

                document.Add(namePara);

                Paragraph deptPara = new Paragraph()
                 .SetFont(bold)
                 .SetFontSize(11)
                 .SetFixedPosition(pageNumber, left, footerBaseY + 55, pageWidth - left - right);

                deptPara.Add(new Text("Department. ")
                    .SetFontColor(ColorConstants.BLACK));

                deptPara.Add(new Text("Alphonsol private Ltd.")
                    .SetFontColor(new DeviceRgb(230, 92, 0)));

                document.Add(deptPara);



                // ORANGE BAR 
                PdfCanvas canvas = new PdfCanvas(pdf.GetPage(pageNumber));
                canvas
                    .SetFillColor(new DeviceRgb(255, 140, 0))
                    .Rectangle(
                        left,
                        footerBaseY + 35,
                        pageWidth - left - right,
                        10
                    )
                    .Fill();

                Paragraph addressPara = new Paragraph(
                    "Registered Address: E-1, Grand Square, Flat No. 104, Anand Nagar, G B Road, Thane (W) 400607"
                )
                .SetFont(bold)
                .SetFontSize(9)
               .SetFontColor(ColorConstants.BLACK)
                .SetFixedPosition(
                    pageNumber,
                    left,
                    footerBaseY + 18,
                    pageWidth - left - right
                );

                document.Add(addressPara);



                document.Close();
                return ms.ToArray();

            }
        }
        protected UserEmployeeTypeResponseDO CallGetEmployeeTypeAPI(int userId)
        {
            UserEmployeeTypeResponseDO resultObj = new UserEmployeeTypeResponseDO();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    //client.BaseAddress = new Uri("https://localhost:44360/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestObj = new { user_id = userId };

                    string json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response =
                        client.PostAsync("UserList/GetUserEmployeeType", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        resultObj = js.Deserialize<UserEmployeeTypeResponseDO>(jsonString);
                    }
                    else
                    {
                        resultObj.Success = false;
                        resultObj.Error = "API Error: " + response.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                resultObj.Success = false;
                resultObj.Error = ex.Message;
            }

            return resultObj;
        }

        //private byte[] GenerateConfirmationLetter(string employeeName, string confirmationDate)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var writer = new PdfWriter(ms);
        //        var pdf = new PdfDocument(writer);
        //        var document = new Document(pdf, PageSize.A4);
        //        document.SetMargins(40, 40, 40, 40);

        //        PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        //        PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);


        //        float[] headerWidths = { 1, 2 };
        //        Table headerTable = new Table(UnitValue.CreatePercentArray(headerWidths))
        //                                .UseAllAvailableWidth();

        //        // LEFT SIDE - LOGO
        //        //Image logo = new Image(ImageDataFactory.Create(Server.MapPath("../../assets/images/alphonsol_logo.png")));
        //        //logo.SetWidth(150);
        //        //logo.SetAutoScale(true);

        //        //Cell logoCell = new Cell()
        //        //    .Add(logo)
        //        //    .SetBorder(Border.NO_BORDER)
        //        //    .SetTextAlignment(TextAlignment.LEFT)
        //        //    .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //        //    .SetPadding(0);

        //        // LEFT SIDE - LOGO + Pvt. Ltd. + CIN No
        //        Image logo = new Image(ImageDataFactory.Create(Server.MapPath("../../assets/images/alphonsol_logo.png")));
        //        logo.SetWidth(150);
        //        logo.SetAutoScale(true);

        //        // Create font
        //        var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        //        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        //        // Company details paragraph
        //        Paragraph companyDetails = new Paragraph()
        //            .Add(new Text("Pvt. Ltd.\n").SetFont(boldFont).SetFontSize(10))   // bold + new line
        //            .Add(new Text("CIN No - ").SetFont(boldFont).SetFontSize(10))     // label bold
        //            .Add(new Text("U722200MH2022PTC381560").SetFont(normalFont).SetFontSize(10)) // number normal
        //            .SetTextAlignment(TextAlignment.LEFT)
        //            .SetMarginTop(5);

        //        // Add to cell
        //        Cell logoCell = new Cell()
        //            .Add(logo)
        //            .Add(companyDetails)
        //            .SetBorder(Border.NO_BORDER)
        //            .SetTextAlignment(TextAlignment.LEFT)
        //            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //            .SetPadding(0);


        //        // RIGHT SIDE - ADDRESS + CONTACT
        //        Paragraph rightAddress = new Paragraph()
        //            .Add("High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601\n")
        //            .Add("Contact No - 9920393999\n")
        //            .Add("Email Address - support@alphonsol.com\n")
        //            .Add("Website - www.alphonsol.com")
        //            .SetFont(normal)
        //            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)) // Bold font
        //            .SetFontSize(9)
        //            .SetTextAlignment(TextAlignment.RIGHT);

        //        Cell rightCell = new Cell()
        //            .Add(rightAddress)
        //            .SetBorder(Border.NO_BORDER)
        //            .SetTextAlignment(TextAlignment.RIGHT)
        //            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //            .SetPadding(0);

        //        headerTable.AddCell(logoCell);
        //        headerTable.AddCell(rightCell);

        //        document.Add(headerTable);

        //        // -------------------------------------------------------------
        //        // ORANGE LINE (under full width)
        //        // -------------------------------------------------------------
        //        SolidLine orangeLine = new SolidLine(2);
        //        orangeLine.SetColor(ColorConstants.ORANGE);
        //        LineSeparator separator = new LineSeparator(orangeLine);
        //        separator.SetMarginTop(5);
        //        separator.SetMarginBottom(15);

        //        document.Add(separator);

        //        // -------------------------------------------------------------
        //        // TITLE
        //        // -------------------------------------------------------------
        //        Text titleText = new Text("Confirmation Letter")
        //            .SetFont(bold)
        //            .SetFontSize(16)
        //            .SetUnderline(1, -3);

        //        Paragraph title = new Paragraph(titleText)
        //            .SetTextAlignment(TextAlignment.CENTER)
        //            .SetMarginBottom(20);

        //        document.Add(title);

        //        // -------------------------------------------------------------
        //        // DATE
        //        // -------------------------------------------------------------
        //        document.Add(new Paragraph("Date: " + confirmationDate)
        //            .SetFont(bold)
        //            .SetFontSize(12)
        //            .SetMarginBottom(10));

        //        // -------------------------------------------------------------
        //        // GREETING
        //        // -------------------------------------------------------------
        //        document.Add(new Paragraph("Dear " + employeeName + ",")
        //            .SetFont(normal)
        //            .SetFontSize(12)
        //            .SetMarginBottom(10));

        //        // -------------------------------------------------------------
        //        // MAIN BODY
        //        // -------------------------------------------------------------
        //        string bodyText =
        //            "We are pleased to inform you that following the successful completion of your " +
        //            "6-month internship with Alphonsol Pvt Ltd, we are delighted to confirm your " +
        //            "transition into a full-time employee of the organization from " + confirmationDate + ". " +
        //            "Congratulations on this well-earned achievement!\n\n" +
        //            "We appreciate your hard work, dedication, and the valuable contributions you've made " +
        //            "during your training period, and we look forward to your continued success and growth " +
        //            "as part of our team.\n";

        //        document.Add(new Paragraph(bodyText)
        //            .SetFont(normal)
        //            .SetFontSize(12));

        //        // -------------------------------------------------------------
        //        // LEAVE BENEFITS
        //        // -------------------------------------------------------------
        //        document.Add(new Paragraph("1. Leave Benefits:")
        //            .SetFont(bold)
        //            .SetFontSize(12)
        //            .SetMarginTop(15));

        //        string leaveBenefits =
        //            "Annual Leave: Total 21 days per year including.\n" +
        //            "Sick Leave: 7 days per year.\n" +
        //            "Casual Leave: 6 days per year.\n" +
        //            "Privilege Leave: 4 days per year.\n\n" +
        //            "All leave requests should be submitted in advance through the Leave Management " +
        //            "System in accordance with company policy.\n";

        //        document.Add(new Paragraph(leaveBenefits)
        //            .SetFont(normal)
        //            .SetFontSize(12));

        //        // -------------------------------------------------------------
        //        // SIGNATURE
        //        // -------------------------------------------------------------
        //        document.Add(new Paragraph("\nRegards,\n\nHuman Resources Department,\nAlphonsol Private Ltd.")
        //            .SetFont(bold)
        //            .SetFontSize(12)
        //            .SetMarginTop(25));

        //        document.Close();
        //        return ms.ToArray();
        //    }
        //}


        private byte[] GenerateConfirmationLetter(string employeeName, string confirmationDate)
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);
                document.SetMargins(40, 40, 40, 40);

                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // HEADER TABLE
                float[] headerWidths = { 1, 2 };
                //float[] headerWidths = { 1, 2.8f };

                Table headerTable = new Table(UnitValue.CreatePercentArray(headerWidths))
                    .UseAllAvailableWidth();

                // ---------------- LEFT SIDE ----------------
                Image logo = new Image(ImageDataFactory.Create(
                    Server.MapPath("../../assets/images/alphonsol_logo.png")))
                    .SetWidth(150);

                // LOGO + Pvt. Ltd. row
                //Table logoRowTable = new Table(new float[] { 3, 1 });
                //logoRowTable.SetWidth(UnitValue.CreatePercentValue(100));
                //logoRowTable.SetBorder(Border.NO_BORDER);

                //logoRowTable.AddCell(
                //    new Cell().Add(logo)
                //        .SetBorder(Border.NO_BORDER)
                //        .SetTextAlignment(TextAlignment.LEFT)
                //);

                //logoRowTable.AddCell(
                //    new Cell().Add(new Paragraph("Pvt. Ltd.")
                //        .SetFont(bold)
                //        .SetFontSize(10))
                //        .SetBorder(Border.NO_BORDER)
                //        .SetTextAlignment(TextAlignment.RIGHT)
                //        .SetVerticalAlignment(VerticalAlignment.TOP)
                //);
                Table logoRowTable = new Table(new float[] { 4, 2 });
                logoRowTable.SetWidth(UnitValue.CreatePercentValue(100));
                logoRowTable.SetBorder(Border.NO_BORDER);

                // LOGO
                logoRowTable.AddCell(
                    new Cell()
                        .Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT)
                );

                logoRowTable.AddCell(
                    new Cell()
                        .Add(new Paragraph("Pvt.\u00A0Ltd.")
                            .SetFont(bold)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.RIGHT))
                        .SetBorder(Border.NO_BORDER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                Paragraph cinPara = new Paragraph()
                    .Add(new Text("CIN No - ").SetFont(bold).SetFontSize(10))
                    .Add(new Text("U722200MH2022PTC381560").SetFont(normal).SetFontSize(10))
                    .SetMarginTop(5);

                Cell logoCell = new Cell()
                    .Add(logoRowTable)
                    .Add(cinPara)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(0);

                //  RIGHT SIDE 
                Paragraph rightAddress = new Paragraph()
                    .Add(new Text(
                        "High-street\u00A0Corporate\u00A0Center,\u00A0FB-03,\u00A0Kapurbawdi\u00A0Junction,\u00A0Thane(W)\u00A0400601"
                    ))
                    .Add("\nContact No - 9920393999")
                    .Add("\nEmail Address - support@alphonsol.com")
                    .Add("\nWebsite - www.alphonsol.com")
                    .SetFont(bold)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.RIGHT);


                Cell rightCell = new Cell()
                    .Add(rightAddress)
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                headerTable.AddCell(logoCell);
                headerTable.AddCell(rightCell);

                document.Add(headerTable);

                // ORANGE LINE
                SolidLine orangeLine = new SolidLine(2);
                orangeLine.SetColor(ColorConstants.ORANGE);
                document.Add(new LineSeparator(orangeLine)
                    .SetMarginTop(5)
                    .SetMarginBottom(15));

                // TITLE

                document.Add(new Paragraph(
                    new Text("Confirmation Letter")
                        .SetFont(bold)
                        .SetFontSize(16)
                        .SetUnderline(1, -3))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // DATE
                document.Add(new Paragraph("Date: " + confirmationDate)
                    .SetFont(bold)
                    .SetFontSize(12));

                // GREETING
                document.Add(new Paragraph("Dear " + employeeName + ",")
                    .SetFont(bold)
                    .SetFontSize(12)
                    .SetMarginBottom(10));

                // BODY
                Paragraph bodyPara = new Paragraph()

                         .Add(new Text(
                             "We are pleased to inform you that following the successful completion of your " +
                             "6-month internship with Alphonsol Pvt Ltd, we are delighted to confirm your " +
                             "transition into a full-time employee of the organization from "
                         ).SetFont(normal))

                         .Add(new Text(confirmationDate)
                             .SetFont(bold))

                         .Add(new Text(".").SetFont(normal))

                         .Add("\n")

                         .Add(new Text("Congratulations on this well-earned achievement!"))
                         //.SetFont(bold))

                         .Add("\n\n")

                         .Add(new Text(
                             "We appreciate your hard work, dedication, and the valuable contributions you've made " +
                             "during your training period, and we look forward to your continued success and growth " +
                             "as part of our team."
                         ).SetFont(normal));

                bodyPara.SetFontSize(12);
                bodyPara.SetMarginBottom(10);
                document.Add(bodyPara);


                // LEAVE BENEFITS
                document.Add(new Paragraph("1. Leave Benefits:")
                    .SetFont(bold)
                    .SetFontSize(12)
                    .SetMarginTop(15));

                document.Add(new Paragraph(
                    "As a confirmed employee, you are now eligible for the following leave benefits:")
                    .SetFont(normal)
                    .SetFontSize(12)
                    .SetMarginBottom(8));

                document.Add(new Paragraph(
                    "Annual Leave: Total 21 days per year including.\n" +
                    "Sick Leave: 7 days per year.\n" +
                    "Casual Leave: 6 days per year.\n" +
                    "Privilege Leave: 4 days per year.\n\n" +
                    "All leave requests should be submitted in advance through the Leave Management " +
                    "System in accordance with company policy.\n")
                    .SetFont(normal)
                    .SetFontSize(12));


                // Paragraph signPara = new Paragraph()       //for small size document
                //.Add(new Text("Regards,\n\n")
                //    .SetFont(bold)
                //    .SetFontSize(10))

                //.Add(new Text("Human Resources Department,\n")
                //    .SetFont(bold)
                //    .SetFontSize(10))

                //.Add(new Text("Alphonsol Pvt. Ltd.")
                //    .SetFont(bold)
                //    .SetFontSize(10)                    // ✅ small size
                //    .SetFontColor(new DeviceRgb(230, 92, 0))) // 🔥 Dark Orange

                //.SetMarginTop(25);

                //  document.Add(signPara);


                Paragraph signPara = new Paragraph()
                    .Add(new Text("Regards,\n\n")
                        .SetFont(bold)
                        .SetFontSize(10))

                    .Add(new Text("Human Resources Department,\n")
                        .SetFont(bold)
                        .SetFontSize(10))

                    .Add(new Text("Alphonsol Pvt. Ltd.")
                        .SetFont(bold)
                        .SetFontSize(10)
                        .SetFontColor(new DeviceRgb(230, 92, 0)))

                    .SetMarginTop(25);

                document.Add(signPara);


                SolidLine footerLine = new SolidLine(14);
                footerLine.SetColor(new DeviceRgb(255, 140, 0));

                document.Add(new LineSeparator(footerLine)
                    .SetMarginTop(40));   // adjust spacing if needed

                Paragraph addressPara = new Paragraph(
                        "Registered Address: E-1, Grand Square, Flat No. 104, Anand Nagar, G B Road, Thane (W) 400607"
                    )
                    .SetFont(bold)
                    .SetFontSize(9)
                    .SetTextAlignment(TextAlignment.LEFT)   // ⬅ LEFT CORNER
                    .SetMarginTop(6)
                    .SetFontColor(ColorConstants.BLACK);
                document.Add(addressPara);

                document.Close();
                return ms.ToArray();
            }
        }

        public void SendConfirmationLetterEmail(string hrEmail, string employeeName, byte[] pdfBytes)
        {
            try
            {
                string subject = $"Confirmation Letter - {employeeName}";
                string body = $"Dear <b>{employeeName}</b>,<br/><br/>Please find attached the confirmation letter.<br/><br/>Regards,<br/>HRMS System";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email);
                    mail.To.Add(hrEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    // PDF attachment
                    mail.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "ConfirmationLetter.pdf"));

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("updateprobationperiodflag", "SendConfirmationLetterEmail",
                    "Exception=" + ex.Message + " Trace=" + ex.StackTrace, UserId);
            }
        }
        private void CallUpdateAPI(int userId, int probationFlag, string Remark, string DateOfExtended)
        {
            userProbationflagResponseDO apiResponse = CallUpdateProbationFlagAPI(userId, probationFlag, Remark, DateOfExtended);

            if (apiResponse.Success)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "showUserSavedMessage('Success','" + apiResponse.ResponseMsg + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "showUserSavedMessage('Failed','" + apiResponse.Error + "');", true);
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                //txtEmployeeCode.Text = string.Empty;
                //txt_name.Text = string.Empty;
                //txt_fullname.Text = string.Empty;
                ddlprobationflag.SelectedIndex = 0;
                txtRemark.Text = string.Empty;
                txtextendeddate.Text = string.Empty;
                //txt_email.Text = string.Empty;
                //txt_contact.Text = string.Empty;

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("updateprobationperiodflag", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected userProbationflagResponseDO CallUpdateProbationFlagAPI(int userId, int probationFlag, string Remark, string DateOfExtended)
        {
            userProbationflagResponseDO resultObj = new userProbationflagResponseDO();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    //client.BaseAddress = new Uri("https://localhost:44360/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Request object
                    var requestObj = new UpdateProbationRequest
                    {
                        UserId = userId,
                        ProbationFlag = probationFlag,
                        Remark = Remark,
                        DateOfExtended = DateOfExtended,
                    };

                    var json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // POST API CALL
                    HttpResponseMessage response = client.PostAsync("UserList/UpdateProbationPeriod", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        resultObj = js.Deserialize<userProbationflagResponseDO>(jsonString);
                    }
                    else
                    {
                        resultObj.Success = false;
                        resultObj.Error = "API returned error: " + response.StatusCode.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                resultObj.Success = false;
                resultObj.Error = "Exception: " + ex.Message;
            }

            return resultObj;
        }
        protected userDateResponseDataDO CallGetProbationDateAPI(int userId)
        {
            userDateResponseDataDO resultObj = new userDateResponseDataDO();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var requestObj = new { user_id = userId };

                    string json = new JavaScriptSerializer().Serialize(requestObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response =
                        client.PostAsync("UserList/GetUserJoiningDateForProbation", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        resultObj = js.Deserialize<userDateResponseDataDO>(jsonString);
                    }
                    else
                    {
                        resultObj.Success = false;
                        resultObj.Error = "API Error: " + response.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                resultObj.Success = false;
                resultObj.Error = ex.Message;
            }

            return resultObj;
        }

    }
}