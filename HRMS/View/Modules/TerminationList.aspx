<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="TerminationList.aspx.cs" Inherits="HRMS.View.Modules.TerminationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>

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


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--    <div class="modal fade" id="responseModal" tabindex="-1" role="dialog" aria-labelledby="responseModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="responseModalLabel">Employee Response</h5>
                <button type="button" class="close" data-dismiss="modal" onclick="closeResponseModal()">&times;</button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfResponseUserId" runat="server" />
                <p id="modalEmployeeName"></p>
                <p id="modalResponseStatus"></p>
                <!-- Add more fields if needed -->
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" onclick="closeResponseModal()">Close</button>
                <asp:Button ID="btnSubmitResponse" runat="server" CssClass="btn btn-success" Text="Submit" OnClick="btnSubmitResponse_Click" />
            </div>
        </div>
    </div>
</div>--%>

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Termination List</asp:Label>
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>
                            <%--                            <button runat="server" id="btn_advanceserach" onserverclick="AdvSearchFunction" class="btn btn-outline-dark ms-2 light-border" title="Advance Search">
        <i class="bx bx-search-alt"></i>&nbsp;Advance Search
     
    </button>--%>
    &nbsp;
     
    <%--                            <button runat="server" id="btn_adduser" onserverclick="Button3_ServerClick" class="btn btn-primary ms-2" title="Add User">
        <i class="fas fa-user-plus"></i>&nbsp;Add User
     
    </button>--%>
                            <%--                            <asp:Button ID="btnBack1" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="false" OnClick="btnBack1_click" />--%>
                        </div>
                    </div>

                    <div class="row mb-3 align-items-end">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="ddlcompany" class="fw-bold">Select Company</label>
                                <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control custom-dropdown" TabIndex="6">
                                    <asp:ListItem Text="-- Select Company --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-md-8 d-flex align-items-end">
                            <div class="d-flex gap-2">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnsearch_click" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnclear_click" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <asp:UpdatePanel ID="updGrid" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false"
                                        DataKeyNames="UserId" EnablePersistedSelection="true" OnRowDataBound="gridview_RowDataBound"
                                        OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                        AllowSorting="true" OnSorting="gridview_Sorting"
                                        Style="margin: 0 auto;" EmptyDataText="No records found.">
                                        <Columns>
                                             <asp:TemplateField HeaderText="SR No">
     <ItemTemplate>
         <%# (gridview.PageIndex * gridview.PageSize) + Container.DataItemIndex + 1 %>
     </ItemTemplate>
 </asp:TemplateField>
                                            <asp:BoundField DataField="EmployeeCode" HeaderText="Employee Code" Visible="true" />
                                            <asp:BoundField DataField="UserId" HeaderText="User Id" Visible="false" />
                                            <%--                                    <asp:BoundField DataField="Username" HeaderText="Username" ItemStyle-Width="130px" />--%>
                                            <asp:BoundField DataField="user_fullname" HeaderText="Employee Name" />
                                            <asp:BoundField DataField="user_mail_id" HeaderText="Email Id" />
                                            <%--                                    <asp:BoundField DataField="contact_detail" HeaderText="Contact Number" ItemStyle-Width="130px" />--%>
                                            <asp:BoundField
                                                DataField="TerminationDate"
                                                HeaderText="Termination Date"
                                                DataFormatString="{0:dd-MM-yyyy}"
                                                NullDisplayText="-" />

                                            <%--                                     <asp:BoundField DataField="notice_status" HeaderText="Status" />--%>




                                            <asp:TemplateField HeaderText="Response">
                                                <ItemTemplate>
                                                    <asp:Button ID="btnPending" runat="server"
                                                       CssClass="btn btn-sm btn-primary"
                                                        Text="Pending"
                                                        CommandArgument='<%# Eval("UserId") %>'
                                                        OnClick="btnPending_Click" />

                                                    <asp:Button ID="btnResponded" runat="server"
                                                      CssClass="btn btn-sm btn-primary"
                                                        Text="Responded"
                                                        CommandArgument='<%# Eval("UserId") %>'
                                                        OnClick="btnResponded_Click" />
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>


                             <%--   <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                                </Triggers>--%>


                            </asp:UpdatePanel>
                            <asp:Panel ID="paginationContainer" runat="server"
                                CssClass="pagination-container"
                                Style="text-align: right; font-size: 14px; color: black;"
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
        function initializeSearch() {
            $(document).on('input', '#searchInput', function () {
                var searchTerm = $(this).val().toLowerCase();
                filterGrid(searchTerm);
            });
            $(document).on('keydown', '#searchInput', searchOnEnter);
        }

        function filterGrid(searchTerm) {
            $('#<%= gridview.ClientID %> tr:has(td)').hide();

            if (searchTerm === '') {
                $('#<%= gridview.ClientID %> tr:has(td)').show();
            } else {
                $('#<%= gridview.ClientID %> tr:has(td)').filter(function () {
                    var found = false;
                    $(this).find('td').each(function () {
                        if ($(this).text().toLowerCase().includes(searchTerm)) {
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
    <%-- <script>
        function openResponseModal(userId) {
            // Set the hidden field value
            document.getElementById('<%= hfResponseUserId.ClientID %>').value = userId;

            // Optionally set modal content dynamically (employee name/status)
            // You can fetch from GridView row using JS if needed

            $('#responseModal').modal('show');
            return false;
        }

        function closeResponseModal() {
            $('#responseModal').modal('hide');
            return false;
        }

    </script>--%>
    <script>

        function checkDeadlineOnly(deadlineStr) {

            var deadline = new Date(deadlineStr);
            var today = new Date();

            // Remove time
            today.setHours(0, 0, 0, 0);
            deadline.setHours(0, 0, 0, 0);

            // If expired
            if (today > deadline) {
                alert("Response deadline is over. You cannot respond now.");
                return false; // STOP action
            }

            // Allowed
            return true; // Continue postback
        }

    </script>

</asp:Content>
