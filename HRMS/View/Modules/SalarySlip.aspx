<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="SalarySlip.aspx.cs" Inherits="HRMS.View.Modules.SalarySlip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Select Company</asp:Label>
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

                    <!-- Search Row -->
                    <div class="row mb-3 align-items-end">
                        <!-- Dropdown -->
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="ddlcompany" class="fw-bold">Select Company</label>
                                <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control custom-dropdown" TabIndex="6">
                                    <asp:ListItem Text="-- Select Company --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Buttons: Search + Clear -->
                        <div class="col-md-8 d-flex align-items-end">
                            <div class="d-flex gap-2">
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
                                            <asp:LinkButton ID="lnkView" runat="server" CommandName="viewUser" CommandArgument='<%# Eval("UserId") %>' title="Salary Slip">
                                            <i class="fa fa-edit"></i>
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
</asp:Content>
