<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddBioxiasalaryform.aspx.cs" Inherits="HRMS.View.Modules.AddBioxiasalaryform" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" />
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://code.jquery.com/ui/1.13.2/jquery-ui.js"></script>

    <style>
        .salary-slip {
            padding: 20px;
            max-width: 900px;
            margin: auto;
            font-size: 14px;
        }

            .salary-slip table {
                width: 100%;
                border-collapse: collapse;
            }

            .salary-slip th, .salary-slip td {
                border: 1px solid #000;
                padding: 6px;
                text-align: left;
            }

            .salary-slip h5 {
                font-weight: bold;
            }

        .form-control-sm {
            display: inline-block;
            width: 150px;
            margin-left: 8px;
        }

        .salary-header td {
            padding: 6px 10px;
            border: none !important;
        }

        .netpay {
            font-weight: bold;
        }

        .fixed-input {
            width: 180px !important; /* adjust as per your design */
            display: inline-block;
        }

        .full-width {
            width: calc(100% - 20px) !important; /* reduce width by 20px */
            margin-left: 10px;
            margin-right: 10px;
        }


        .fixed-width {
            width: 180px; /* Keeps other inputs smaller */
            display: inline-block;
        }

        .table-input-cell {
            padding-left: 10px;
            padding-right: 10px;
        }

        .salary-slip td {
            padding: 6px 10px; /* equal left-right padding inside table cells */
        }

        .amount-input {
            width: 100%; /* fill available width */
            box-sizing: border-box; /* respect cell padding */
            margin: 0; /* no extra margin */
        }

        .netpay td {
            font-weight: 600;
            font-size: 0.9rem;
        }

        .inline-netpay {
            width: calc(100% - 96px); /* adjust so textbox ends same as amount-input */
            margin-left: 10px; /* same gap as other textboxes */
            margin-right: 0; /* no extra margin on right */
            display: inline-block;
            vertical-align: middle;
            box-sizing: border-box; /* ensures padding doesn’t overflow */
        }

        .month-dropdown {
            width: 150px !important; /* same as your textboxes */
            display: inline-block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">

                    <div class="salary-slip">
                        <h5 class="text-center">BIOXIA</h5>
                        <p class="text-center">
                            H-7, Rajlaxmi Commercial Complex, Ground Floor, Kalher, Tal - Bhiwandi, Dist - Thane 421302, Maharashtra, India
                        </p>

                        <!-- Header Info in Table Format -->
                        <table class="salary-header">
                            <tr>
                               <td><b>Pay Slip:</b></td>
                                <td>
                                    <asp:TextBox ID="txtpayslip" runat="server" CssClass="form-control form-control-sm" />
                                </td>
                                <td><b>Month:</b></td>
                                <td>
                                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control form-control-sm month-dropdown"></asp:DropDownList>
                                </td>
                                <td><b>Branch:</b></td>
                                <td>
                                    <asp:TextBox ID="txtBranch" runat="server" CssClass="form-control form-control-sm" /></td>
                            </tr>
                            <tr>
                                <td><b>Emp Name:</b></td>
                                <td>
                                    <asp:DropDownList ID="ddl_employeename" runat="server" CssClass="form-control form-control-sm month-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddl_employeename_SelectedIndexChanged" Enabled="true"></asp:DropDownList>
                                <td><b>Grade:</b></td>
                                <td>
                                    <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td><b>Department:</b></td>
                                <td>
                                    <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control form-control-sm" /></td>
                            </tr>
                            <tr>
                                <td><b>Emp Code:</b></td>
                                <td>
                                    <asp:TextBox ID="txtEmployeeId" runat="server" CssClass="form-control form-control-sm" ReadOnly="true"/></td>
                                <td><b>Designation:</b></td>
                                <td>
                                    <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control form-control-sm month-dropdown" Enabled="true"></asp:DropDownList>
                                <td><b>Division:</b></td>
                                <td>
                                    <asp:TextBox ID="txtDivision" runat="server" CssClass="form-control form-control-sm" /></td>
                            </tr>
                            <tr>
                                <td><b>ESIC No:</b></td>
                                <td>
                                    <asp:TextBox ID="txtESIC" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td><b>PF No:</b></td>
                                <td>
                                    <asp:TextBox ID="txtPFNo" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td><b>Joining Date:</b></td>
                                <td>
                                    <asp:TextBox ID="txtJoiningDate" runat="server"
                                        CssClass="form-control form-control-sm"
                                        TextMode="Date" />
                                </td>

                            </tr>
                            <tr>
                                <td><b>Days Paid:</b></td>
                                <td>
                                    <asp:TextBox ID="txtDaysPaid" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td><b>LWP/Absent:</b></td>
                                <td>
                                    <asp:TextBox ID="txtAbsent" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
                        <br />

                        <!-- Salary Table -->
                        <table class="salary-table table table-bordered">
                            <thead class="table-light">
                                <tr>
                                    <th>Earnings</th>
                                    <th>Amount</th>
                                    <th>Deductions & Recoveries</th>
                                    <th>Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Basic</td>
                                    <td>
                                        <asp:TextBox ID="txtBasic" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td>Professional Tax</td>
                                    <td>
                                        <asp:TextBox ID="txtProfTax" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                </tr>
                                <tr>
                                    <td>HRA</td>
                                    <td>
                                        <asp:TextBox ID="txtHRA" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td>TDS</td>
                                    <td>
                                        <asp:TextBox ID="txtTDS" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                </tr>
                                <tr>
                                    <td>Travel Allowance</td>
                                    <td>
                                        <asp:TextBox ID="txtTravel" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td>Others</td>
                                    <td>
                                        <asp:TextBox ID="txtOtherDeduction" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                </tr>
                                <tr>
                                    <td>Special Allowance</td>
                                    <td>
                                        <asp:TextBox ID="txtSpecialAllowance" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Mobile Reimbursement</td>
                                    <td>
                                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Bonus</td>
                                    <td>
                                        <asp:TextBox ID="txtBonus" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Incentive</td>
                                    <td>
                                        <asp:TextBox ID="txtIncentive" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Others</td>
                                    <td>
                                        <asp:TextBox ID="txtOtherEarnings" runat="server" CssClass="form-control form-control-sm amount-input" /></td>
                                    <td></td>
                                    <td></td>
                                </tr>

                                <!-- Totals row -->
                                <tr class="netpay">
                                     <td><b>Amount Total</b></td>
                                    <td>
                                        <asp:TextBox ID="txtTotalEarnings" runat="server" CssClass="form-control form-control-sm amount-input" />
                                    </td>
                                     <td><b>Amount Total</b></td>
                                    <td>
                                        <asp:TextBox ID="txtTotalDeductions" runat="server" CssClass="form-control form-control-sm amount-input" />
                                    </td>
                                </tr>




                                <!-- Keep Gross Pay, Tax Deduction & Net Pay in Deduction column -->
                                <tr>
                                    <td colspan="2"></td>
                                    <td>Gross Pay</td>
                                    <td>
                                        <asp:TextBox ID="txtGrossPay" runat="server" CssClass="form-control form-control-sm amount-input" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2"></td>
                                    <td>Professional Tax Deducted</td>
                                    <td>
                                        <asp:TextBox ID="txtTaxDeducted" runat="server" CssClass="form-control form-control-sm amount-input" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2"></td>
                                    <td>Net Pay</td>
                                    <td>
                                        <asp:TextBox ID="txtNetPay" runat="server" CssClass="form-control form-control-sm amount-input" />
                                    </td>
                                </tr>

                                <!-- Final bold Net Pay row -->
                                <tr class="final-netpay">
                                    <td colspan="4" style="font-weight: bold; padding: 6px 10px; white-space: nowrap;">Net Pay :
        <asp:TextBox ID="txtFinalNetPay" runat="server"
            CssClass="form-control form-control-sm inline-netpay" />
                                    </td>
                                </tr>




                            </tbody>
                        </table>


                        <p class="mt-2"><small>* This is computer generated Salary Slip, hence the authorized signature is not required.</small></p>


                        <div class="text-end mt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                            <asp:Button ID="btnClear" runat="server" Text="clear" CssClass="btn btn-primary custom-clear-button" OnClick="btnClear_Click" />
                        </div>

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
          // --- Earnings ---
          var basic = parseFloat(document.getElementById('<%= txtBasic.ClientID %>').value) || 0;
        var hra = parseFloat(document.getElementById('<%= txtHRA.ClientID %>').value) || 0;
        var special = parseFloat(document.getElementById('<%= txtSpecialAllowance.ClientID %>').value) || 0;
        var lta = parseFloat(document.getElementById('<%= txtTravel.ClientID %>').value) || 0;
        var mobile = parseFloat(document.getElementById('<%= txtMobile.ClientID %>').value) || 0;
        var bonus = parseFloat(document.getElementById('<%= txtBonus.ClientID %>').value) || 0;
        var incentive = parseFloat(document.getElementById('<%= txtIncentive.ClientID %>').value) || 0;
        var others = parseFloat(document.getElementById('<%= txtOtherEarnings.ClientID %>').value) || 0;

        // --- Deductions ---
        var profTax = parseFloat(document.getElementById('<%= txtProfTax.ClientID %>').value) || 0;
        var otherDeduction = parseFloat(document.getElementById('<%= txtOtherDeduction.ClientID %>').value) || 0;
        var profTaxDeducted = parseFloat(document.getElementById('<%= txtTaxDeducted.ClientID %>').value) || 0;

        // --- Calculations ---
        var totalEarnings = basic + hra + special + lta + mobile + bonus + incentive + others;
        var totalDeductions = profTax + otherDeduction + profTaxDeducted;

        var netPay = totalDeductions;   // <-- this is deductions
        var finalnetPay = totalEarnings - totalDeductions;
        var grosspay = totalEarnings;   // <-- GrossPay is equal to total earnings

        // --- Display in textboxes ---
        document.getElementById('<%= txtTotalEarnings.ClientID %>').value = totalEarnings.toFixed(2);
        document.getElementById('<%= txtTotalDeductions.ClientID %>').value = totalDeductions.toFixed(2);
        document.getElementById('<%= txtNetPay.ClientID %>').value = netPay.toFixed(2);
        document.getElementById('<%= txtFinalNetPay.ClientID %>').value = finalnetPay.toFixed(2);
        document.getElementById('<%= txtGrossPay.ClientID %>').value = grosspay.toFixed(2); // ? GrossPay shown as total earnings
    }

    // Attach events on page load
    window.onload = function () {
        // Earnings inputs
        document.getElementById('<%= txtBasic.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtHRA.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtSpecialAllowance.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtTravel.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtMobile.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtBonus.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtIncentive.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtOtherEarnings.ClientID %>').oninput = calculateSalary;

        // Deductions inputs
        document.getElementById('<%= txtProfTax.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtOtherDeduction.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtTaxDeducted.ClientID %>').oninput = calculateSalary;
      };
  </script>

    <script type="text/javascript">
        $(function () {
            $("#<%= txtJoiningDate.ClientID %>").datepicker({
            dateFormat: "yyyy-mm-dd",   // customize format
            changeMonth: true,
            changeYear: true,
            
        });
    });
    </script>

</asp:Content>
