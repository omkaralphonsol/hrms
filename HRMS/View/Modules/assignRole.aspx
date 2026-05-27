<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="assignRole.aspx.cs" Inherits="HRMS.View.Modules.assignRole" %>
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
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
<%--    <asp:UpdatePanel ID="UpdatePanel1" runat="server">--%>
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-center mb-4">
                               <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4" Style="font-size: 2.0em; font-weight: bold;">Assign Role User</asp:Label>
                                <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                            </div>

                            <input type="hidden" id="Userroledetailsid" name="Userroledetailsid" value="" />
                            <div class="alert alert-success ml-5 mr-5" runat="server" id="divAlert" visible="false">
                                <asp:Label Text="" ID="lblErrorMessage" runat="server" />

                            </div>

                            <div class="form-group mb-6">
                                <label for="input-roles">User Name</label>
                                <div class="mb-3 position-relative custom-dropdown-container">
                                    <asp:DropDownList ID="ddluser" runat="server" CssClass="form-control custom-dropdown" AutoPostBack="true">
                                        <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="ddluser"
                                        InitialValue="0" ErrorMessage="Please select user name"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>


                            <div class="form-group mb-6">
                                <label for="input-roles">User Role</label>
                                <div class="form-group mt-4 mt-lg-0 position-relative custom-dropdown-container">
                                    <asp:DropDownList ID="ddlrole" runat="server" CssClass="form-control custom-dropdown" AutoPostBack="true">
                                        <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvUserRole" runat="server" ControlToValidate="ddlrole"
                                        InitialValue="0" ErrorMessage="Please select user role"
                                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>
                            <br />
                            <div>
                                <asp:Button ID="btn_submit" runat="server" OnClick="save_assignroleclick" CssClass="btn btn-success" Text="Submit"
                                     CommandArgument="Submit" ValidationGroup="SaveValidationGroup" />
                                <asp:Button Id="btn_clear" CssClass="btn btn-primary custom-clear-button" Text="Reset" runat="server" OnClick="ClearButton_Click"  />
                            </div>

                        </div>
                    </div>
        </ContentTemplate>
   <%-- </asp:UpdatePanel>--%>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showAssignedRoleMessage(status, remark) {
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
    '<%= ddluser.ClientID %>'
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

