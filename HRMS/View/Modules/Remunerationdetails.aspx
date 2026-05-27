<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Remunerationdetails.aspx.cs" Inherits="HRMS.View.Modules.Remunerationdetails" %>

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
                            Style="font-size: 2.0em; font-weight: bold;">Remuneration Details</asp:Label>
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


                    <div class="row mb-3">

                        <div class="col-lg-6 col-md-6">
                            <div class="form-group">
                                <label class="fw-bold mb-1">Select Type</label>
                                <asp:DropDownList ID="ddlType" runat="server"
                                    CssClass="form-control custom-dropdown"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                    <asp:ListItem Text="-- Select --" Value="" />
                                    <asp:ListItem Text="Monthly" Value="Monthly" />
                                    <asp:ListItem Text="Yearly" Value="Yearly" />
                                </asp:DropDownList>
                            </div>
                        </div>




                        <asp:Panel ID="pnlMonth" runat="server"
                            CssClass="col-lg-6 col-md-6"
                            Visible="false">
                            <div class="form-group">
                                <label class="fw-bold mb-1">Select Month</label>
                                <asp:DropDownList ID="ddlMonth" runat="server"
                                    CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select Month --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlYear" runat="server"
                            CssClass="col-lg-6 col-md-6"
                            Visible="false">
                            <div class="form-group">
                                <label class="fw-bold mb-1">Select Year</label>
                                <asp:DropDownList ID="ddlYear" runat="server"
                                    CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select Year --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </asp:Panel>

                    </div>






                    <div class="col-md-8 d-flex align-items-end">
                        <div class="d-flex gap-2">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnsearch_click" />
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnclear_click" />
                        </div>
                    </div>
                    <br />
                    <br />

                    <!-- GridView -->
                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false"
                                DataKeyNames="UserId" EnablePersistedSelection="true"
                                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gridview_Sorting"
                                Style="margin: 0 auto;" EmptyDataText="No records found.">
                                <Columns>
                                    <asp:TemplateField HeaderText="SR No" ItemStyle-Width="100px">
                                        <ItemTemplate>
                                            <%# (gridview.PageIndex * gridview.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:BoundField DataField="employeeCode" HeaderText="Employee Code" ItemStyle-Width="130px" Visible="true" />
                                    <%--                                    <asp:BoundField DataField="UserId" HeaderText="User Id" ItemStyle-Width="130px" Visible="false" />--%>
                                    <asp:BoundField DataField="Username" HeaderText="Username" ItemStyle-Width="130px" />
                                    <asp:BoundField DataField="MonthlyCTC" HeaderText="Monthly CTC" ItemStyle-Width="130px"
                                        HtmlEncode="false" HeaderStyle-CssClass="monthlyCol" />
                                    <asp:BoundField DataField="YearlyCTC" HeaderText="Yearly CTC" ItemStyle-Width="130px"
                                        HtmlEncode="false" HeaderStyle-CssClass="yearlyCol" />


                                </Columns>
                            </asp:GridView>

                            <asp:Label ID="lblMessage" runat="server"
                                ForeColor="Red"
                                Font-Bold="True"
                                Font-Size="Medium"
                                Visible="False"
                                CssClass="text-center d-block mt-3" />
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

