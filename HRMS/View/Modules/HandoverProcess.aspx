<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="HandoverProcess.aspx.cs" Inherits="HRMS.View.Modules.HandoverProcess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Handover Process List</asp:Label>
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>

                        </div>
                    </div>


                    <!-- GridView -->
                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <asp:GridView runat="server" ID="gvHandover" class="table custom-gridview" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand"
                                DataKeyNames="EmployeeResignationId" EnablePersistedSelection="true"
                                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gridview_Sorting"
                                Style="margin: 0 auto;" EmptyDataText="No records found.">
                                <Columns>

                                    <%-- SR NO --%>
                                    <asp:TemplateField HeaderText="SR No">
                                        <ItemTemplate>
                                            <%# (gvHandover.PageIndex * gvHandover.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserId" HeaderText="User Id" ItemStyle-Width="130px" Visible="false" />

                                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" />

                                    <asp:BoundField DataField="resignation_date" HeaderText="Resignation Date"
                                        DataFormatString="{0:yyyy-MM-dd}" />

                                    <asp:BoundField DataField="last_working_date_display" HeaderText="Last Working Date"
                                        DataFormatString="{0:yyyy-MM-dd}" />

                                    <asp:BoundField DataField="hr_status" HeaderText="Resignation Status" />

                                    <%--  <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkOpen"
                                                runat="server"
                                                Text="Open"
                                                CommandName="Open"
                                                CommandArgument='<%# Eval("EmployeeResignationId") %>'
                                                CssClass="btn btn-sm btn-primary" />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkOpen"
                                                runat="server"
                                                Text="Open"
                                                CommandName="Open"
                                                CommandArgument='<%# Eval("EmployeeResignationId") + "|" + Eval("UserId") %>'
                                                CssClass="btn btn-sm btn-primary" />
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
                icon: status.toLowerCase() === "success" ? "success" : "error",
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
            $('#<%= gvHandover.ClientID %> tr:has(td)').hide();

            if (searchTerm === '') {
                $('#<%= gvHandover.ClientID %> tr:has(td)').show();
            } else {
                $('#<%= gvHandover.ClientID %> tr:has(td)').filter(function () {
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

