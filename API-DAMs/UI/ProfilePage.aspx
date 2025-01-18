<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="ProfilePage.aspx.cs" Inherits="API_DAMs.UI.ProfilePage1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="CSS/ProfilePage.css" rel="stylesheet" />

    <div class="container emp-profile">
        <div class="row">
            <div class="col-md-4">

                <div class="profile-img">
                    <asp:Literal ID="litImage" runat="server"></asp:Literal>

                    <div class="file btn btn-lg btn-primary">
                        Change Photo
                        <asp:FileUpload ID="fileUpload" runat="server" CssClass="file-input" OnChange="previewImage();" />
                    </div>

                    <div id="imagePreview" style="display:none;">
                        <span style="font-weight: bold; display: block; margin-bottom: 10px;">PREVIEW</span>
                        <img id="previewImg" src="#" alt="Selected Image" style="max-width: 200px; margin-top: 10px;" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                    </div>
                    <asp:Label ID="lblError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

                </div>


                <script type="text/javascript">
                    function previewImage() {
                        var fileInput = document.getElementById('<%= fileUpload.ClientID %>');
                        var previewImg = document.getElementById('previewImg');
                        var imagePreview = document.getElementById('imagePreview');

                        var file = fileInput.files[0];
                        if (file) {
                            var reader = new FileReader();
                            reader.onload = function (e) {
                                previewImg.src = e.target.result;
                                imagePreview.style.display = 'block';  // Show submit/cancel buttons
                            };
                            reader.readAsDataURL(file);
                        }
                    }
                </script>



            </div>
            <div class="col-md-6">
                <div class="profile-head">
                    <h5>
                        <asp:Literal ID="litUsername" runat="server"></asp:Literal>
                    </h5>
                    <h6>
                        <asp:Literal ID="litTagline" runat="server"></asp:Literal>
                    </h6>
                    <p class="proile-rating">Joined since : <span><asp:Literal ID="litJD" runat="server"></asp:Literal></span></p>
                    <ul class="nav nav-tabs" id="myTab" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="home-tab" data-toggle="tab" role="tab" href="#home" aria-controls="home" aria-selected="true">About</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="profile-tab" data-toggle="tab" role="tab" href="#profile" aria-controls="profile" aria-selected="false">Graph</a>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnEditProfile" runat="server" Text="Edit Profile" CssClass="profile-edit-btn" OnClick="btnEditProfile_Click" />
            </div>

        </div>
        <div class="row">
            <div class="col-md-4">
                <!-- View Mode -->
                <div class="profile-work" id="StaticBio" runat="server" visible="true">
                    <p>User Bio</p>
                    <textarea id="txtUserDesc" runat="server" class="form-control" rows="8" disabled="disabled"></textarea>
                </div>

                <!-- Edit Mode -->
                <div class="profile-work" id="EditableBio" runat="server" visible="false">
                    <p>Edit Bio</p>
                    <textarea id="txtEditUserDesc" runat="server" class="form-control" rows="8"></textarea>
                </div>
            </div>
            <div class="col-md-8">
                <div class="tab-content profile-tab" id="myTabContent">
                    <div class="tab-pane fade show active" id="home" role="tabpanel" aria-labelledby="home-tab">
                            <asp:Panel ID="pnlViewMode" runat="server">
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>User Id</label>
                                    </div>
                                    <div class="col-md-6">
                                        <p><asp:Literal ID="litID" runat="server"></asp:Literal></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Name</label>
                                    </div>
                                    <div class="col-md-6">
                                        <p><asp:Literal ID="litName" runat="server"></asp:Literal></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Email</label>
                                    </div>
                                    <div class="col-md-6">
                                        <p><asp:Literal ID="litEmail" runat="server"></asp:Literal></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Phone</label>
                                    </div>
                                    <div class="col-md-6">
                                        <p><asp:Literal ID="litPhone" runat="server"></asp:Literal></p>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>User Visibility</label>
                                    </div>
                                    <div class="col-md-6">
                                        <p><asp:Literal ID="LitVisibility" runat="server"></asp:Literal></p>
                                    </div>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlEditMode" runat="server" Visible="false">
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>User Id</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtID" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Name</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Email</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Phone</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>Tagline</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtTagline" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <label>User Visibility</label>
                                    </div>
                                    <div class="col-md-6">
                                        <asp:RadioButtonList ID="rblVisibility" runat="server" CssClass="form-check">
                                            <asp:ListItem Text="Public" Value="Public"></asp:ListItem>
                                            <asp:ListItem Text="Private" Value="Private"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <asp:Button ID="btnSubmitEdit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmitEdit_Click" />
                                        <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancelEdit_Click" />
                                    </div>
                                </div>
                            </asp:Panel>

                        </div>
                    <div class="tab-pane fade" id="profile" role="tabpanel" aria-labelledby="profile-tab">
                        <div class="timeline-content">
                            <h5>Graph</h5>
                            <p>Your graph content goes here.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div> 

        <script type="text/javascript">
            // Wait for the document to be fully loaded
            $(document).ready(function () {
                // Tab switching functionality
                $('#myTab .nav-link').click(function (e) {
                    e.preventDefault(); // Prevent default link behavior

                    // Remove active class from all tabs and tab panes
                    $('#myTab .nav-link').removeClass('active');
                    $('.tab-pane').removeClass('show active');

                    // Add active class to clicked tab
                    $(this).addClass('active');

                    // Get the target tab pane ID
                    var targetPane = $(this).attr('href');

                    // Show the selected tab pane
                    $(targetPane).addClass('show active');
                });

                // Image preview function (existing functionality)
                window.previewImage = function () {
                    var fileInput = document.getElementById('<%= fileUpload.ClientID %>');
                    var previewImg = document.getElementById('previewImg');
                    var imagePreview = document.getElementById('imagePreview');

                    var file = fileInput.files[0];
                    if (file) {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            previewImg.src = e.target.result;
                            imagePreview.style.display = 'block';  // Show submit/cancel buttons
                        };
                        reader.readAsDataURL(file);
                    }
                };
            });
        </script>
</asp:Content>
