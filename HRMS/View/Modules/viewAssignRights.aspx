<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="viewAssignRights.aspx.cs" Inherits="HRMS.View.Modules.viewAssignRights" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <script src="../../assets/js/commonFunctions.js"></script>
    <style>
        .gridview-pagination a {
            margin-right: 10px;
        }
    </style>
    <style>
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
                    Are you sure you want to delete this Assigned Rights?
               
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeDeleteModal()" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="confirmDeleteButton" runat="server" type="button" class="btn btn-danger" OnClick="confirmDeleteButton_Click" Text="Delete" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnClickedAssignRightsId" runat="server" />
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">

                    <h2 class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">&nbsp;Assigned Rights List
   
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

       

                            <button runat="server" id="btn_addassignrights" onserverclick="AddFunction" class="btn btn-primary ms-2" title="Add Assign Rights">
                                <i class="fas fa-user-plus"></i>&nbsp;Assign Rights
       
                            </button>
                            <asp:Button ID="btnBack1" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="false" OnClick="btnBack1_click" />

                        </div>
                    </h2>

                    <div id="advancedSearchFields" class="row" runat="server" visible="false">
                        <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="ddl_role">User Role</label>
                                <asp:DropDownList ID="ddl_role" runat="server" CssClass="form-control custom-dropdown"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="mb-3">
                                <label for="ddl_menu">Menu</label>
                                <asp:DropDownList ID="ddl_menu" runat="server" CssClass="form-control custom-dropdown"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlMenu_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-6" id="lbl_submenu" runat="server" visible="false">
                            <div class="mb-3">
                                <label for="ddl_submenu">Sub-Menu</label>
                                <asp:DropDownList ID="ddl_submenu" runat="server" CssClass="form-control custom-dropdown" InitialValue=""></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-12  mt-3">
                            <asp:Button runat="server" ID="Button1" Text="Search" CssClass="btn btn-primary" OnClick="AdvSearchButton_Click" />
                            <asp:Button runat="server" ID="Button7" Text="Clear" CssClass="btn btn-primary" OnClick="AdvClearButton_Click" />
                            <asp:Button runat="server" ID="Button8" Text="Back" CssClass="btn btn-primary" OnClientClick="highlightBackButton();" OnClick="AdvBackButton_Click" />

                        </div>
                    </div>


                    <br />
                    <%-- <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
                    <div class="table-responsive">

                        <asp:HiddenField ID="hfPageIndexViewRights" runat="server" />

                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false" DataKeyNames="userrightid" OnRowCommand="gvUsers_RowCommand" EnablePersistedSelection="true" OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                    AllowSorting="true" OnSorting="gridview_Sorting" Style="margin: 0 auto;" EmptyDataText="No records found.">
                                    <Columns>

                                        <asp:TemplateField HeaderText="SR No" ItemStyle-Width="80px">
                                            <ItemTemplate>
                                                <%# (Convert.ToInt32(hfPageIndexViewRights.Value) * gridview.PageSize) + Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:BoundField DataField="userrightid" HeaderText="Role Id" ItemStyle-Width="130px" Visible="false" />
                                        <asp:BoundField DataField="roledescription" HeaderText="User Role" ItemStyle-Width="130px" />
                                        <asp:BoundField DataField="Menu" HeaderText="Menu" ItemStyle-Width="130px" />
                                        <asp:BoundField DataField="Submenu" HeaderText="Submenu" ItemStyle-Width="130px" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" CommandName="deleteRight" runat="server" CommandArgument='<%# Eval("userrightid") %>' title="Delete Right">
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
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="gridview" EventName="RowCommand" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </div>
                </div>
            </div>
        </div>
        <!-- end col -->
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showRightsSavedMessage(status, remark) {
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
            '<%= ddl_role.ClientID %>',
            '<%= ddl_menu.ClientID %>',
            '<%= ddl_submenu.ClientID %>'
            ];

            dropdownIds.forEach(function (id) {
                var ddl = document.getElementById(id);
                if (ddl && ddl.options.length > 0) {
                    ddl.options[0].disabled = true;
                    ddl.options[0].style.color = 'gray';
                }
            });
        };
    </script>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script type="text/javascript">
        function openDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('show');
        }
        function closeDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('hide');
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
</asp:Content>

