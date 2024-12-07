<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="viewAPI.aspx.cs" Inherits="API_DAMs.UI.viewAPI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>

    <link href="CSS/view.css" rel="stylesheet" />
    
    <div class="container nav-spacer">
        <h5 class="text-center ">View your API</h5>
        <asp:Repeater ID="rptResults" runat="server" OnItemCommand="rptResults_ItemCommand">
            <HeaderTemplate>
                <table id="example" class="table table-striped" style="width:100%">
                    <thead>
                        <tr>
                            <th>No.</th>
                            <th>API Name</th>
                            <th>HTTP Method</th>
                            <th>API Description</th>
                            <th>API Endpoint</th>
                            <th>Date and Time</th>
                            <th>API Details</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Container.ItemIndex + 1 %></td>
                    <td><%# Eval("API_name") %></td>
                    <td><%# Eval("API_HTTP_method") %></td>
                    <td ><%#Eval("API_desc").ToString() %></td>
                    <td><%# Eval("API_endpoint") %></td>
                    <td><%# Eval("code_uploadDate", "{0:dd-MM-yyyy HH:mm:ss}") %></td>
                    <td>
                        <asp:LinkButton CommandName="Redirect" CommandArgument='<%# Eval("API_id") %>' runat="server" CssClass="btn btn-link">View Details</asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                    <tfoot>
                        <tr>
                            <th>No.</th>
                            <th>API Name</th>
                            <th>HTTP Method</th>
                            <th>API Description</th>
                            <th>API Endpoint</th>
                            <th>Date and Time</th>
                            <th>API Details</th>
                        </tr>
                    </tfoot>
                </table>
            </FooterTemplate>
        </asp:Repeater> 
    </div>
    <script src="JS/viewAPI_Page.js"></script>
</asp:Content>
