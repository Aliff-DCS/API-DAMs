<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignInPage.aspx.cs" Inherits="API_DAMs.UI.SignInPage" %>

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
        <a href="landingPage.aspx" class="button2">&lt&lt Back</a>
    </div>

    <div class="padding color">
        <a class="navbar-brand" href="#">
            <img src="../images/api_logo.png" width="60" height="60" class ="center"/> API DAMs 
        </a>
	</div>
    <form runat="server">
        <div class="container" id="container">
	        <div class="form-container sign-in-container">
		        <div class="form">
                    <h1 class="centered>">Sign In</h1>
			        <asp:Label runat="server" AssociatedControlID="txtEmailU">Email:</asp:Label>
                    <asp:TextBox ID="txtEmailU" runat="server" class="input" placeholder="Enter email or username"></asp:TextBox>
                    
                    <asp:Label runat="server" AssociatedControlID="txtPassword">Password:</asp:Label>
                    <asp:TextBox ID="txtPassword" runat="server" class="input" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                    
                    
                    <asp:Button ID="btnSignIn" runat="server" Text="SIGN IN" OnClick="SignIn_Click" CssClass="button" />
                    
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-danger"></asp:Label>
                </div>
	        </div>
            <%-- Animation --%>
		    <div class="overlay-container">
			    <div class="overlay">
				    <div class="overlay-panel overlay-right">
					    <h1>Don't have an account?</h1>
					    <p>Enter your personal details and start journey with us</p>
                        <asp:LinkButton class="button2 ghost" ID="signUp" runat="server" PostBackUrl="SignPage.aspx" >Sign Up</asp:LinkButton>
				    </div>
			    </div>
		    </div>

	    </div>
    </form>

    <footer>
        <div id="footer2" class="container-fluid">
            <div class="row">
                <div class="col-xs-12 col-sm-12 col-md-12 text-center">
                    <p style="color: whitesmoke">&copy All right Reversed. <a
                    class="footerlinks" href="#" target="_blank">API DAMs</a></p>
                </div>
            </div>
        </div>
    </footer>

    <script src="JS/SignPage.js"></script>
</body>
</html>