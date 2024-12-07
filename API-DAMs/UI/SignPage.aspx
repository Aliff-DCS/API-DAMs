<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignPage.aspx.cs" Inherits="API_DAMs.UI.SignPage" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Sign Up / Sign In</title>
    
	<link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
	<link href="CSS/SignPage.css" rel="stylesheet" />
</head>
<body>
    <div class="absolute">
        <a href="landingPage.aspx" class="button2">&lt;&lt; Back</a>
    </div>
    <div class="padding color">
        <a class="navbar-brand" href="landingPage.aspx">
            <img src="../images/api_logo.png" width="60" height="60" class="center"/> API DAMs 
        </a>
	</div>
    <form runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <%-- Sign up --%>
	    <div class="container" id="container">
	        <div class="form-container sign-in-container">
		        <div class="form">
                    <h1 class="centered>">Sign Up</h1>
			        <asp:Label runat="server" AssociatedControlID="txtEmail">Email:</asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server" class="input" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
                    
                    <asp:Label runat="server" AssociatedControlID="txtPassword">Password:</asp:Label>
                    <asp:TextBox ID="txtPassword" runat="server" class="input" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                    
                    <asp:Label runat="server" AssociatedControlID="txtRePassword">Re-Password:</asp:Label>
                    <asp:TextBox ID="txtRePassword" runat="server" class="input" TextMode="Password" placeholder="Re-enter your password"></asp:TextBox>
                    
                    <asp:Label runat="server" AssociatedControlID="txtUsername">Username:</asp:Label>
                    <asp:TextBox ID="txtUsername" runat="server" class="input" placeholder="Enter your username"></asp:TextBox>
                    
                    <asp:Button ID="btnSignUp" runat="server" Text="SIGN UP" OnClick="SignUp_Click" CssClass="button" />
                    
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger"></asp:Label>
                </div>
	        </div>
            <%-- Animation --%>
		    <div class="overlay-container">
			    <div class="overlay">
				    <div class="overlay-panel overlay-right">
					    <h1>Don't have an account?</h1>
					    <p>Enter your personal details and start journey with us</p>
                        <asp:LinkButton class="button2 ghost" ID="signUp" runat="server" PostBackUrl="SignInPage.aspx">Sign In</asp:LinkButton>
				    </div>
			    </div>
		    </div>
	    </div>
    </form>
    <footer>
        <div id="footer2" class="container-fluid">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 text-center">
                    <p style="color: whitesmoke">&copy; All right Reversed. <a
                    class="footerlinks" href="landingPage.aspx" target="_blank">API DAMs</a></p>
                </div>
            </div>
        </div>
    </footer>
    <script src="JS/SignPage.js"></script>
</body>
</html>