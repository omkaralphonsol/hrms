<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="useruploaddocuments.aspx.cs" Inherits="HRMS.View.Modules.useruploaddocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script src="../../assets/js/commonFunctions.js"></script>
    <style>
        .gridview-pagination a {
            margin-right: 10px;
        }
    </style>

    <style>
        .drawer {
            display: none;
            position: fixed;
            right: 0;
            top: 0;
            height: 100%;
            width: 350px;
            background-color: #fff;
            box-shadow: -1px 0 10px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            padding: 20px;
        }

            .drawer.show {
                display: block;
            }

        .drawer-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        div.scroll {
            overflow-x: auto;
            white-space: nowrap;
            max-width: 19%;
            height: auto
        }

        .close-button {
            cursor: pointer;
        }

        /* .small-font {
           font-size: 14px;
        }*/

        .page-container {
            margin-left: 20px;
            margin-right: 20px;
        }

        .pagination-container {
            margin-top: 20px;
            padding: 10px;
        }

        .app-search .position-relative {
            display: flex;
        }

        .app-search input {
            border-radius: 0;
        }

        .app-search .btn {
            border-radius: 0;
        }

        .app-search .input-group-append {
            position: absolute;
            right: 0;
            top: 0;
            bottom: 0;
            display: flex;
            align-items: center;
        }

        .drawer-content {
            display: flex;
            flex-direction: column;
            height: 100%;
        }

        .drawer-footer {
            margin-top: auto;
            display: flex;
            justify-content: flex-end;
            align-items: center;
        }
    </style>
    <style>
        .custom-dropdown-container {
            position: relative;
        }

        .custom-dropdown {
            padding-right: 25px;
            -webkit-appearance: none;
            -moz-appearance: none;
            appearance: none;
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"%3E%3Cpath stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" /%3E%3C/svg%3E') no-repeat right center;
            background-size: 16px;
        }
    </style>
    <style>
        .highlighted {
            background-color: rgba(0, 0, 0, 0.5);
            color: #ffffff;
        }
    </style>
    <style>
        .view-link {
            color: grey;
        }

        .edit-link {
            color: blue;
        }

        .comment-link {
            color: orange;
        }

        .phone-link {
            color: #19b512;
        }

        .delete-link {
            color: red;
        }
    </style>
    <style>
        .pagination-container {
            display: flex;
            align-items: center;
        }

            .pagination-container button,
            .pagination-container select {
                margin: 0;
            }

        .custom-gridview {
            border-left: none;
            border-right: none;
        }

            .custom-gridview th,
            .custom-gridview td {
                border-left: none;
                border-right: none;
            }
    </style>
    <style>
        #overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.3);
            backdrop-filter: blur(3px);
            z-index: 999;
            animation: fadeInOverlay 0.3s ease-in-out;
        }

        #popup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        #phonePopup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }


        #DeletePopUp {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        #DeleteMessage {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        @keyframes fadeInOverlay {
            from {
                opacity: 0;
            }

            to {
                opacity: 1;
            }
        }

        @keyframes fadeInPopup {
            from {
                opacity: 0;
                transform: translate(-50%, -50%) scale(0.8);
            }

            to {
                opacity: 1;
                transform: translate(-50%, -50%) scale(1);
            }
        }
    </style>
    <style>
        #phonePopupPhoneNumber,
        #phonePopupMobileNumber,
        #phonePopupLabels {
            font-size: 16px;
        }

        .light-border {
            border-color: lightgrey;
        }

        .pagination-container {
            display: flex;
            justify-content: flex-end; /* pushes content to the right */
            align-items: center;
            margin-top: 10px;
            padding: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Bootstrap Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteModalLabel">Confirm Deletion</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true" onclick="closeDeleteModal()">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this User?
                </div>
                <%-- <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeDeleteModal()" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="confirmDeleteButton" runat="server" type="button" class="btn btn-danger" OnClick="confirmDeleteButton_Click" Text="Delete" />
                </div>--%>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnClickedUserId" runat="server" />
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <div class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">
                        &nbsp;User Upload Documents
   
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>
                            <button runat="server" id="btn_advanceserach" onserverclick="AdvSearchFunction" class="btn btn-outline-dark ms-2 light-border" title="Advance Search">
                                <i class="bx bx-search-alt"></i>&nbsp;Advance Search
       
                            </button>
                            &nbsp;
       
                            <%--                            <button runat="server" id="btn_adduser" onserverclick="Button3_ServerClick" class="btn btn-primary ms-2" title="Add User">
                                <i class="fas fa-user-plus"></i>&nbsp;Add User
       
                            </button>--%>
                            <asp:Button ID="btnBack1" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="false" OnClick="btnBack1_click" />

                        </div>
                    </div>


                    <div class="overlay" onclick="closeFilterDrawer()"></div>
                    <div id="advancedSearchFields" class="row" runat="server" visible="false">
                        <%-- <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="input-ddl_employeeCode">Employee Code</label>
                                <asp:DropDownList ID="ddl_employeeCode" CssClass="form-control custom-dropdown" runat="server">
                                </asp:DropDownList>

                            </div>

                        </div>--%>
                        <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="input-ddl_username">Username</label>
                                <asp:DropDownList ID="ddl_username" CssClass="form-control custom-dropdown" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="input-contact-number">Contact Number</label>
                                <asp:TextBox ID="txt_contact" runat="server" CssClass="form-control" TabIndex="4"
                                    placeholder="Enter Contact Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)"></asp:TextBox>
                                <asp:CustomValidator ID="cvPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                    ClientValidationFunction="validatePhoneNumber"
                                    ErrorMessage="Invalid phone number format. Example: +91 XXXXXXXXXX"
                                    ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                <asp:RegularExpressionValidator ID="revPhoneNumber" runat="server" ControlToValidate="txt_contact"
                                    ValidationExpression="^\+91 \d{10}$" ErrorMessage="Invalid phone number format. Example: +91 9876523451"
                                    ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                            </div>
                        </div>
                        <div class="col-12 mt-2 " style="float: right;">
                            <div>
                                <asp:Button runat="server" ID="Button1" Text="Search" CssClass="btn btn-primary" OnClick="AdvSearchButton_Click" />
                                <asp:Button runat="server" ID="Button7" Text="Clear" CssClass="btn btn-primary" OnClick="AdvClearButton_Click" />
                                <asp:Button runat="server" ID="Button8" Text="Back" CssClass="btn btn-primary" OnClientClick="highlightBackButton();" OnClick="AdvBackButton_Click" />
                            </div>
                        </div>
                    </div>


                    <br />
                    <%-- <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
                    <div class="row mb-3 align-items-end">
                        <!-- Dropdown -->
                        <div class="col-md-4">
                            <div class="form-group" runat="server" id="ddlcomp" visible="true">
                                <label for="ddlcompany" class="fw-bold">Select Company</label>
                                <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control custom-dropdown" TabIndex="6">
                                    <asp:ListItem Text="-- Select Company --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Buttons: Search + Clear -->
                        <div class="col-md-8 d-flex align-items-end">
                            <div class="d-flex gap-2" id="btnsearchclear" runat="server" visible="true">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnsearch_click" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnclear_click" />
                            </div>
                        </div>
                    </div>

                    <!-- GridView -->
                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand"
                                DataKeyNames="UserId" EnablePersistedSelection="true"
                                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gridview_Sorting"
                                Style="margin: 0 auto;" EmptyDataText="No records found.">
                                <Columns>
                                    <asp:BoundField DataField="EmployeeCode" HeaderText="Employee Code" ItemStyle-Width="130px" Visible="true" />
                                    <asp:BoundField DataField="UserId" HeaderText="User Id" ItemStyle-Width="130px" Visible="false" />
                                    <asp:BoundField DataField="Username" HeaderText="Username" ItemStyle-Width="130px" />
                                    <asp:BoundField DataField="user_fullname" HeaderText="Employee Name" ItemStyle-Width="130px" />
                                    <asp:BoundField DataField="user_mail_id" HeaderText="Email Id" ItemStyle-Width="130px" />
                                    <asp:BoundField DataField="contact_detail" HeaderText="Contact Number" ItemStyle-Width="130px" />

                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="80px">
                                        <ItemTemplate>
                                           <asp:LinkButton ID="lnkAdd" runat="server"
    CommandName="addUser"
    CommandArgument='<%# Eval("UserId") %>'
    title="Add User">

    <i class="fa fa-plus"></i>
</asp:LinkButton>
                                            <asp:LinkButton ID="lnkViews" runat="server" CommandName="viewUsers" CommandArgument='<%# Eval("UserId") %>' CssClass="me-1" ToolTip="View User">
    <i class="fa fa-eye"></i>
                                            </asp:LinkButton>
                                            <%--<asp:LinkButton ID="lnkViews" runat="server" CommandName="viewUsers" CommandArgument='<%# Eval("UserId") %>' CssClass="me-1" ToolTip="View">
<i class="fa fa-eye"></i>
                                            </asp:LinkButton>--%>

                                            <%--  <asp:LinkButton ID="lnkDelete" CommandName="deleteUser" runat="server" CommandArgument='<%# Eval("UserId") %>' title="Delete User">
                                             <i class="fa fa-trash"></i>
                                            </asp:LinkButton>--%>
                                        </ItemTemplate>

                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <!-- Pagination -->
                            <asp:Panel ID="paginationContainer" runat="server"
                                CssClass="pagination-container"
                                Style="display: flex; justify-content: flex-end; font-size: 14px; color: black;"
                                Visible="false">

                                <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
                                    Style="background-color: white; color: black; border: 1px solid #ddd; padding: 5px 10px; margin: 2px;">
                                </asp:DropDownList>
                            </asp:Panel>



                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- end col -->
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

    <script type="text/javascript">
        window.onload = function () {
            var dropdownIds = [
       <%-- '<%= ddl_employeeCode.ClientID %>',--%>
                '<%= ddl_username.ClientID %>'
            ];

            dropdownIds.forEach(function (id) {
                var ddl = document.getElementById(id);
                if (ddl && ddl.options.length > 0) {
                    ddl.options[0].style.color = 'gray';
                }
            });
        };

    </script>

    <%--new code--%>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script type="text/javascript">
        function openDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('show');
            return false;
        }
        function closeDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('hide');
            return false;
        }
    </script>

    <script>     
        function initializeSearch() {
            $(document).on('input', '#searchInput', function () {
                var searchTerm = $(this).val().toLowerCase();
                filterGrid(searchTerm);
            });
            $(document).on('keydown', '#searchInput', searchOnEnter);
        }


        function filterGrid(searchTerm) {
      <%= gridview.ClientID %> 
            $('#<%= gridview.ClientID %> tr:has(td)').hide();

            if (searchTerm === '') {
                $('#<%= gridview.ClientID %> tr:has(td)').show();
            } else {
                var genderColumnIndex = 3;

                $('#<%= gridview.ClientID %> tr:has(td)').filter(function () {
                    var found = false;
                    $(this).find('td').each(function (index) {
                        var cellText = $(this).text().toLowerCase();

                        if (index === genderColumnIndex && cellText.startsWith(searchTerm)) {
                            found = true;
                            return false;
                        }

                        if (index !== genderColumnIndex && cellText.includes(searchTerm)) {
                            found = true;
                            return false;
                        }
                    });
                    return found;
                }).show();
            }
        }


        function searchOnEnter(event) {
            if (event.key === 'Enter') {
                event.preventDefault();
                var searchTerm = $('#searchInput').val().toLowerCase();
                filterGrid(searchTerm);
            }
        }


        $(document).ready(function () {
            initializeSearch();
        });
  </script>

    <script>
        function validatePhoneNumber(sender, args) {
            var phoneNumberPattern = /^\+91 \d{10}$/;
            args.IsValid = phoneNumberPattern.test(args.Value);
        }

        function formatPhoneNumber(element) {
            var input = element.value.replace(/[^\d]/g, ''); // Remove all non-numeric characters
            if (input.startsWith('91')) {
                input = input.substring(2); // Remove leading '91' if already present
            }
            if (input.length > 10) {
                input = input.substring(0, 10); // Limit to 10 digits
            }
            if (input.length > 0) {
                input = '+91 ' + input;
            }
            element.value = input;
        }
    </script>

</asp:Content>


