<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AssignRights.aspx.cs" Inherits="HRMS.View.Modules.AssignRights" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
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
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                          <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4" Style="font-size: 2.0em; font-weight: bold;">Assign Rights</asp:Label>
                        <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-group mb-4">
                                <label for="input-roles">User Role</label>
                                <div class="mb-3 position-relative custom-dropdown-container">
                                    <asp:DropDownList ID="ddlrole" runat="server" CssClass="form-control custom-dropdown" AutoPostBack="true">
                                        <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddlrole" runat="server" ControlToValidate="ddlrole" InitialValue="0" ErrorMessage="User Role is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>
                        </div>
                    </div>
                   <%-- <div class="row">--%>
                        <div class="col-lg-12">
                            <div class="form-group mb-4">
                                <label for="input-menu">Select Menu</label>
                                <div class="mb-3 position-relative custom-dropdown-container">
                                    <asp:DropDownList runat="server" CssClass="form-control custom-dropdown m-b" ID="ddlMenu" AutoPostBack="true" OnSelectedIndexChanged="ddlMenu_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddlMenu" runat="server" ControlToValidate="ddlMenu" InitialValue="" InitialValueText="" ErrorMessage="Select Menu" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="form-group mb-4">
                                <div id="lbl_submenu" runat="server" visible="false">
                                    <label for="input-submenu">Submenu</label>
                                    <div class="mb-3 position-relative custom-dropdown-container">
                                        <asp:DropDownList ID="ddl_submenu" runat="server" AutoPostBack="true" CssClass="form-control custom-dropdown">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfv_ddlSubMenu" runat="server" ControlToValidate="ddl_submenu" InitialValue="" InitialValueText="" ErrorMessage="Select SubMenu" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="d-flex gap-2">
                            <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success" Text="Submit" CommandArgument="Submit" OnClick="btn_submit_Click" ValidationGroup="SaveValidationGroup" />
                            <asp:Button ID="btn_reset" runat="server" CssClass="btn btn-primary custom-clear-button" Text="Reset"  OnClick="btn_reset_Click" Style="margin-right: 10px;" />
                        </div>
                    <%--</div>--%>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showRightSavedMessage(status, remark) {
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
    '<%= ddlrole.ClientID %>',
    '<%= ddlMenu.ClientID %>',
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
</asp:Content>

