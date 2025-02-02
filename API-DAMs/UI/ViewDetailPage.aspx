<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="ViewDetailPage.aspx.cs" Inherits="API_DAMs.UI.ViewDetailPage" Async="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>

    <link href="CSS/Documentation.css" rel="stylesheet" />

    <style>
        #collaborationModal .modal-dialog,
        #addCollaboratorPopup .modal-dialog {
            max-width: 600px;
        }

        .collaborator-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
        }
        .close{
            padding:10px;
        }
        #example_info{
            display:none !important;
        }
        .dt-paging{
            display:none !important;
        }
        .dt-paging{
            display:none !important;
        }
        .modal-content{
            width: 900px !important;
            position:absolute;
            left: -28% !important;
        }
        .modal-lg{
            align-items:center;
            
        }
        /* Ensure the label is on top of the TextBox */
        .form-group label {
            display: block; /* Make the label take up the full width */
            margin-bottom: 5px; /* Add some spacing between the label and the TextBox */
        }

        /* Style the small TextBox for the API Invoke Count */
        .small-textbox {
            width: 100%; /* Take up the full width of the parent container */
            max-width: 100px; /* Set a maximum width for the TextBox */
            text-align: center; /* Center the text inside the TextBox */
            font-size: 0.875rem; /* Smaller font size for the TextBox */
        }

        /* Adjust the position of the API Invoke Count box */
        .col-2 .form-group {
            margin-top: 30px; /* Lower the box by adding margin */
        }
    </style>

    <div class="container nav-spacer">
        <h2 class="text-center ">API Details</h2>
        <small class="d-block text-center mt-2">
            <asp:Label ID="lblPerm" runat="server"></asp:Label>
        </small>

    <div class="mb-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="h4 mb-3">The API Details:</h2>
            <asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="EditButton_Click" Visible="false" />
        </div>

        <div class="form-group flex">
            <label for="API_Name">API Name:</label>
            <asp:TextBox ID="API_Name" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="form-group flex">
            <label for="Endpoint">API Endpoint:</label>
            <asp:TextBox ID="Endpoint" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
        </div>
        <div class="row">
            <!-- Parameter Required -->
            <div class="col-md-4">
                <div class="form-group">
                    <label for="Param_req">Parameter Required:</label>
                    <asp:TextBox ID="Param_req" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <!-- API Method Type -->
            <div class="col-md-4">
                <div class="form-group">
                    <label for="Method_Type">API Method Type:</label>
                    <asp:TextBox ID="Method_Type" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <!-- POST Method Panel -->
            <div class="col-md-4">
                <asp:Panel ID="PostMethodPanel" runat="server" CssClass="form-group post-method-panel" Visible="false">
                    <label for="PostMethodText">POST Method:</label>
                    <asp:TextBox ID="PostMethodText" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
                </asp:Panel>
            </div>
        </div>

        <asp:Panel ID="JSONKey" runat="server" CssClass="post-method-panel" Visible="false">
            <div class="form-group flex">
                <label for="PostMethodText">JSON Key:</label>
                <asp:TextBox ID="JSONMethodText" runat="server" CssClass="form-control white" ReadOnly="true"></asp:TextBox>
            </div>
        </asp:Panel>

        <div class="row">
            <!-- Description (90% width) -->
            <div class="col-10">
                <div class="form-group">
                    <label for="Description">Description:</label>
                    <asp:TextBox ID="Description" runat="server" CssClass="form-control white" ReadOnly="true" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
            </div>

            <!-- API Invoke Count (10% width) -->
            <div class="col-2">
                <div class="form-group" style="margin-top: 30px;"> <!-- Adjust margin-top to lower the box -->
                    <label for="InvokeCount">API Invoke Count:</label>
                    <asp:TextBox ID="InvokeCount" runat="server" CssClass="form-control white small-textbox" ReadOnly="true"></asp:TextBox>
                </div>
            </div>
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


    <script src="JS/viewAPI_Page.js"></script>

</asp:Content>
