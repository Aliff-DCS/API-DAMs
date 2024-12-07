<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="ViewDetailPage.aspx.cs" Inherits="API_DAMs.UI.ViewDetailPage" Async="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="CSS/Documentation.css" rel="stylesheet" />

    <div class="container nav-spacer">
        <h5 class="text-center ">API Details</h5>
    <div class="mb-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h4 mb-3">The API Details:</h2>
            <asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="EditButton_Click" />
        </div>

        <div class="form-group flex">
            <label for="API_Name">API Name:</label>
            <asp:TextBox ID="API_Name" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="form-group flex">
            <label for="Endpoint">API Endpoint:</label>
            <asp:TextBox ID="Endpoint" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="form-group flex">
            <label for="Param_req">Parameter Required:</label>
            <asp:TextBox ID="Param_req" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="form-group flex">
            <label for="Method_Type">API Method Type:</label>
            <asp:TextBox ID="Method_Type" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <asp:Panel ID="PostMethodPanel" runat="server" CssClass="post-method-panel" Visible="false">
            <div class="form-group flex">
                <label for="PostMethodText">POST Method:</label>
                <asp:TextBox ID="PostMethodText" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
            </div>
        </asp:Panel>

        <asp:Panel ID="JSONKey" runat="server" CssClass="post-method-panel" Visible="false">
            <div class="form-group flex">
                <label for="PostMethodText">JSON Key:</label>
                <asp:TextBox ID="JSONMethodText" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
            </div>
        </asp:Panel>

        <div class="form-group flex">
            <label for="Description">Description:</label>
            <asp:TextBox ID="Description" runat="server" CssClass="form-control white" ReadOnly="true" TextMode="MultiLine" Rows="4"></asp:TextBox>
        </div>

        <asp:Button ID="SaveEditButton" runat="server" Text="Save Edit" CssClass="btn btn-success mt-4" Visible="false" OnClick="SaveEditButton_Click" />

        <asp:PlaceHolder ID="ParameterPlaceholder" runat="server">
            <div class="parameter-container"  runat="server" id="test">
                <div class="form-group flex">
                    <label for="Output">Output:</label>
                    <asp:TextBox ID="Output" runat="server" CssClass="form-control MultiLine black" ReadOnly="true" TextMode="MultiLine" Rows="12"></asp:TextBox>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</div>
</asp:Content>
