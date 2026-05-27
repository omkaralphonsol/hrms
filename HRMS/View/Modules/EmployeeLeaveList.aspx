<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeLeaveList.aspx.cs" Inherits="HRMS.View.Modules.EmployeeLeaveList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .custom-gridview {
            border-left: none;
            border-right: none;
        }

        .custom-gridview th,
        .custom-gridview td {
            border-left: none;
            border-right: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <div class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">
                        &nbsp;Employee Leave List
                    </div>

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="table-responsive">
                                <asp:GridView
                                    runat="server"
                                    ID="gridview"
                                    CssClass="table custom-gridview"
                                    AutoGenerateColumns="false"
                                    OnRowCommand="gridview_RowCommand"
                                    EmptyDataText="No leave requests found.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SR No" ItemStyle-Width="80px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="emp_name" HeaderText="Employee Name" ItemStyle-Width="220px" />
                                        <asp:BoundField DataField="approval_status" HeaderText="Approval Status" ItemStyle-Width="150px" />
                                        <asp:BoundField DataField="request_date" HeaderText="Request Date" ItemStyle-Width="140px" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkView"
                                                    runat="server"
                                                    CommandName="viewLeave"
                                                    CommandArgument='<%# Eval("leave_id") + "|" + Eval("emp_id") %>'
                                                    title="View Leave">
                                                    <i class="fa fa-eye"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
