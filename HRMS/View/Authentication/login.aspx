<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="HRMS.View.Authentication.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- Character Encoding -->
    <meta charset="UTF-8" />

    <!-- Viewport Settings for Mobile Responsiveness -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />

    <link rel="shortcut icon" href="../../assets/images/faviconicon.png" />
    <link href="../../assets/css/app.min.css" rel="stylesheet" type="text/css" />
    <link href="../../assets/css/icons.min.css" rel="stylesheet" type="text/css" />
    <style>
        .password-container {
            position: relative;
        }

        .toggle-password {
            position: absolute;
            top: 50%;
            right: 10px; /* Adjust the right distance as needed */
            transform: translateY(-50%);
            cursor: pointer;
        }

        .captcha-container {
            margin-bottom: 5px; /* Adjust the margin as needed */
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
                            <div class="row justify-content-center align-items-center">
                                <div class="col-12 text-center">
                                    <%--<img src="~/assets/images/alphonsol_logo.png" alt="Alphonsol Logo" class="mb-3" style="max-width: 100px;">--%>
                                    <div style="padding-top: 20px; padding-bottom: 0;">
                                        <img src="../../assets/images/alphonsol_logo.png" alt="" height="30" />
                                    </div>
                                    <br />
                                    <%-- <div class="text-primary p-4">
                                            <h4 class="text-primary d-inline-block">ALPHONSOL LOGIN</h4>
                                        </div>--%>
                                    <h6 class="d-inline-block">Please enter your credentials to sign in!</h6>
                                </div>
                            </div>

                            <br />
                            <div class="card-body pt-0">
                                <div class="p-2">
                                    <div class="alert alert-info" runat="server" id="divAlert" visible="false">
                                        <asp:Label Text="" ID="lblErrorMessager" runat="server" />
                                    </div>
                                    <div>
                                        <label for="input-userName" style="font-weight: 700; font-size: 120%">Username</label>
                                        <asp:TextBox ID="usernametxt" runat="server" CssClass="form-control" AutoCompleteType="Disabled" placeholder="Enter username"></asp:TextBox>

                                        <div>
                                            <asp:Label ID="lblUserNameError" runat="server" ForeColor="Red" Font-Size="Small" Visible="false" />
                                        </div>
                                    </div>
                                    <br />

                                    <div class="form-group">
                                        <label for="input-password" style="font-weight: 700; font-size: 120%">Password</label>
                                        <div class="password-container">
                                            <asp:TextBox ID="passwordtxt" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="Enter password"></asp:TextBox>
                                            <i class="toggle-password fas fa-eye" onclick="togglePasswordVisibility()"></i>
                                        </div>
                                        <div>
                                            <asp:Label ID="lblPasswordError" runat="server" ForeColor="Red" Font-Size="Small" Visible="false" />
                                        </div>
                                        <div class="invalid-tooltip" text="" id="Div1" runat="server"></div>
                                    </div>
                                    <br />
                                    <label for="input-captcha" style="font-weight: 700; font-size: 120%">Captcha</label>
                                    <div class="form-group d-flex align-items-center captcha-container">
                                        <asp:Image ID="Image2" runat="server" Height="72px" ImageUrl="~/View/Authentication/Captcha.aspx" Width="100%" />
                                        <button type="button" id="refreshCaptcha" class="btn btn-outline-secondary ms-2">
                                            <i class="fas fa-sync-alt"></i>
                                        </button>
                                    </div>

                                    <asp:Label runat="server" ID="Label1"></asp:Label>
                                    <asp:Label runat="server" ID="lblCaptchaMessage"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <asp:TextBox runat="server" ID="txtVerificationCode" class="form-control" placeholder="Enter Captcha" AutoComplete="off" />
                                    <div>
                                        <asp:Label ID="lblCaptchaError" runat="server" ForeColor="Red" Font-Size="Small" Visible="false" />
                                    </div>
                                    <div class="invalid-tooltip" text="" id="catchaError" runat="server"></div>
                                </div>
                                <br />
                                <%-- <div class="form-check">
                                         <asp:CheckBox ID="CheckBox1" runat="server" CssClass="form-check-input" />
                                         <asp:Label CssClass="form-check-label">Remember me</asp:Label>
                                     </div>--%>
                                <div class="mt-3 d-grid" style="font-weight: 700; font-size: 100%;">
                                    <asp:Button ID="loginButton" runat="server" Text="LOGIN" CssClass="btn btn-primary waves-effect waves-light"  Style="font-size: 1.3em; font-weight: bold;" OnClick="loginButton_Click"/>
                                </div>

                                <div class="mt-4 text-center">
                                    <a href="forgetPassword.aspx">Forget Password <i class="mdi mdi-lock me-1"></i></a>
                                </div>
                                <%-- <div class="mt-4 text-center">
                                    <a href="changePassword.aspx">Change Password <i class="mdi mdi-lock me-1"></i></a>
                                </div>--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
<script>
    window.onload = function () {
        // Add Cache-Control headers to prevent caching
        document.addEventListener('DOMContentLoaded', function () {
            document.getElementsByTagName('html')[0].style.display = 'none';
            window.location.replace(window.location.href);
            document.getElementsByTagName('html')[0].style.display = 'block';
        });
    };
</script>

<script>
    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('refreshCaptcha').addEventListener('click', function () {
            var imageUrl = document.getElementById('<%= Image2.ClientID %>').src;
            var newImageUrl = imageUrl.split('?')[0] + '?rand=' + new Date().getTime(); // Appending a random value to the URL to force refresh
            document.getElementById('<%= Image2.ClientID %>').src = newImageUrl;
        });
    });

    document.addEventListener('DOMContentLoaded', function () {
        var togglePassword = document.getElementById('togglePassword');
        var passwordField = document.getElementById('<%= passwordtxt.ClientID %>');

        togglePassword.addEventListener('click', function () {
            var fieldType = passwordField.getAttribute('type');
            if (fieldType === 'password') {
                passwordField.setAttribute('type', 'text');
                togglePassword.innerHTML = '<i class="far fa-eye-slash"></i>';
            } else {
                passwordField.setAttribute('type', 'password');
                togglePassword.innerHTML = '<i class="far fa-eye"></i>';
            }
        });
    });
</script>
<script>
    function togglePasswordVisibility() {
        var passwordInput = document.getElementById('<%= passwordtxt.ClientID %>');
        var icon = document.querySelector('.toggle-password');

        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            icon.classList.remove('fa-eye');
            icon.classList.add('fa-eye-slash');
        } else {
            passwordInput.type = 'password';
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        }
    }
</script>

</html>
