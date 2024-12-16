<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="CollaboratorPage.aspx.cs" Inherits="API_DAMs.UI.CollaboratorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>

    <link href="CSS/view.css" rel="stylesheet" />

    
    <div class="container nav-spacer">
        <h2 class="text-center ">Find collaborator</h2>
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
                <tfoot>
                    <tr>
                        <th>User Name</th>
                        <th>User Email</th>
                        <th>User Tagline</th>
                        <th>Joined Date</th>
                        <th>User Details</th>
                        <th>Friend Request</th>
                    </tr>
                </tfoot>
            </table>
        </FooterTemplate>
    </asp:Repeater>


    </div>
    <script src="JS/viewAPI_Page.js"></script>
</asp:Content>
