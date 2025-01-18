<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="FileUploadPage.aspx.cs" Inherits="API_DAMs.UI.FileUploadPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="CSS/Documentation.css" rel="stylesheet" />

<div class="container nav-spacer">
    <h2 class="text-center">API Documentation</h2>
    
    <!-- File Upload Section -->
    <div id="file-upload-section" class="form-group mt-3">
        <label for="FileUpload">Upload your code file:</label>
        <asp:FileUpload ID="FileUpload" runat="server" CssClass="form-control" />
        <asp:Label ID="FileUploadError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
    </div>
    <div>
        <asp:HyperLink ID="UploadedFileLink" runat="server" CssClass="text-success" Visible="false"></asp:HyperLink>
    </div>
    
    <asp:HiddenField ID="HiddenFileContent" runat="server" />

    <!-- Extract Information Button -->
    <asp:Button runat="server" Text="Extract information" CssClass="btn btn-primary mt-3" OnClick="handleCode" />

    <!-- Code Details Form -->
    <asp:Panel ID="code_doc" CssClass="mt-4" runat="server" Visible="false">
        <h1 class="h4 text-center">Your File Details:</h1>

        <div class="form-group d-flex justify-content-between">
            <div class="flex-grow-1 me-2">
                <label for="Platform">Platform:</label>
                <asp:DropDownList ID="Platform" CssClass="form-control" runat="server">
                    <asp:ListItem Value="Windows">Windows</asp:ListItem>
                    <asp:ListItem Value="Mac OS">Mac OS</asp:ListItem>
                    <asp:ListItem Value="Linux">Linux</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="flex-grow-1 ms-2">
                <label for="Application">Select Application:</label>
                <asp:DropDownList ID="Application" CssClass="form-control" runat="server">
                </asp:DropDownList>
            </div>
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
