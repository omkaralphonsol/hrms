<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddUserMain.aspx.cs" Inherits="HRMS.View.Modules.AddUserMain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <style>
        .employee-form-shell {
            max-width: 1200px;
            margin: 0 auto;
        }

        .employee-form-card {
            border: 1px solid #e5e7eb;
            border-radius: 14px;
            box-shadow: 0 8px 24px rgba(16, 24, 40, 0.06);
        }

        .employee-form-header {
            border-bottom: 1px solid #eef0f3;
            padding-bottom: 14px;
            margin-bottom: 18px;
        }

        .employee-form-title {
            font-size: 1.6rem !important;
            font-weight: 700 !important;
            color: #1f2937;
            margin-bottom: 0 !important;
        }

        .employee-form-shell label {
            font-weight: 600;
            color: #374151;
            margin-bottom: 7px;
            font-size: 13.5px;
        }

        .employee-form-shell .form-control,
        .employee-form-shell .custom-dropdown {
            border: 1px solid #d1d5db;
            border-radius: 10px;
            min-height: 42px;
            font-size: 14px;
            box-shadow: none;
        }

        .employee-form-shell .form-control:focus,
        .employee-form-shell .custom-dropdown:focus {
            border-color: #0f6cbd;
            box-shadow: 0 0 0 3px rgba(15, 108, 189, 0.15);
        }

        .form-section-title {
            font-size: 12px;
            font-weight: 700;
            letter-spacing: .06em;
            color: #6b7280;
            text-transform: uppercase;
            margin: 6px 0 14px;
        }

        .employee-form-actions {
            margin-top: 8px;
            padding-top: 10px;
            border-top: 1px solid #eef0f3;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        .employee-form-shell .btn-success {
            background: #0f766e;
            border-color: #0f766e;
            border-radius: 10px;
            font-weight: 600;
            padding: 9px 18px;
        }

        .employee-form-shell .btn-primary,
        .employee-form-shell .btn-secondary {
            border-radius: 10px;
            font-weight: 600;
            padding: 9px 18px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row employee-form-shell">
        <div class="col-lg-12">
            <div class="card employee-form-card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4 employee-form-header">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title employee-form-title">Add New User</asp:Label>
                        <%--<h4 class="card-title">Add User</h4>--%>
                        <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                    </div>

                    <div class="row">
                        <div class="col-12">
                            <div class="form-section-title">Basic Information</div>
                        </div>
                        <div class="col-lg-6">

                            <div class="form-group mb-4">
                                <label for="input-fullname">Employee Code</label>
                                <asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter Employee Code" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="txtEmployeeCode"
                                    InitialValue=""
                                    ErrorMessage="Employee Code is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>

                            <div class="form-group mb-4">
                                <label for="input-username">Username</label>
                                <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter Username" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txt_username" runat="server" ControlToValidate="txt_name" InitialValue=""
                                    ErrorMessage="Username is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                <asp:RegularExpressionValidator ID="rev_txt_username" runat="server" ControlToValidate="txt_name"
                                    ValidationExpression="^[^\s][\w]{7,14}$" ErrorMessage="Username must contain 8 to 15 characters without spaces"
                                    ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                            </div>

                            <div class="form-group mb-4">
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

                        <div class="col-lg-6">
                            <div class="mt-4 mt-lg-0">
                                <%-- <div class="form-group mb-4">
                                    <label for="input-fullname">Full Name</label>
                                    <asp:TextBox ID="txt_fullname" runat="server" CssClass="form-control" TabIndex="2" MaxLength="30"
                                        onkeypress="return blockSpecialChar(event)"
                                        onblur="capitalizeFirstLetter(this); checkNoLeadingSpace(this)"
                                        placeholder="Enter Full Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_txt_fullname" runat="server" ControlToValidate="txt_fullname" InitialValue=""
                                        ErrorMessage="Full Name is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <asp:RegularExpressionValidator ID="rev_txt_fullname" runat="server" ControlToValidate="txt_fullname"
                                        ValidationExpression="^[a-zA-Z\s]*$" ErrorMessage="Full Name must contain only letters and spaces"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>--%>

                                <div class="form-group mb-4">
                                    <label for="input-fullname">Employee Name</label>

                                    <asp:TextBox ID="txt_fullname" runat="server" CssClass="form-control" TabIndex="2" MaxLength="30"
                                        onkeypress="return blockNumbersAndSpecialChar(event)"
                                        onblur="capitalizeFirstLetter(this); checkNoLeadingSpace(this)"
                                        placeholder="Enter Employee Name">
                                    </asp:TextBox>

                                    <asp:RequiredFieldValidator ID="rfv_txt_fullname" runat="server" ControlToValidate="txt_fullname" InitialValue=""
                                        ErrorMessage="Full Name is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                                    <asp:RegularExpressionValidator ID="rev_txt_fullname" runat="server" ControlToValidate="txt_fullname"
                                        ValidationExpression="^[a-zA-Z\s]*$"
                                        ErrorMessage="Full Name must contain only letters and spaces"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>


                                <div class="form-group mb-4">
                                    <label for="input-contact-number">Contact Number</label>
                                    <asp:TextBox ID="txt_contact" runat="server" CssClass="form-control" TabIndex="4"
                                        placeholder="Enter Contact Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_txt_contact" runat="server" ControlToValidate="txt_contact" InitialValue=""
                                        ErrorMessage="Contact Number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <%-- <asp:CustomValidator ID="cvPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                        ClientValidationFunction="validatePhoneNumber"
                                        ErrorMessage="Invalid phone number format. Example: +91 XXXXXXXXXX"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <asp:RegularExpressionValidator ID="revPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                        ValidationExpression="^\+91 \d{10}$" ErrorMessage="Invalid phone number format. Example: +91 9876523451"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />--%>
                                </div>
                                <div class="form-group mb-4">
                                    <label for="ddldesign">Designation</label>
                                    <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control custom-dropdown" TabIndex="6">
                                    </asp:DropDownList>

                                    <asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1"
                                        runat="server"
                                        ControlToValidate="ddldesign"
                                        InitialValue=""
                                        ErrorMessage="Designation is required"
                                        ForeColor="Red"
                                        Display="Dynamic"
                                        ValidationGroup="SaveValidationGroup" />
                                </div>


                                <div class="form-group mb-4" runat="server" visible="false">
                                    <asp:Label runat="server" for="input-password" ID="lblpass">Password</asp:Label>
                                    <div class="password-container" style="position: relative;">
                                        <asp:TextBox ID="txt_password" runat="server" TabIndex="6" CssClass="form-control" MaxLength="15"
                                            TextMode="Password" placeholder="Enter Password" Style="margin-top: 8px;" onblur="checkNoLeadingSpace(this)">
                                        </asp:TextBox>
                                        <i class="toggle-password fas fa-eye" style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); cursor: pointer;"
                                            onclick="togglePasswordVisibility()"></i>
                                    </div>
                                </div>


                            </div>
                        </div>

                        <div class="col-12">
                            <div class="form-section-title">Employment Details</div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group mb-4">
                                <label for="input-username">ESIC No</label>
                                <asp:TextBox ID="txtESICNo" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter ESIC Number" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txtESICNo" runat="server" ControlToValidate="txtESICNo" InitialValue=""
                                    ErrorMessage="ESIC number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>

                            <div class="form-group mb-4">
                                <label for="input-username">PF No</label>
                                <asp:TextBox ID="txtPFNo" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter PF Number" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txtPFNo" runat="server" ControlToValidate="txtPFNo" InitialValue=""
                                    ErrorMessage="PF number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>

                            <div class="form-group mb-4">
                                <label for="input-email">Division</label>
                                <asp:TextBox ID="txtDivision" runat="server" TabIndex="5" CssClass="form-control"
                                    placeholder="Enter Division" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txtDivision" runat="server" ControlToValidate="txtDivision" InitialValue=""
                                    ErrorMessage="Division is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>
                        </div>



                        <div class="col-lg-6">
                            <div class="form-group mb-4">
                                <label for="input-username">Department</label>
                                <asp:TextBox ID="txtdept" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter Department Name" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txtdept" runat="server" ControlToValidate="txtdept" InitialValue=""
                                    ErrorMessage="Department is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>
                            <div class="form-group mb-4">
                                <label for="input-username">Branch</label>
                                <asp:TextBox ID="txtbranch" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15"
                                    placeholder="Enter Branch" onblur="checkNoLeadingSpace(this)">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_txtbranch" runat="server" ControlToValidate="txtbranch" InitialValue=""
                                    ErrorMessage="Branch is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />

                            </div>

                            <div class="form-group mb-4">
                                <label for="ddlcompany">Company Name</label>
                                <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control" TabIndex="6">
                                </asp:DropDownList>

                                <asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator2"
                                    runat="server"
                                    ControlToValidate="ddlcompany"
                                    InitialValue=""
                                    ErrorMessage="Company Name is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group mb-4">
                                <label for="txtDateOfJoining">Date of Joining</label>

                                <div id="divDateOfJoining" class="input-group date mb-2" runat="server">
                                    <asp:TextBox
                                        runat="server"
                                        ID="txtDateOfJoining"
                                        CssClass="form-control"
                                        autocomplete="off"
                                        MaxLength="20"
                                        placeholder="Select Date of Joining"
                                        Style="font-size: 13px;">
                                    </asp:TextBox>
                                    <span class="input-group-addon">
                                        <i class="fas fa-calendar-alt fa-lg"
                                            style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%);"></i>
                                    </span>
                                </div>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="txtDateOfJoining"
                                    InitialValue=""
                                    ErrorMessage="Date is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>
                            <div class="form-group mb-4">
                                <label for="ddl_reportingmanager">Reporting Manager</label>
                                <asp:DropDownList ID="ddl_reportingmanager" runat="server" CssClass="form-control custom-dropdown"></asp:DropDownList>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="ddl_reportingmanager"
                                    InitialValue=""
                                    ErrorMessage="Reporting Manager is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group mb-4">
                                <label for="ddlprobationperiod">Probation Period</label>

                                <asp:DropDownList ID="ddlprobationperiod" runat="server" CssClass="form-control" TabIndex="6">
                                    <asp:ListItem Text="0 Month" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="1 Month" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="3 Months" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="6 Months" Value="6" Selected="True"></asp:ListItem>
                                </asp:DropDownList>

                                <asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator4"
                                    runat="server"
                                    ControlToValidate="ddlprobationperiod"
                                    InitialValue=""
                                    ErrorMessage="Probation Period is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>
                            <div class="form-group mb-4">
                                <label>Employment Type</label>

                                <asp:DropDownList ID="ddlexporintern" runat="server" CssClass="form-control" TabIndex="6">
                                </asp:DropDownList>


                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="ddlexporintern"
                                    InitialValue=""
                                    ErrorMessage="Type is required"
                                    ForeColor="Red"
                                    Display="Dynamic"
                                    ValidationGroup="SaveValidationGroup" />
                            </div>

                        </div>
                    </div>

                    <div class="employee-form-actions">
                        <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success" ValidationGroup="SaveValidationGroup" OnClick="SubmitButtonClick" Text="Submit" CommandArgument="Submit" />
                        <asp:Button CssClass="btn btn-primary custom-clear-button" runat="server" ID="btn_rest" Text="Reset" OnClick="ClearButton_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--   <script type="text/javascript">

        /$(document).ready(function () { $('.selectBox').SumoSelect(); });/
        function blockSpecialChar(e) {
            var k;
            document.all ? k = e.keyCode : k = e.which;
            return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || k == 55 || (k >= 48 && k <= 57) || k == 45);
        }
        function setTwoNumberDecimal(event) {
            this.value = parseFloat(this.value).toFixed(2);
        }
    </script>--%>
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
    <%-- <script type="text/javascript">
        window.onload = function () {
            var ddl = document.getElementById('<%= ddldesign.ClientID %>');
            ddl.options[0].disabled = true;
            ddl.options[0].style.color = 'gray';
        };
    </script>--%>
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


        function togglePasswordVisibility() {
            var passwordInput = document.getElementById('<%= txt_password.ClientID %>');
            var toggleIcon = document.querySelector('.toggle-password');
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.classList.remove('fa-eye');
                toggleIcon.classList.add('fa-eye-slash');
            } else {
                passwordInput.type = 'password';
                toggleIcon.classList.remove('fa-eye-slash');
                toggleIcon.classList.add('fa-eye');
            }
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
    <script type="text/javascript">
        $(document).ready(function () {
            flatpickr("#<%= txtDateOfJoining.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true
            });



            // Optional: Show calendar on icon click
            $('.input-group-addon').on('click', function () {
                $(this).closest('.input-group').find('input').focus();
            });
        });
</script>
</asp:Content>

