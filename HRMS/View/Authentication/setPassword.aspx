<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="setPassword.aspx.cs" Inherits="HRMS.View.Authentication.setPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title></title>
    <script src="../../assets/libs/jquery/jquery.min.js"></script>

    <link href="../../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />

    <link rel="shortcut icon" href="../../assets/images/faviconicon.png" />

    <link href="../../assets/css/app.min.css" rel="stylesheet" type="text/css" />

    <link href="../../assets/css/icons.min.css" rel="stylesheet" type="text/css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
<script>
    function showPassSavedMessage(status, remark) {
        Swal.fire({

            icon: status === "Success" ? "success" : "error",
            text: remark,
            timer: 4000,
            showConfirmButton: false
        });
    }
</script>

    <style>
        .password-container {
            position: relative;
        }

            .password-container input {
                padding-right: 40px; /* space for the icon */
            }

        .password-toggle-icon {
            position: absolute;
            top: 50%;
            right: 10px;
            transform: translateY(-50%);
            cursor: pointer;
            color: #aaa;
        }
    </style>
</head>

<body>

    <form id="form1" runat="server">

        <div class="account-pages my-5 pt-sm-5">

            <div class="container">

                <div class="row justify-content-center">

                    <div class="col-md-8 col-lg-6 col-xl-5">

                        <div class="card overflow-hidden">

                            <div class="bg-primary-subtle">

                                <div class="row">

                                    <div class="col-7 mx-auto">

                                        <div class="text-primary p-4">

                                            <h4 class="text-primary">Alphonsol Login</h4>

                                        </div>

                                    </div>

                                </div>

                            </div>

                            <br />
                            <br />

                            <div class="card-body pt-0">

                                <%--  <div class="auth-logo">

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

                                    <div class="password-container">
                                        <asp:TextBox ID="Password" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="Password" />
                                        <span class="password-toggle-icon" onclick="togglePasswordVisibility('<%= Password.ClientID %>', this)">
                                            <i class="fa fa-eye-slash"></i>
                                        </span>
                                    </div>


                                    <br />

                                     <div class="password-container">

                                        <asp:TextBox ID="newPassword" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="Confirm password" />
                                         <span class="password-toggle-icon" onclick="togglePasswordVisibility('<%= newPassword.ClientID %>', this)">
    <i class="fa fa-eye-slash"></i>
</span>
                                    </div>

                                    <br />

                                    <div class="mt-3 d-grid">

                                        <asp:Button ID="loginButton" runat="server" Text="Change" CssClass="btn btn-primary waves-effect waves-light" OnClick="btnchange_Click" />

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

<script>
    function togglePasswordVisibility(inputId, iconSpan) {
        var textbox = document.getElementById(inputId);
        var icon = iconSpan.querySelector('i');

        if (textbox.type === "password") {
            textbox.type = "text";
            icon.classList.remove("fa-eye-slash");
            icon.classList.add("fa-eye");
        } else {
            textbox.type = "password";
            icon.classList.remove("fa-eye");
            icon.classList.add("fa-eye-slash");
        }
    }
</script>


</html>
