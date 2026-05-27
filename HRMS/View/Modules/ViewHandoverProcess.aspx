<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ViewHandoverProcess.aspx.cs" Inherits="HRMS.View.Modules.ViewHandoverProcess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .handover-check {
            display: flex;
            align-items: center;
            gap: 12px; /* space between checkbox & text */
            padding: 12px 16px;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            background: #fff;
        }

            .handover-check .form-check-input {
                margin-right: 10px;
            }

            .handover-check label {
                font-weight: 600;
                cursor: pointer;
            }

        .handover-checkbox {
            transform: scale(1.3);
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
                            Style="font-size: 2.0em; font-weight: bold;">Handover Process</asp:Label>
                         <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                    </div>
                    <hr class="mb-4" />

                    <asp:HiddenField ID="hfResignationId" runat="server" />
                    <asp:HiddenField ID="hfUserId" runat="server" />

                    <!-- Checklist -->
                    <div class="row g-4">

                        <div class="col-md-6">
                            <div class="form-check handover-check">
                                <asp:CheckBox ID="chkPendriveBackup" runat="server" CssClass="form-check-input handover-checkbox" />
                                <label class="form-check-label">
                                    💾 Pendrive with Backup
                                </label>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-check handover-check">
                                <asp:CheckBox ID="chkLaptopCharger" runat="server" CssClass="form-check-input handover-checkbox" />
                                <label class="form-check-label">
                                    💻 Laptop with Charger
                                </label>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-check handover-check">
                                <asp:CheckBox ID="chkContactDetails" runat="server" CssClass="form-check-input handover-checkbox" />
                                <label class="form-check-label">
                                    📞 Contact Details Shared
                                </label>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-check handover-check">
                                <asp:CheckBox ID="chkDiary" runat="server" CssClass="form-check-input handover-checkbox" />
                                <label class="form-check-label">
                                    📘 Diary / Documents
                                </label>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-check handover-check">
                                <asp:CheckBox ID="chkIdCard" runat="server" CssClass="form-check-input handover-checkbox" />
                                <label class="form-check-label">
                                    🪪 ID Card Submitted
                                </label>
                            </div>
                        </div>

                    </div>

                    <!-- Remarks -->
                    <div class="row mt-4">
                        <div class="col-md-12">
                            <label class="fw-semibold mb-2">HR Remark</label>
                            <asp:TextBox ID="txtRemark" runat="server"
                                CssClass="form-control rounded-3"
                                TextMode="MultiLine"
                                Rows="3"
                                placeholder="Enter manager remarks...">
                            </asp:TextBox>
                        </div>
                    </div>

                
                    <div class="row mt-4">
                        <div class="col-md-12 text-start">
                            <asp:Button ID="btnSave" runat="server"
                                Text="Submit"
                                CssClass="btn btn-success"
                                ValidationGroup="SaveValidationGroup"
                                OnClick="btnSave_Click"/>

                            <asp:Button ID="btnClear" runat="server"
                                Text="Reset"
                                CssClass="btn btn-primary custom-clear-button"
                                OnClick="btnClear_Click" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

      <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
  <script>
      function showSavedMessage(status, remark) {
          Swal.fire({

              icon: status === "Success" ? "success" : "error",
              text: remark,
              timer: 4000,
              showConfirmButton: false
          });
      }
  </script>
</asp:Content>
