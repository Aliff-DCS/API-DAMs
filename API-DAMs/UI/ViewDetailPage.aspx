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
    </style>

    <div class="container nav-spacer">
        <h2 class="text-center ">API Details</h2>
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

        <!-- Collaboration Button -->
        <asp:Button ID="CollaborationButton" runat="server" Text="Collaboration" CssClass="btn btn-info mt-4" OnClientClick="showCollaborationModal(); return false;" />

        <!-- Collaboration Modal -->
        <div id="collaborationModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="collaborationModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header d-flex justify-content-between align-items-center">
                        <h5 class="modal-title" id="collaborationModalLabel">Collaborator</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>

                    <div class="modal-body">
                        <!-- Collaborator List -->
                       <asp:Repeater ID="CollaboratorRepeater" runat="server" OnItemCommand="rptResults_ItemCommand">
                            <HeaderTemplate>
                                <table id="example" class="table table-striped" style="width:100%">
                                    <thead>
                                        <tr>
                                            <th>Collaborator Name</th>
                                            <th>Permission</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("CollaboratorName") %></td>
                                    <td>
                                        <!-- Permission Display -->
                                        <asp:Label ID="lblPermission" runat="server" Text='<%# Eval("Permission") %>' />
                                        <asp:DropDownList ID="ddlPermission" runat="server" Visible="false" CssClass="form-select form-select-sm">
                                            <asp:ListItem Value="Read">Read</asp:ListItem>
                                            <asp:ListItem Value="Write">Write</asp:ListItem>
                                            <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <!-- Action Buttons -->
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit Permission" CommandName="Edit" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' CssClass="btn btn-primary btn-sm" />
                                        <asp:Button ID="btnSave" runat="server" Text="Save" CommandName="Save" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' Visible="false" CssClass="btn btn-success btn-sm" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' Visible="false" CssClass="btn btn-secondary btn-sm" />
                                        <asp:Button ID="btnRemove" runat="server" Text="Remove Collaborator" CommandName="Remove" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' Visible="false" CssClass="btn btn-danger btn-sm" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                    <tfoot>
                                    </tfoot>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="modal-body">
                        <!-- Button to toggle the dropdown -->
                        <button class="btn btn-primary mb-3" type="button" data-bs-toggle="collapse" data-bs-target="#collapsibleContent" aria-expanded="false" aria-controls="collapsibleContent">
                            +Add Collaborator+
                        </button>

                        <div class="collapse" id="collapsibleContent">
                            <div class="card card-body">
                                <h5>Available Friends:</h5>
                                <asp:Repeater ID="FriendsRepeater" runat="server" OnItemCommand="FriendsRepeater_ItemCommand">
                                    <ItemTemplate>
                                        <div class="friend-item d-flex justify-content-between align-items-center mb-2">
                                            <span>
                                                <%# Eval("user_username") %> 
                                            </span>
                                            <asp:Button ID="AddCollaboratorButton" runat="server" 
                                                        Text="Add Collaborator" 
                                                        CssClass="btn btn-success btn-sm" 
                                                        CommandName="AddCollaborator" 
                                                        CommandArgument='<%# Eval("user_id") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <div>
                            <asp:Label ID="SuccessMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
                            <asp:Label ID="ErrorMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>



                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="SaveCollaborationButton" runat="server" Text="Save" CssClass="btn btn-success" OnClick="SaveCollaborationButton_Click" />
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
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


    <script>
        // Function to open the modal and set the state in local storage
        function showCollaborationModal() {
            $('#collaborationModal').modal('show');
            localStorage.setItem('modalOpen', 'true');
        }

        // Event to clear the modal state when it is closed
        $('#collaborationModal').on('hidden.bs.modal', function () {
            localStorage.removeItem('modalOpen');
        });

        // Check local storage on page load to reopen the modal if needed
        $(document).ready(function () {
            if (localStorage.getItem('modalOpen') === 'true') {
                $('#collaborationModal').modal('show');
            }
        });
    </script>


    <script src="JS/viewAPI_Page.js"></script>

</asp:Content>
