<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="ViewDetailPage.aspx.cs" Inherits="API_DAMs.UI.ViewDetailPage" Async="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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
                        <div id="collaboratorContainer">
                            <asp:Repeater ID="CollaboratorRepeater" runat="server">
                                <HeaderTemplate>
                                    <div class="row mb-2">
                                        <div class="col-6 font-weight-bold">Name</div>
                                        <div class="col-6 font-weight-bold">Permission</div>
                                    </div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="row mb-2">
                                        <div class="col-6"><%# Container.ItemIndex + 1 %>. <%# Eval("SharedName") %></div>
                                        <div class="col-6">
                                            <select class="form-control" data-collab-id="<%# Eval("collab_id") %>">
                                                <option value="Read" <%# Eval("collab_permission").ToString() == "Read" ? "selected" : "" %>>Read</option>
                                                <option value="Edit" <%# Eval("collab_permission").ToString() == "Edit" ? "selected" : "" %>>Edit</option>
                                                <option value="Delete" <%# Eval("collab_permission").ToString() == "Delete" ? "selected" : "" %>>Delete</option>
                                            </select>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Add New Collaborator Button -->
                        <button class="btn btn-primary mt-3" onclick="showAddCollaboratorPopup(); return false;">+ Add New</button>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="SaveCollaborationButton" runat="server" Text="Save" CssClass="btn btn-success" OnClick="SaveCollaborationButton_Click" />
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Add New Collaborator Popup -->
        <div id="addCollaboratorPopup" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="addCollaboratorPopupLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header d-flex justify-content-between align-items-center">
                        <h5 class="modal-title" id="addCollaboratorPopupLabel">Add a New Collaborator</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label for="searchName">Search Name</label>
                            <input type="text" class="form-control" id="searchName" placeholder="Enter name" onkeyup="searchFriends()">
                        </div>
                        <div id="searchResults">
                            <!-- Search results will appear here -->
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="addSelectedCollaborators();">Add</button>
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
    function showCollaborationModal() {
        $('#collaborationModal').modal('show');
    }

    function showAddCollaboratorPopup() {
        $('#addCollaboratorPopup').modal('show');
    }

    function addCollaborator() {
        const name = document.getElementById('searchName').value;
        if (name.trim() !== '') {
            // Add new collaborator row to the main list (example hard-coded implementation)
            const collaboratorContainer = document.getElementById('collaboratorContainer');
            const newRow = document.createElement('div');
            newRow.classList.add('row', 'mb-2');
            newRow.innerHTML = `
                <div class="col-6">${name}</div>
                <div class="col-6">
                    <select class="form-control">
                        <option value="Read">Read</option>
                        <option value="Edit">Edit</option>
                        <option value="Delete">Delete</option>
                    </select>
                </div>
            `;
            collaboratorContainer.appendChild(newRow);

            // Close the Add Collaborator popup
            $('#addCollaboratorPopup').modal('hide');
        } else {
            alert('Please enter a valid name.');
        }
    }

    // Global variable to track selected collaborators
    let selectedCollaborators = [];

    function searchFriends() {
        const searchTerm = document.getElementById('searchName').value.trim();
        const searchResultsContainer = document.getElementById('searchResults');

        if (searchTerm === '') {
            searchResultsContainer.innerHTML = '';
            return;
        }

        $.ajax({
            url: "https://localhost:44340/UI/Collaborators.aspx/SearchFriends", // Use ResolveUrl for ASP.NET
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            searchTerm: searchTerm
        }),
        dataType: 'json',
        success: function (response) {
            console.log('Full response:', response);
            // ... rest of the success handling
        },
        error: function (xhr, status, error) {
            console.error('Search failed - Full Error:', xhr);
            console.error('Status:', status);
            console.error('Error:', error);

            if (xhr.responseText) {
                console.error('Response Text:', xhr.responseText);
                searchResultsContainer.innerHTML = `<p class="text-danger mt-2">Error: ${xhr.responseText}</p>`;
            } else {
                searchResultsContainer.innerHTML = '<p class="text-danger mt-2">Unexpected error occurred</p>';
            }
        }
    });
    }

    function updateSelectedCollaborators(checkbox, friend) {
        if (checkbox.checked) {
            // Add to selected collaborators
            selectedCollaborators.push({
                userId: friend.UserId,
                username: friend.Username,
                email: friend.Email
            });
        } else {
            // Remove from selected collaborators
            selectedCollaborators = selectedCollaborators.filter(
                collab => collab.userId !== friend.UserId
            );
        }
    }

    function addSelectedCollaborators() {
        if (selectedCollaborators.length === 0) {
            alert('Please select at least one collaborator');
            return;
        }

        // AJAX call to add selected collaborators
        $.ajax({
            url: 'YourAddCollaboratorsPage.aspx/AddCollaborators', // Replace with your actual server-side method
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                collaborators: selectedCollaborators
            }),
            dataType: 'json',
            success: function (response) {
                if (response.d) {
                    // Successfully added collaborators
                    alert('Collaborators added successfully');

                    // Close the modal
                    $('#addCollaboratorPopup').modal('hide');

                    // Optionally refresh the collaborators list
                    loadCollaborators();

                    // Reset selected collaborators
                    selectedCollaborators = [];
                } else {
                    alert('Failed to add collaborators');
                }
            },
            error: function (xhr, status, error) {
                console.error('Add collaborators failed:', error);
                alert('An error occurred while adding collaborators');
            }
        });
    }

    // Optional: Clear search and selections when modal opens
    $('#addCollaboratorPopup').on('show.bs.modal', function () {
        document.getElementById('searchName').value = '';
        document.getElementById('searchResults').innerHTML = '';
        selectedCollaborators = [];
    });
</script>



</asp:Content>
