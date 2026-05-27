<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Addrole.aspx.cs" Inherits="HRMS.View.Modules.Addrole" %>
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

                    <input type="hidden" id="Roleid" name="Roleid" value="" />
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="RoleTitleLabel" CssClass="card-title mb-4" Style="font-size: 2.0em; font-weight: bold;">Add New Role</asp:Label>
                        <asp:Button runat="server" ID="Button3" Text="Back" CssClass="btn btn-secondary" OnClick="viewRoleClick" />
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-12">
                                <div class="mb-3 position-relative custom-dropdown-container">
                                    <label for="input-role">Create Role</label>
                                    <asp:TextBox ID="txt_role" runat="server" CssClass="form-control" MaxLength="15" onkeypress="return blockSpecialChar(event)" placeholder="Enter Role"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_role" runat="server" ControlToValidate="txt_role" InitialValue="" ErrorMessage="Please Enter Role" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                    <asp:RegularExpressionValidator ID="rev_role" runat="server" ControlToValidate="txt_role" ValidationExpression="^[a-zA-Z\s]*$" ErrorMessage="Please enter only characters" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>
                            <div>
                                <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success"  OnClick="save_roleclick" Text="Submit" CommandArgument="Submit" ValidationGroup="SaveValidationGroup" />
                                <asp:Button ID="btn_reset" runat="server" CssClass="btn btn-primary custom-clear-button" OnClick="ClearButton_Click" Text="Reset" CommandArgument="Reset" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">

        /$(document).ready(function () { $('.selectBox').SumoSelect(); });/
        function blockSpecialChar(e) {
            var k;
            document.all ? k = e.keyCode : k = e.which;
            return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || k == 55 || (k >= 48 && k <= 57) || k == 45);
        }
        function setTwoNumberDecimal(event) {
            this.value = parseFloat(this.value).toFixed(2);
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
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
</asp:Content>

