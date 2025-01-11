<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="viewAPI.aspx.cs" Inherits="API_DAMs.UI.viewAPI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.js"></script>
    <script src="https://cdn.datatables.net/2.1.8/js/dataTables.bootstrap5.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet">


    <link href="CSS/view.css" rel="stylesheet" />
    
    <div class="container nav-spacer">
        <asp:Label ID="lblHeader" runat="server" CssClass="h2 text-center">Choose the application</asp:Label>

        <div class="row align-items-center mb-3">
            <!-- Dropdown aligned to the left -->
            <div class="col-auto">
                <asp:DropDownList ID="ddlApplications" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlApplications_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <!-- Button aligned to the right -->
            <div class="col text-end">
                <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#createAppModal">
                    Create Application
                </button>
            </div>
        </div>

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
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click" />
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" Visible="false" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <hr />

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
            <asp:Repeater ID="rptResults" runat="server" OnItemCommand="rptResults_ItemCommand">
                <HeaderTemplate>
                    <table id="example" class="table table-striped" style="width:100%">
                        <thead>
                            <tr>
                                <th class="delete-sort">Select</th>
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
                        <td><input type="checkbox" name="selectItem" value="<%# Eval("API_id") %>" class="selectCheckbox" /></td>
                        <td><%# Eval("API_name") %></td>
                        <td><%# Eval("API_HTTP_method") %></td>
                        <td><%# Eval("API_desc").ToString() %></td>
                        <td><%# Eval("API_endpoint") %></td>
                        <td><%# Eval("code_uploadDate", "{0:dd-MM-yyyy HH:mm:ss}") %></td>
                        <td>
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

            // Remove 'dt-column-order' class from <th> elements that have 'delete-sort' class on table redraw
            table.on('draw', function () {
                // Select all <th> elements with the class 'delete-sort' and remove the 'dt-column-order' class
                $('#example thead th.delete-sort').removeClass('dt-column-order');
            });
        });

        function toggleAllCheckboxes(source) {
            // Get all checkboxes with class 'selectCheckbox'
            var checkboxes = document.querySelectorAll('.selectCheckbox');
            checkboxes.forEach(function(checkbox) {
                checkbox.checked = source.checked;
            });
        }
    </script>
</asp:Content>
