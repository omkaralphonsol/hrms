<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="UploadExcelforsalary.aspx.cs" Inherits="HRMS.View.Modules.UploadExcelforsalary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Upload Excel</asp:Label>
                        <%-- <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary"
                            Text="Back" OnClick="btnBack_Click" />--%>
                    </div>

                    <div class="row">


                        <div class="col-lg-6">
                            <div class="form-group mb-6">
                                <label class="form-label">Upload Attachments:</label>
                                <asp:FileUpload ID="fileUpload" CssClass="form-control" runat="server"
                                    AllowMultiple="true" accept=".xlsx,.xls" />
                                <asp:Label ID="lblFileUploadError" runat="server" CssClass="text-danger"></asp:Label>
                                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger d-block"></asp:Label>
                            </div>
                        </div>
                    </div>



                    <%--                <div class="row">
     <div class="col-md-6">
         <div class="form-group mb-4">
             <label class="form-label">Uploaded Documents :</label>

             <asp:GridView ID="gvAttachments" EmptyDataText="No Files Uploaded" CssClass="table table-bordered table-hover" runat="server" OnRowCommand="gvAttachments_RowCommand" AutoGenerateColumns="false">
                 <Columns>
                     <asp:TemplateField HeaderText="File Name">
                         <ItemTemplate>
                             <div style="white-space: normal; word-wrap: break-word; width: 200px;">
                                 <%# Eval("FileName") %>
                             </div>
                         </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Actions">
                         <ItemTemplate>
                             <a href='<%# Eval("FilePath") %>' target="_blank" class="btn btn-sm btn-primary"><i class="fa fa-eye"></i></a>
                             <asp:LinkButton ID="btnDelete" CssClass="btn btn-sm btn-danger" runat="server" CommandName="DeleteFile" CommandArgument='<%# Eval("FileId") %>'>
                               <i class="fa fa-trash"></i>
                             </asp:LinkButton>
                         </ItemTemplate>
                     </asp:TemplateField>
                 </Columns>
             </asp:GridView>
             <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger small"></asp:Label>
         </div>
     </div>
 </div>--%>
                    <div class="mt-4">
                        <asp:Button ID="btnUpload" CssClass="btn btn-success" runat="server"
                            Text="Upload" OnClick="btnUpload_Click" />
                        <asp:Button CssClass="btn btn-primary custom-clear-button" runat="server" ID="btn_rest" Text="Reset" OnClick="ClearButton_Click" />

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
        function showsalarySavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>
    <script>
        function blockNumbersAndSpecialChar(event) {
            var key = event.key;
            var regex = /^[a-zA-Z\s]$/;
            return regex.test(key);
        }

        function capitalizeFirstLetter(input) {
            if (input.value.length > 0) {
                input.value = input.value.charAt(0).toUpperCase() + input.value.slice(1);
            }
        }

        function checkNoLeadingSpace(input) {
            input.value = input.value.replace(/^\s+/, '');
        }
    </script>

    <script>
        function checkNoLeadingSpace(input) {
            input.value = input.value.trimStart();
            if (input.value.startsWith(" ")) {
                input.value = input.value.trimStart();
            }
        }
</script>




</asp:Content>
