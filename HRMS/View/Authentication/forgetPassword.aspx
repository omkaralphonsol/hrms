<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forgetPassword.aspx.cs" Inherits="HRMS.View.Authentication.forgetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="../../assets/images/faviconicon.png" />
    <link href="../../assets/css/app.min.css" rel="stylesheet" type="text/css" />
    <link href="../../assets/css/icons.min.css" rel="stylesheet" type="text/css" />
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script type="text/javascript">
        function showDataSavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="account-pages my-5 pt-sm-5">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-md-8 col-lg-6 col-xl-5">
                        <div class="card overflow-hidden">
                            <div>

                                <div class="bg-primary-subtle">
                                    <div class="row">
                                        <div class="col-7 mx-auto">
                                            <div class="text-primary p-4">
                                                <h4 class="text-primary">Forget Password</h4>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--<br /><br />--%>
                            <div class="card-body pt-0">
                                <%--   <div class="auth-logo">
                                        <div class="avatar-md profile-user-wid mb-4">
                                            <span class="avatar-title rounded-circle bg-light">
                                                <img src="~/assets/images/logo-light.svg" alt="" class="rounded-circle" height="34" />
                                            </span>
                                        </div>
                                </div>--%>
                                <div class="p-2">
                                    <div class="alert alert-info" runat="server" id="divAlert" visible="false">
                                        <asp:Label Text="" ID="lblErrorMessager" runat="server" />
                                    </div>
                                    <br />
                                    <div>
                                        <label for="input-userName">Email-Id</label>
                                        <asp:TextBox ID="emailtxt" runat="server" CssClass="form-control" AutoCompleteType="Disabled" placeholder="Enter Your Email-id" required=""></asp:TextBox>
                                    </div>
                                    <br />
                                    <div id="div_otp" runat="server" visible="false">
                                        <div>
                                            <label for="input-userName">Verify OTP</label>
                                            <asp:TextBox ID="txt_verifyotp" runat="server" CssClass="form-control" AutoCompleteType="Disabled" placeholder="Enter Your OTP" required=""></asp:TextBox>
                                        </div>
                                        <%--<br />                                                       
<div class="mt-3 d-grid">
    <asp:Button ID="btn_verify" runat="server" Text="Verify OTP" CssClass="btn btn-primary waves-effect waves-light" OnClick="btn_verify_Click" />
</div>  --%>
                                    </div>
                                    <div class="mt-3 d-grid">
                                        <asp:Button ID="btn_sendmail" runat="server" Text="Send OTP" CssClass="btn btn-primary waves-effect waves-light" CommandArgument="Send OTP" OnClick="btn_Emailsend_Click" />
                                    </div>


                                    <div class="mt-4 text-center">
                                        <a href="login.aspx">LogIn here <i class="mdi mdi-lock me-1"></i></a>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

</body>
</html>
