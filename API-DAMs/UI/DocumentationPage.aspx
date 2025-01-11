<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="DocumentationPage.aspx.cs" Inherits="API_DAMs.UI.DocumentationPage" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <link href="CSS/Documentation.css" rel="stylesheet" />

    <div class="container nav-spacer">
            <h2 class="text-center ">API Documentation</h2>
            <div id="text-field" class="form-group">
                <label for="SC_text">Insert your code here:</label>
                <asp:TextBox ID="SC_text" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="20" Columns="50"></asp:TextBox>
            </div>

            <!-- Documentation Button -->
            <asp:Button runat="server" Text="Documentation" CssClass="btn btn-primary" OnClick="handleCode" OnClientClick="handleCodeJs();" />

            <!-- Code Details Form -->
            <asp:Panel ID="code_doc" CssClass="mt-4" runat="server" Visible="false">
                <h1 class="h4 text-center">Your Code Details:</h1>

                <!-- Platform Dropdown -->
                <div class="form-group flex">
                    <label for="Platform">Platform:</label>
                    <asp:DropDownList ID="Platform" CssClass="form-control" runat="server">
                        <asp:ListItem Value="Windows">Windows</asp:ListItem>
                        <asp:ListItem Value="Mac OS">Mac OS</asp:ListItem>
                        <asp:ListItem Value="Linux">Linux</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group flex">
                    <label for="Application">Select Application:</label>
                    <asp:DropDownList ID="Application" CssClass="form-control" runat="server" >
                    </asp:DropDownList>
                </div>

                <!-- Description -->
                <div class="form-group flex">
                    <label for="Description">Description:</label>
                    <asp:TextBox ID="Description" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                </div>
            </asp:Panel>

            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-success mt-3" OnClick="handleSubmit" Visible="false" />

    </div>
</asp:Content>
