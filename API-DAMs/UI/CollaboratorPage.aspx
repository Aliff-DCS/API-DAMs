<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="CollaboratorPage.aspx.cs" Inherits="API_DAMs.UI.CollaboratorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet">

    <link href="CSS/view.css" rel="stylesheet" />
<style>
    /* Apply border and border-radius to the entire table */
    #example {
        border: 2px solid black; /* Table border */
        border-radius: 8px; /* Rounded corners for the table */
        overflow: hidden; /* To ensure rounded corners show correctly */
    }

    /* Apply border and background to table headers */
    #example th {
        background-color: #007bff; /* Header background color */
        color: white; /* Text color for headers */
        border: 1px solid #ddd; /* Border for header cells */
        text-align: center; /* Optional: Align header text to the center */
    }

    /* Optional: Add border radius to table rows or cells */
    #example td, #example th {
        border-radius: 5px; /* Rounded corners for header and data cells */
    }
</style>
    
    <div class="container nav-spacer">
        <h2 class="text-center ">Find friends</h2>
       <asp:Repeater ID="rptResults" runat="server" OnItemCommand="rptResults_ItemCommand" OnItemDataBound="rptResults_ItemDataBound">
        <HeaderTemplate>
            <table id="example" class="table table-striped" style="width:100%">
                <thead>
                    <tr>
                        <th>User Name</th>
                        <th>User Email</th>
                        <th>User Tagline</th>
                        <th>Joined Date</th>
                        <th>User Details</th>
                        <th>Friend Request</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
            <td>
                <img src='<%# ResolveUrl(GetUserProfileImage(Eval("user_username").ToString())) %>' 
                     width="40" height="40" class="imgProfile" alt="Profile Image" />
                <%# Eval("user_username") %>
            </td>
            <td><%# Eval("user_email") %></td>
            <td><%# Eval("user_email") %></td>
            <td><%# Eval("user_joined_date", "{0:dd-MM-yyyy HH:mm:ss}") %></td>
            <td>
                <asp:LinkButton CommandName="Redirect" CommandArgument='<%# Eval("user_id") %>'
                                runat="server" CssClass="btn btn-link">View Details</asp:LinkButton>
            </td>
            <td>
                <asp:PlaceHolder ID="phFriendRequestActions" runat="server"></asp:PlaceHolder>
            </td>

        </tr>
    </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>


    </div>
    <script src="JS/viewAPI_Page.js"></script>
    <script>new DataTable('#example');</script>
</asp:Content>
