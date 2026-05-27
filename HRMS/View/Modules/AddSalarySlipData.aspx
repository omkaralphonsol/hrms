<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddSalarySlipData.aspx.cs" Inherits="HRMS.View.Modules.AddSalarySlipData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap + Boxicons -->
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
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


                    <%--  <div class="d-flex justify-content-between align-items-center mb-4">
                    
                        <div style="width: 75px;"></div>

                    
                        <div class="text-center flex-grow-1">
                            <h3 class="fw-bold mb-1">Alphonsol Pvt. Ltd.</h3>
                            <p class="mb-0">Address: High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601</p>
                        </div>

                     
                        <div>
                            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary"
                                Text="Back" OnClick="btnBack_Click" />
                        </div>
                    </div>--%>
                    <div class="position-relative mb-4">

                        <!-- Back Button (Right Side) -->
                        <div class="position-absolute top-0 end-0">
                            <asp:Button ID="btnBack" runat="server"
                                CssClass="btn btn-secondary"
                                Text="Back"
                                OnClick="btnBack_Click" />
                        </div>

                        <!-- Logo + Address (Centered) -->
                        <div class="text-center">
                            <img src="../../assets/images/alphonsol_logo.png"
                                alt="Alphonsol Logo"
                                style="height: 40px; width: auto; margin-bottom: 6px;" />

                            <p class="mb-0" style="font-size: 13px;">
                                Address: High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601
                            </p>
                        </div>

                    </div>


                    <!-- Employee & Month Info -->
                    <table class="table table-bordered">
                        <tr>
                            <td><strong>Payslip for the Month:</strong></td>
                            <td>
                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control custom-dropdown">

                                </asp:DropDownList>
                            </td>
                            <td><strong>Employee Name:</strong></td>
                            <td>

                                <asp:DropDownList ID="ddl_employeename" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_employeename_SelectedIndexChanged" Enabled="true">
                                </asp:DropDownList>
                            </td>
                        </tr>

                        <tr>
                            <td><strong>Designation:</strong></td>
                            <td>
                                <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control" Enabled="true">
                                </asp:DropDownList></td>
                            <td><strong>Employee ID:</strong></td>
                            <td>
                                <asp:TextBox ID="txtEmployeeId" runat="server" CssClass="form-control" Enabled="true" /></td>
                        </tr>
                        <tr>
                            <td><strong>Days Paid:</strong></td>
                            <td>
                                <asp:TextBox ID="txtDaysPaid" runat="server" CssClass="form-control" Enabled="true" /></td>
                            <td><strong>Days Present:</strong></td>
                            <td>
                                <asp:TextBox ID="txtDaysPresent" runat="server" CssClass="form-control" Enabled="true" /></td>
                        </tr>
                        <tr>
                            <td><strong>Absent:</strong></td>
                            <td>
                                <asp:TextBox ID="txtAbsent" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtAbsent_TextChanged"></asp:TextBox></td>
                            <td colspan="2"></td>
                        </tr>
                    </table>

                    <!-- Earnings & Deductions -->
                    <table class="table table-bordered">
                        <thead class="table-light">
                            <tr>
                                <th>Earnings</th>
                                <th>Amount</th>
                                <th>Deduction & Recoveries</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Basic Salary</td>
                                <td>
                                    <asp:TextBox ID="txtBasicSalary" runat="server" CssClass="form-control" Enabled="true" /></td>
                                <td>Professional Tax</td>
                                <td>
                                    <asp:TextBox ID="txtProfessionalTax" runat="server" CssClass="form-control" Enabled="true" /></td>
                            </tr>
                            <tr>
                                <td>House Rent Allowance</td>
                                <td>
                                    <asp:TextBox ID="txtHRA" runat="server" CssClass="form-control" Enabled="true" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Special Allowance</td>
                                <td>
                                    <asp:TextBox ID="txtSpecialAllowance" runat="server" CssClass="form-control" Enabled="true" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Leave Travel Allowance</td>
                                <td>
                                    <asp:TextBox ID="txtLTA" runat="server" CssClass="form-control" Enabled="true" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr class="fw-bold">
                                <td>Total Earnings</td>
                                <td>
                                    <asp:TextBox ID="txtTotalEarnings" runat="server" CssClass="form-control" Enabled="true" /></td>
                                <td>Total Deductions</td>
                                <td>
                                    <asp:TextBox ID="txtTotalDeductions" runat="server" CssClass="form-control" Enabled="true" /></td>
                            </tr>
                            <tr class="fw-bold">
                                <td colspan="2">Net Pay</td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtNetPay" runat="server" CssClass="form-control" Enabled="true" /></td>
                            </tr>
                        </tbody>
                    </table>

                    <!-- Footer -->
                    <p class="text-center mt-3">
                        <small>This is auto generated payslip and does not require signature</small>
                    </p>

                    <!-- Action Buttons -->
                    <div class="text-end mt-4">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="clear" CssClass="btn btn-primary custom-clear-button" OnClick="btnClear_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showsalarySavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
  </script>
    <script type="text/javascript">
        function calculateSalary() {
            // Parse all values or default to 0
            var basic = parseFloat(document.getElementById('<%= txtBasicSalary.ClientID %>').value) || 0;
            var hra = parseFloat(document.getElementById('<%= txtHRA.ClientID %>').value) || 0;
            var special = parseFloat(document.getElementById('<%= txtSpecialAllowance.ClientID %>').value) || 0;
            var lta = parseFloat(document.getElementById('<%= txtLTA.ClientID %>').value) || 0;
            var tax = parseFloat(document.getElementById('<%= txtProfessionalTax.ClientID %>').value) || 0;

            // Calculations
            var totalEarnings = basic + hra + special + lta;
            var totalDeductions = tax;
            var netPay = totalEarnings - totalDeductions;

            // Display in textboxes
            document.getElementById('<%= txtTotalEarnings.ClientID %>').value = totalEarnings.toFixed(2);
            document.getElementById('<%= txtTotalDeductions.ClientID %>').value = totalDeductions.toFixed(2);
            document.getElementById('<%= txtNetPay.ClientID %>').value = netPay.toFixed(2);
        }

        // Attach events on page load
        window.onload = function () {
            document.getElementById('<%= txtBasicSalary.ClientID %>').oninput = calculateSalary;
            document.getElementById('<%= txtHRA.ClientID %>').oninput = calculateSalary;
            document.getElementById('<%= txtSpecialAllowance.ClientID %>').oninput = calculateSalary;
            document.getElementById('<%= txtLTA.ClientID %>').oninput = calculateSalary;
            document.getElementById('<%= txtProfessionalTax.ClientID %>').oninput = calculateSalary;
        };
    </script>

</asp:Content>
