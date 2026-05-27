<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="adduserdocuments.aspx.cs" Inherits="HRMS.View.Modules.adduserdocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Add User Documents</asp:Label>
                        <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary"
                            Text="Back" OnClick="btnBack_Click" />
                    </div>

                    <div class="row">
                        <div class="col-lg-6">
                            <div class="form-group mb-6">
                                <label for="ddldocument">select Documents</label>
                                <asp:DropDownList ID="ddldocument" runat="server" CssClass="form-control" TabIndex="6">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group mb-6">
                                <label class="form-label">Upload Attachments:</label>
                                <%-- <div class="d-flex align-items-center gap-2">--%>
                                <asp:FileUpload ID="fileUpload" CssClass="form-control" runat="server" AllowMultiple="true" />
                                <%--<asp:Button ID="btnUpload" CssClass="btn btn-outline-secondary" runat="server" 
                                    Text="Upload" OnClick="btnUpload_Click" />--%>
                                <asp:Label ID="lblFileUploadError" runat="server" CssClass="text-danger"></asp:Label>
                                <%--  </div>--%>
                                <small class="text-muted">PDF, DOCX, CSV, JPG allowed. Max size 5MB.</small>
                                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger d-block"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-6">
                            <div class="form-group mb-6">
                                <label for="input-contact-number">Reference Number</label>
                                <asp:TextBox ID="txt_reference" runat="server" CssClass="form-control" TabIndex="4"
                                    placeholder="Enter Reference Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txt_Reference" runat="server" ControlToValidate="txt_reference" InitialValue=""
                                    ErrorMessage="Reference Number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group mb-6">
                                <label for="input-email">Email Id</label>
                                <asp:TextBox ID="txt_email" runat="server" TabIndex="5" CssClass="form-control"
                                    placeholder="Enter Email Id" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txt_email" runat="server" ControlToValidate="txt_email" InitialValue=""
                                    ErrorMessage="Email Id is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                <asp:RegularExpressionValidator ID="rev_txt_email" runat="server" ControlToValidate="txt_email"
                                    ValidationExpression="^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                                    ErrorMessage="Invalid Email Format. Example: test777@gmail.com or test.alphonsol@gmail.com"
                                    ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                            </div>
                        </div>




                    </div>

                    <%--                <div class="row">
     <div class="col-md-6">
         <div class="form-group mb-4">
             <label class="form-label">Uploaded Documents :</label>

             <asp:GridView ID="gvAttachments" EmptyDataText="No Files Uploaded" CssClass="table table-bordered table-hover" runat="server" OnRowCommand="gvAttachments_RowCommand" AutoGenerateColumns="false">
                 <Columns>
                     <asp:TemplateField HeaderText="File Name">
                         <ItemTemplate>
                             <div style="white-space: normal; word-wrap: break-word; width: 200px;">
                                 <%# Eval("FileName") %>
                             </div>
                         </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Actions">
                         <ItemTemplate>
                             <a href='<%# Eval("FilePath") %>' target="_blank" class="btn btn-sm btn-primary"><i class="fa fa-eye"></i></a>
                             <asp:LinkButton ID="btnDelete" CssClass="btn btn-sm btn-danger" runat="server" CommandName="DeleteFile" CommandArgument='<%# Eval("FileId") %>'>
                               <i class="fa fa-trash"></i>
                             </asp:LinkButton>
                         </ItemTemplate>
                     </asp:TemplateField>
                 </Columns>
             </asp:GridView>
             <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger small"></asp:Label>
         </div>
     </div>
 </div>--%>
                    <div class="mt-4">
                        <asp:Button ID="btnUpload" CssClass="btn btn-success" runat="server"
                            Text="Upload" OnClick="btnUpload_Click" />
                        <asp:Button CssClass="btn btn-primary custom-clear-button" runat="server" ID="btn_rest" Text="Reset" OnClick="ClearButton_Click" />

                    </div>
                </div>
            </div>
        </div>
    </div>


    <script type="text/javascript">

        /$(document).ready(function () { $('.selectBox').SumoSelect(); });/
        function blockSpecialChar(e) {
            var k;
            document.all ? k = e.keyCode : k = e.which;
            return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || k == 55 || (k >= 48 && k <= 57) || k == 45);
        }
        function setTwoNumberDecimal(event) {
            this.value = parseFloat(this.value).toFixed(2);
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showUserSavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>
           <script>
               function blockNumbersAndSpecialChar(event) {
                   var key = event.key;
                   var regex = /^[a-zA-Z\s]$/;
                   return regex.test(key);
               }

               function capitalizeFirstLetter(input) {
                   if (input.value.length > 0) {
                       input.value = input.value.charAt(0).toUpperCase() + input.value.slice(1);
                   }
               }

               function checkNoLeadingSpace(input) {
                   input.value = input.value.replace(/^\s+/, '');
               }
           </script>

        <script>
            function checkNoLeadingSpace(input) {
                input.value = input.value.trimStart();
                if (input.value.startsWith(" ")) {
                    input.value = input.value.trimStart();
                }
            }
</script>

        <script>
            function validatePhoneNumber(sender, args) {
                var phoneNumberPattern = /^\+91 \d{10}$/;
                args.IsValid = phoneNumberPattern.test(args.Value);
            }

            function formatPhoneNumber(element) {
                var input = element.value.replace(/[^\d]/g, '');
                //if (input.startsWith('91')) {
                //    input = input.substring(2);
                //}
                if (input.length > 10) {
                    input = input.substring(0, 10);
                }
                //if (input.length > 0) {
                //    input = '+91 ' + input;
                //}
                element.value = input;
            }

            

            function containsSpecialChars(str) {
                const specialChars =
                    /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
                return specialChars.test(str);
            }

            function CheckPassword(p) {
                errors = [];
                if (p.value.length < 8) {
                    errors.push("Your password must be at least 8 characters");
                }
                if (p.value.search(/[A-Z]/) < 0) {
                    errors.push("Your password must contain at least one upper case letter.");
                }

                if (p.value.search(/[a-z]/) < 2) {
                    errors.push("Your password must contain at least 3 lowercase letters.");
                }
                if (p.value.search(/[0-9]/) < 2) {
                    errors.push("Your password must contain at least 2 numbers.");
                }
                if (!containsSpecialChars(p.value)) {
                    errors.push("Your password must contain at least one specail character.");
                }

                if (errors.length > 0) {
                    document.getElementById('errorname').innerHTML = errors.join("\n");
                    return false;
                }
                document.getElementById('errorname').innerHTML = "";
                return true;
            }

            function CheckPasswordNew(sender, args) {
                var validatetext = args.Value;
                errors = [];
                if (validatetext.length < 8) {
                    errors.push("Your password must be at least 8 characters");
                }
                if (validatetext.search(/[A-Z]/) < 0) {
                    errors.push("Your password must contain at least one upper case letter.");
                }

                if (validatetext.search(/[a-z]/) < 2) {
                    errors.push("Your password must contain at least 3 lowercase letters.");
                }
                if (validatetext.search(/[0-9]/) < 2) {
                    errors.push("Your password must contain at least 2 numbers.");
                }
                if (!containsSpecialChars(validatetext)) {
                    errors.push("Your password must contain at least one specail character.");
                }

                if (errors.length > 0) {
                    args.IsValid = false;
                }
                else {
                    args.IsValid = true;
                }

            }
    </script>
        <script>
            function capitalizeFirstLetter(input) {
                input.value = input.value.charAt(0).toUpperCase() + input.value.slice(1);
            }
    </script>
      
</asp:Content>


