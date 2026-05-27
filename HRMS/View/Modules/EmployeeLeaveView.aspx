<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeLeaveView.aspx.cs" Inherits="HRMS.View.Modules.EmployeeLeaveView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <div class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">
                        &nbsp;Employee Leave View
                    </div>
                    <div class="row mb-3">
                        <div class="col-12">
                            <div class="card">
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Leave</label>
                                                <asp:TextBox ID="txtLookupId" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Leave Type</label>
                                                <asp:TextBox ID="txtLeaveTypeId" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Leave From Date</label>
                                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Leave To Date</label>
                                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="mb-3">
                                                <label>Leave Description</label>
                                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" ReadOnly="true" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12">
                            <div class="card">
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="mb-3">
                                                <label>Verification Status</label>
                                                <asp:TextBox ID="txtApprovalStatus" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="mb-3">
                                                <label>Rejection Reason</label>
                                                <asp:TextBox ID="txtRejectionRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Approved From Date</label>
                                                <asp:TextBox ID="txtApprovedFromDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3">
                                                <label>Approved To Date</label>
                                                <asp:TextBox ID="txtApprovedToDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-3">
                        <div class="col-lg-6">
                            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
