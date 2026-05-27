<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddImsetsalaryForm.aspx.cs" Inherits="HRMS.View.Modules.AddImsetsalaryForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .inline-netpay {
            width: calc(100% - 96px); /* adjust so textbox ends same as amount-input */
            margin-left: 10px; /* same gap as other textboxes */
            margin-right: 0; /* no extra margin on right */
            display: inline-block;
            vertical-align: middle;
            box-sizing: border-box; /* ensures padding doesn’t overflow */
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">

                    <!-- Company Header -->
                    <div class="text-center mb-3">
                        <h5><strong>IMSET</strong></h5>
                        <p>H-8, Rajlaxmi Commercial Complex, Ground Floor, Kalher, Tal - Bhiwandi, Dist - Thane 421302, Maharashtra, India</p>
                    </div>

                    <!-- Employee & Salary Details -->
                    <table class="table table-borderless">
                        <tr>
                            <td><b>Pay Slip:</b></td>
                            <td>
                                <asp:TextBox ID="txtPaySlip" runat="server" CssClass="form-control form-control-sm" /></td>
                            <td><b>Month:</b></td>
                            <td>
                                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control form-control-sm month-dropdown"></asp:DropDownList>
                            <td><b>Branch:</b></td>
                            <td>
                                <asp:TextBox ID="txtBranch" runat="server" CssClass="form-control form-control-sm" /></td>
                        </tr>
                        <tr>
                            <td><b>Grade:</b></td>
                            <td>
                                <asp:TextBox ID="txtGrade" runat="server" CssClass="form-control form-control-sm" /></td>
                            <td><b>Emp Name:</b></td>
                            <td>
                                 <asp:DropDownList ID="ddl_employeename" runat="server" CssClass="form-control form-control-sm month-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddl_employeename_SelectedIndexChanged" Enabled="true"></asp:DropDownList>
                            <td><b>Department:</b></td>
                            <td>
                                <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control form-control-sm" /></td>
                        </tr>
                        <tr>
                            <td><b>Designation:</b></td>
                            <td>
                                 <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control form-control-sm month-dropdown" Enabled="true"></asp:DropDownList>
                            <td><b>Days Paid:</b></td>
                            <td>
                                <asp:TextBox ID="txtDaysPaid" runat="server" CssClass="form-control form-control-sm" /></td>
                            <td><b>LWP/Absent:</b></td>
                            <td>
                                <asp:TextBox ID="txtAbsent" runat="server" CssClass="form-control form-control-sm" /></td>
                           
                        </tr>
                     <%-- <tr id="trEmployeeId" runat="server" visible="false">
    <td><strong>Employee ID:</strong></td>
    <td>
        <asp:TextBox ID="txtEmployeeId" runat="server" CssClass="form-control" ReadOnly="true" />
    </td>
</tr>--%>


                    </table>

                    <!-- Earnings and Deductions -->
                    <table class="table table-bordered">
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
                                    <asp:TextBox ID="txtBasic" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td>Professional Tax</td>
                                <td>
                                    <asp:TextBox ID="txtProfessionalTax" runat="server" CssClass="form-control form-control-sm" /></td>
                            </tr>
                            <tr>
                                <td>HRA</td>
                                <td>
                                    <asp:TextBox ID="txtHRA" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td>TDS</td>
                                <td>
                                    <asp:TextBox ID="txtTDS" runat="server" CssClass="form-control form-control-sm" Text="0.00" /></td>
                            </tr>
                            <tr>
                                <td>Travel Allowance</td>
                                <td>
                                    <asp:TextBox ID="txtTravelAllowance" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td>Others</td>
                                <td>
                                    <asp:TextBox ID="txtOtherDeductions" runat="server" CssClass="form-control form-control-sm" Text="0.00" /></td>
                            </tr>
                            <tr>
                                <td>Special Allowance</td>
                                <td>
                                    <asp:TextBox ID="txtSpecialAllowance" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Mobile Reimbursement</td>
                                <td>
                                    <asp:TextBox ID="txtMobileReimb" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Bonus</td>
                                <td>
                                    <asp:TextBox ID="txtBonus" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Incentive</td>
                                <td>
                                    <asp:TextBox ID="txtIncentive" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>Others</td>
                                <td>
                                    <asp:TextBox ID="txtOtherEarnings" runat="server" CssClass="form-control form-control-sm" /></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td><b>Amount Total</b></td>
                                <td>
                                    <asp:TextBox ID="txtTotalEarnings" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" /></td>
                                <%--<td><b>Total Deductions</b></td>
                                <td><asp:TextBox ID="txtTotalDeductions" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" /></td>--%>
                            </tr>
                            <tr>
                                <td colspan="2" class="text-end"><b>Gross Pay</b></td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtGrossPay" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" /></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="text-end"><b>Net Pay</b></td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtNetPay" runat="server" CssClass="form-control form-control-sm fw-bold" ReadOnly="true" /></td>
                            </tr>
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
                        <asp:Button ID="btnClear" runat="server" Text="clear" CssClass="btn btn-primary custom-clear-button"  OnClick="btnClear_Click"  />
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
        var lta = parseFloat(document.getElementById('<%= txtTravelAllowance.ClientID %>').value) || 0;
        var mobile = parseFloat(document.getElementById('<%= txtMobileReimb.ClientID %>').value) || 0;
        var bonus = parseFloat(document.getElementById('<%= txtBonus.ClientID %>').value) || 0;
        var incentive = parseFloat(document.getElementById('<%= txtIncentive.ClientID %>').value) || 0;
        var others = parseFloat(document.getElementById('<%= txtOtherEarnings.ClientID %>').value) || 0;

        // --- Deductions ---
        var profTax = parseFloat(document.getElementById('<%= txtProfessionalTax.ClientID %>').value) || 0;
        var otherDeduction = parseFloat(document.getElementById('<%= txtOtherDeductions.ClientID %>').value) || 0;
          var TDS = parseFloat(document.getElementById('<%= txtTDS.ClientID %>').value) || 0;
        // --- Calculations ---
        var totalEarnings = basic + hra + special + lta + mobile + bonus + incentive + others;
        var grossPay = totalEarnings;   // ? Gross Pay = Total Earnings
        var totalDeductions = profTax + otherDeduction+TDS
          var finalNetPay = grossPay - totalDeductions; // ? Correct Net Pay
          var netPay = totalDeductions;

        // --- Display in textboxes ---
        document.getElementById('<%= txtTotalEarnings.ClientID %>').value = totalEarnings.toFixed(2);
        document.getElementById('<%= txtGrossPay.ClientID %>').value = grossPay.toFixed(2); // ? Added
          document.getElementById('<%= txtNetPay.ClientID %>').value = netPay.toFixed(2);
        document.getElementById('<%= txtFinalNetPay.ClientID %>').value = finalNetPay.toFixed(2);
    }

    // Attach events on page load
    window.onload = function () {
        // Earnings inputs
        document.getElementById('<%= txtBasic.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtHRA.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtSpecialAllowance.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtTravelAllowance.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtMobileReimb.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtBonus.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtIncentive.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtOtherEarnings.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtTDS.ClientID %>').oninput = calculateSalary;
        // Deductions inputs
        document.getElementById('<%= txtProfessionalTax.ClientID %>').oninput = calculateSalary;
        document.getElementById('<%= txtOtherDeductions.ClientID %>').oninput = calculateSalary;
      };
  </script>

</asp:Content>

