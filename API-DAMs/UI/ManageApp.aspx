<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="ManageApp.aspx.cs" Inherits="API_DAMs.UI.ManageApp" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet">

    <link href="CSS/view.css" rel="stylesheet" />
    <style>
        .close{
            padding:10px;
        }
        /* Apply border and border-radius to the entire table */
        #example {
            border: 2px solid black; /* Table border */
            border-radius: 8px; /* Rounded corners for the table */
            overflow: hidden; /* To ensure rounded corners show correctly */
            max-width:1200px;
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
        .hidden-option {
            display: none;
        }

    </style>

    

    <div class="container nav-spacer">
        <asp:Label ID="lblHeader" runat="server" CssClass="h2 text-center">Please select a view mode.</asp:Label>
        <small class="d-block  mt-2">
            <asp:Label ID="lblPerm" runat="server"></asp:Label>
        </small>

        <div class="row align-items-center mb-3">
            <div class="row align-items-center mb-3">
                <!-- Dropdowns aligned to the left -->
                <div class="col d-flex">
                    <div class="me-2">
                        <asp:DropDownList ID="ddlViewmode" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlViewmode_SelectedIndexChanged" EnableViewState="true">
                            <asp:ListItem Text="Choose view mode" Value="Default" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="All API" Value="All"></asp:ListItem>
                            <asp:ListItem Text="My API" Value="My"></asp:ListItem>
                            <asp:ListItem Text="Shared With Me API" Value="Shared"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <asp:DropDownList ID="ddlApplications" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlApplications_SelectedIndexChanged" EnableViewState="true">
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- Buttons aligned to the right -->
                <div class="col text-end">
                    <asp:Button ID="CollaborationButton" runat="server" Text="Collaboration" CssClass="btn btn-info ms-2" OnClientClick="showCollaborationModal(); return false;" Visible="false" />
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#createAppModal">
                        Create Application
                    </button>
                </div>
            </div>

            <!-- Collaboration Modal -->
            <div id="collaborationModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="collaborationModalLabel" aria-hidden="true" Visible="false">
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
                                <table id="example1" class="table table-striped" style="width:100%">
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
                                        <asp:Label ID="lblPermission" runat="server" Text='<%# Eval("Permission") %>' EnableViewState="true" />
                                        <asp:DropDownList ID="ddlPermission" runat="server" Visible="false" CssClass="form-select form-select-sm" EnableViewState="true">
                                            <asp:ListItem Value="Write">Write</asp:ListItem>
                                            <asp:ListItem Value="Read">Read</asp:ListItem>
                                            <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnEdit" runat="server" Text="Edit Permission" 
                                            CommandName="Edit" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' 
                                            CssClass="btn btn-primary btn-sm"
                                            UseSubmitBehavior="false" />

                                        <asp:Button ID="btnSave" runat="server" Text="Save" 
                                            CommandName="Save" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' 
                                            Visible="false" 
                                            CssClass="btn btn-success btn-sm"
                                            UseSubmitBehavior="false" />

                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                                            CommandName="Cancel" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' 
                                            Visible="false" 
                                            CssClass="btn btn-secondary btn-sm"
                                            UseSubmitBehavior="false" />

                                        <asp:Button ID="btnRemove" runat="server" Text="Remove Collaborator" 
                                            CommandName="Remove" 
                                            CommandArgument='<%# Eval("CollaboratorName") %>' 
                                            Visible="false" 
                                            CssClass="btn btn-danger btn-sm"
                                            UseSubmitBehavior="false" />
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
                                            <span><%# Eval("user_username") %></span>
                                            <asp:Button ID="AddCollaboratorButton" runat="server" 
                                                Text="Add Collaborator" 
                                                CssClass="btn btn-success btn-sm" 
                                                CommandName="AddCollaborator" 
                                                CommandArgument='<%# Eval("user_id") %>' 
                                                UseSubmitBehavior="false" />
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
                </div>
            </div>
        </div>
        </div>
        <hr />

            
        <div id="appDetailsCard" runat="server" class="card mt-3 shadow d-none">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center app-header" data-bs-toggle="collapse" data-bs-target="#appDetailsContent" aria-expanded="false" aria-controls="appDetailsContent">
                <h5 class="mb-0">Application Details</h5>
                <i class="bi bi-chevron-down"></i>
            </div>


            <div id="appDetailsContent" class="collapse">
                <div id="appDetailsContainer" class="card-body">
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label for="appName" class="form-label"><strong>Name:</strong></label>
                            <asp:TextBox ID="txtAppNameEdit" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label for="appTestingPath" class="form-label"><strong>Testing Path:</strong></label>
                            <asp:TextBox ID="txtAppTestingPathEdit" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label for="appProductionPath" class="form-label"><strong>Production Path:</strong></label>
                            <asp:TextBox ID="txtAppProductionPathEdit" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label for="appLanguage" class="form-label"><strong>Language:</strong></label>
                            <asp:TextBox ID="txtAppLanguageEdit" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col text-end">
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" Visible="false"/>
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" Visible="false" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" Visible="false" />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete Application" CssClass="btn btn-danger" OnClick="btnDelete_Click" Visible="false" OnClientClick="return confirm('Are you sure you want to delete this application and all its APIs?');" />
                        </div>
                    </div>
                </div>
            </div>
        </div>



         <!-- Create Application Modal -->
        <div class="modal fade" id="createAppModal" tabindex="-1" aria-labelledby="createAppModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="createAppModalLabel">Create Application</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <!-- Form to create an application -->
                        <asp:Panel ID="createAppPanel" runat="server">
                            <div class="mb-3">
                                <label for="appName" class="form-label">Application Name</label>
                                <asp:TextBox ID="txtAppName" CssClass="form-control" runat="server" Placeholder="Enter application name"></asp:TextBox>
                            </div>
                            <asp:Label ID="lblMessageApp" runat="server" CssClass="h2 text-center"></asp:Label>
                            <div class="mb-3">
                                <label for="appTestingPath" class="form-label">Testing Path</label>
                                <asp:TextBox ID="txtAppTestingPath" CssClass="form-control" runat="server" Placeholder="Enter testing path"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <label for="appProductionPath" class="form-label">Production Path</label>
                                <asp:TextBox ID="txtAppProductionPath" CssClass="form-control" runat="server" Placeholder="Enter production path"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <label for="appLanguage" class="form-label">Application Language</label>
                                <asp:TextBox ID="txtAppLanguage" CssClass="form-control" runat="server" Placeholder="Enter application language"></asp:TextBox>
                            </div>
                        </asp:Panel>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <asp:Button ID="btnSaveApp" CssClass="btn btn-primary" runat="server" Text="Save Application" OnClick="btnSaveApp_Click" />
                    </div>
                </div>
            </div>
        </div>

         <div id="apiResultsContainer" runat="server" visible="false">
            <asp:Repeater ID="rptResults" runat="server" OnItemCommand="rptResults_ItemCommand2">
                <HeaderTemplate>
                    <table id="example" class="table table-striped" style="width:100%">
                        <thead>
                            <tr>
                                <th>API Name</th>
                                <th>HTTP Method</th>
                                <th>Endpoint</th>
                                <th>Time Added</th>
                                <th>Description</th>
                                <th>API Details</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("API_name") %></td>
                        <td><%# Eval("API_HTTP_method") %></td>
                        <td><%# Eval("API_endpoint") %></td>
                        <td><%# Eval("API_update_date", "{0:dd-MM-yyyy HH:mm:ss}") %></td>
                        <td><%# Eval("API_desc").ToString() %></td>
<td class="center-align">
    <asp:LinkButton CommandName="Redirect" CommandArgument='<%# Eval("API_id") %>' runat="server" CssClass="btn btn-link">View Details</asp:LinkButton>
</td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script src="JS/viewAPI_Page.js"></script>
    <script type="text/javascript">

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

        new DataTable('#example');

        document.addEventListener("DOMContentLoaded", function () {
            const appDetailsContent = document.getElementById("appDetailsContent");
            const appDetailsCard = document.querySelector(".app-header");

            // Restore state from sessionStorage
            const savedState = sessionStorage.getItem("appDetailsExpanded");

            if (savedState === "true") {
                appDetailsContent.classList.add("show"); // Bootstrap class to keep it open
                appDetailsCard.setAttribute("aria-expanded", "true");
            } else if (savedState === "false") {
                appDetailsContent.classList.remove("show"); // Collapse it
                appDetailsCard.setAttribute("aria-expanded", "false");
            }

            // Bootstrap's collapse toggle event listener
            appDetailsContent.addEventListener("shown.bs.collapse", function () {
                sessionStorage.setItem("appDetailsExpanded", "true");
            });

            appDetailsContent.addEventListener("hidden.bs.collapse", function () {
                sessionStorage.setItem("appDetailsExpanded", "false");
            });
        });



        // Event listener to toggle the icon
        document.querySelector('.card-header').addEventListener('click', function () {
            const icon = this.querySelector('i');
            icon.classList.toggle('bi-chevron-down');
            icon.classList.toggle('bi-chevron-up');
        });

        $(document).ready(function () {
            // Initialize the DataTable
            var table = $('#example').DataTable();
            var table = $('#example1').DataTable();

            // Remove 'dt-column-order' class from <th> elements that have 'delete-sort' class on table redraw
            table.on('draw', function () {
                // Select all <th> elements with the class 'delete-sort' and remove the 'dt-column-order' class
                $('#example thead th.delete-sort').removeClass('dt-column-order');
            });
        });

    </script>
</asp:Content>
