<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="viewuserdocuments.aspx.cs" Inherits="HRMS.View.Modules.viewuserdocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>




</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
        <div class="modal-dialog  modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteModalLabel">Confirm Deletion</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this Document?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="confirmDeleteButton" runat="server" CssClass="btn btn-danger"
                        OnClick="confirmDeleteButton_Click" Text="Delete" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4" Style="font-size: 2.0em; font-weight: bold;">View User Details</asp:Label>
                        <%--<h4 class="card-title">Add User</h4>--%>
                        <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                    </div>

                    <div class="row">
                        <div class="col-lg-6">

                            <div class="form-group mb-4">
                                <label for="input-fullname">Employee Code</label>
                                <asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="form-control" TabIndex="1" MaxLength="30"
                                    onkeypress="return blockSpecialChar(event)"
                                    onblur="capitalizeFirstLetter(this)"
                                    placeholder="Enter Employee Code" Enabled="false">
                                </asp:TextBox>
                            </div>

                            <div class="form-group mb-4">
                                <label for="input-username">Username</label>
                                <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" TabIndex="3" MaxLength="15" Enabled="false"
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
                                <asp:TextBox ID="txt_email" runat="server" TabIndex="5" CssClass="form-control" Enabled="false"
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

                                    <asp:TextBox ID="txt_fullname" runat="server" CssClass="form-control" TabIndex="2" MaxLength="30" Enabled="false"
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
                                    <asp:TextBox ID="txt_contact" runat="server" CssClass="form-control" TabIndex="4" Enabled="false"
                                        placeholder="Enter Contact Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_txt_contact" runat="server" ControlToValidate="txt_contact" InitialValue=""
                                        ErrorMessage="Contact Number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <asp:CustomValidator ID="cvPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                        ClientValidationFunction="validatePhoneNumber"
                                        ErrorMessage="Invalid phone number format. Example: +91 XXXXXXXXXX"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <asp:RegularExpressionValidator ID="revPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                        ValidationExpression="^\+91 \d{10}$" ErrorMessage="Invalid phone number format. Example: +91 9876523451"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                                <%--     <div class="form-group mb-4">
                                    <label for="ddldesign">Designation</label>
                                    <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control" TabIndex="6">
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
                                </div>--%>
                            </div>
                        </div>
                    </div>



                    <%-- <div>
                        <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success" ValidationGroup="SaveValidationGroup" Text="Submit" CommandArgument="Submit" />
                        <asp:Button CssClass="btn btn-primary custom-clear-button" runat="server" ID="btn_rest" Text="Reset" />
                    </div>--%>
                    <div class="row mb-3">
                        <div class="col-md-12">

                            <h5 class="form-label-custom">Document Details:</h5>
                            <asp:HiddenField ID="hfUserId" runat="server" />

                            <asp:Repeater ID="RepeaterAttachments" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-bordered table-striped">
                                        <thead>
                                            <tr>
                                                <th>Filename</th>
                                                <th>Action</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <%-- Filename column --%>
                                        <td>
                                            <%# Eval("FileName") %>
                                        </td>

                                        <%-- Action column --%>
                                        <td>
                                            <asp:Button ID="btnDownload" runat="server" Text="Download"
                                                CommandArgument='<%# Eval("UserDocDetId") %>'
                                                CssClass="btn btn-sm btn-primary"
                                                OnClick="btnDownload_Click"
                                                Visible='<%# Convert.ToInt32(Eval("UserDocDetId")) != 0 %>' />
                                            <asp:Button ID="btnView" runat="server" Text="View"
                                                CommandArgument='<%# Eval("UserDocDetId") %>'
                                                CssClass="btn btn-sm btn-secondary"
                                                OnClick="btnView_Click"
                                                Visible='<%# Convert.ToInt32(Eval("UserDocDetId")) != 0 %>' />
                                            <asp:LinkButton ID="btnDelete" runat="server"
                                                CommandArgument='<%# Eval("UserDocDetId") %>'
                                                OnClick="btnDelete_Click"
                                                CssClass="btn btn-sm btn-danger"
                                                Visible='<%# Convert.ToInt32(Eval("UserDocDetId")) != 0 %>'>
                <i class="fa fa-trash"></i>
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>

                                <FooterTemplate>
                                    </tbody>
        </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

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
    <script type="text/javascript">
        function openDeleteModal() {
            var modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        }

        function closeDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('hide');
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
        function showFileModal(fileUrl) {
            var modalBody = document.getElementById("modalFileBody");
            modalBody.innerHTML = "<iframe src='" + fileUrl + "' width='100%' height='500px'></iframe>";
            $('#fileModal').modal('show'); // if using Bootstrap modal
        }
    </script>

    <script>
        function validatePhoneNumber(sender, args) {
            var phoneNumberPattern = /^\+91 \d{10}$/;
            args.IsValid = phoneNumberPattern.test(args.Value);
        }

        function formatPhoneNumber(element) {
            var input = element.value.replace(/[^\d]/g, '');
            if (input.startsWith('91')) {
                input = input.substring(2);
            }
            if (input.length > 10) {
                input = input.substring(0, 10);
            }
            if (input.length > 0) {
                input = '+91 ' + input;
            }
            element.value = input;
        }



        function containsSpecialChars(str) {
            const specialChars =
                /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
            return specialChars.test(str);
        }




    </script>
    <script>
        function capitalizeFirstLetter(input) {
            input.value = input.value.charAt(0).toUpperCase() + input.value.slice(1);
        }
    </script>
</asp:Content>

