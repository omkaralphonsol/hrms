<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ViewRole.aspx.cs" Inherits="HRMS.View.Modules.ViewRole" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
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

        div.scroll {
            overflow-x: auto;
            white-space: nowrap;
            max-width: 19%;
            height: auto
        }

        .drawer-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        .close-button {
            cursor: pointer;
        }

        .small-font {
            font-size: 14px;
        }

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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="modal fade" id="deleteRoleModal" tabindex="-1" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Deletion</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this Role?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnConfirmDeleteRole" runat="server" CssClass="btn btn-danger"
                        Text="Delete" OnClick="confirmDeleteButton_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnClickedRoleId" runat="server" />

    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <div class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">
                        &nbsp;Role List
     
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
       
        <button runat="server" id="btn_addRole" onserverclick="Button3_ServerClick" class="btn btn-primary ms-2" title="Add Role">
            <i class="fas fa-user-plus"></i>&nbsp;Add Role
       
        </button>
                            <asp:Button ID="btnBack1" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="false" OnClick="btnBack1_click" />

                        </div>
                    </div>

                </div>


                <div class="overlay" onclick="closeFilterDrawer()"></div>



                <div id="advancedSearchFields" class="row" runat="server" visible="false">
                    <div class="col-lg-12" style="right: -10px">
                        <div class="mb-6">
                            <label for="input-ddl_role">Role</label>
                           <asp:DropDownList ID="ddl_role" CssClass="form-control custom-dropdown" runat="server">
</asp:DropDownList>

                        </div>

                    </div>
                    <br />
                      <br />
                      <br />
                    <div class="col-12 mt-2 " style="right: -10px">
                        <div>
                            <asp:Button runat="server" ID="Button1" Text="Search" CssClass="btn btn-primary" OnClick="AdvSearchButton_Click" />
                            <asp:Button runat="server" ID="Button7" Text="Clear" CssClass="btn btn-primary" OnClick="AdvClearButton_Click" />
                            <asp:Button runat="server" ID="Button8" Text="Back" CssClass="btn btn-primary" OnClientClick="highlightBackButton();" OnClick="AdvBackButton_Click" />
                        </div>
                    </div>
                </div>
                <div id="filterDrawer" class="drawer">
                    <div class="drawer-header">
                        <span>Filter</span>
                        <button class="close-button" onclick="closeFilterDrawer()">Close</button>
                    </div>
                    <br />
                    <div class="drawer-header">
                        <span class="font-weight-bold" style="font-size: 20px; color: black;">Filter</span>
                        <button class="close-button" onclick="closeFilterDrawer()" style="background: none; border: none; font-size: 20px; color: lightgrey;">✖</button>

                    </div>
                    <hr style="border-color: lightgrey; margin-top: 10px; width: 100%;">
                    <div class="drawer-content">
                        <div class="row">
                            <div class="col-md-4">
                                <label for="ddlSearchOption" style="font-size: 14px;">Search By:</label>
                            </div>
                            <div class="col-md-8">
                                <asp:DropDownList ID="ddlsearchp" runat="server" CssClass="form-control" AutoPostBack="true" onchange="return handleDropdownChange();">
                                    <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row">
                            <div class="col-md-4">
                                <label for="txtSearch" style="font-size: 14px;">Enter:</label>
                            </div>
                            <div class="col-md-8">
                                <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholder="Enter.."></asp:TextBox>
                            </div>
                        </div>


                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <hr style="border-color: lightgrey; margin-top: 10px; width: 100%;">
                        <div class="row mt-2">
                            <div class="col-12">
                                <div class="d-flex justify-content-end align-items-center">
                                    <asp:Button runat="server" ID="Button5" Text="Clear" CssClass="btn btn-light ms-2" OnClick="ClearButton_Click" onchange="return handleDropdownChange();" />
                                    <%--  <button class="btn btn-light" onclick="closeFilterDrawer()">Clear</button>--%>
                                    <asp:Button runat="server" ID="Button6" Text="Search" CssClass="btn btn-primary ms-2" />
                                    <%--  <button type="button" class="btn btn-primary ms-2">Search</button>--%>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>


                <br />
                <div class="table-responsive">
                    <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false" DataKeyNames="Roleid" OnRowCommand="gv_RowCommand" EnablePersistedSelection="true" OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                        AllowSorting="true" OnSorting="gridview_Sorting" Style="margin: 0 auto;" EmptyDataText="No records found.">
                        <Columns>

                            <asp:TemplateField HeaderText="SR No" HeaderStyle-CssClass="header-cell" ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RoleId" HeaderText="Role Id" HeaderStyle-CssClass="header-cell"
                                SortExpression="Roleid" ItemStyle-Width="130px" Visible="false" />
                            <asp:BoundField DataField="Roledescription" HeaderText="Role Description" HeaderStyle-CssClass="header-cell"
                                SortExpression="Roledescription" ItemStyle-Width="130px" />
                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkView" runat="server" CommandName="viewRole" title="View Role" CommandArgument='<%# Eval("Roleid") %>'>
                      <i class="fa fa-edit"></i> 
          </asp:LinkButton>
                                    &nbsp;                                               
                    
                     
                     

         <asp:LinkButton ID="lnkDeleteRole" runat="server"
             CommandName="deleteRole"
             CommandArgument='<%# Eval("RoleId") %>'
             ToolTip="Delete">
    <i class="fa fa-trash"></i>
         </asp:LinkButton>


                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="gridview-pagination" />
                    </asp:GridView>
                    <div class="pagination-container" style="font-size: 14px; color: black;">
                        <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
                            Style="background-color: white; color: black; border: 1px solid #ddd; padding: 5px 10px; margin: 2px; margin-left: auto;">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- end col -->
    </div>
   
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>

    <script>
        function showRoleSavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>
    <script type="text/javascript">
        function openDeleteModal(roleId) {
            // Save RoleId in hidden field
            document.getElementById('<%= hdnClickedRoleId.ClientID %>').value = roleId;

            // Show modal
            $('#deleteRoleModal').modal('show');
            return false;
        }

        function closeDeleteModal() {
            $('#deleteRoleModal').modal('hide');
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
        function handleDeleteClick(linkButton) {
            var AssignRoleId = linkButton.getAttribute('data-diagnosis-master-id');
            document.getElementById('<%= hdnClickedRoleId.ClientID %>').value = AssignRoleId;
            showPopupDelete();
        }
    </script>
</asp:Content>
