<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="CollaboratorProfile.aspx.cs" Inherits="API_DAMs.UI.CollaboratorProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="CSS/ProfilePage.css" rel="stylesheet" />

<div class="container emp-profile">
    <div class="row">
        <div class="col-md-4">

            <div class="profile-img">
                <asp:Literal ID="litImage" runat="server"></asp:Literal>
            </div>

        </div>
        <div class="col-md-6">
            <div class="profile-head">
                <h5>
                    <asp:Literal ID="litUsername" runat="server"></asp:Literal>
                </h5>
                <h6>
                    Tagline here
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

    </div>
    <div class="row">
        <div class="col-md-4">
            <div class="profile-work">
                <p>Your Colaborator</p>
                <a href="">Website Link</a><br/>
                <a href="">Bootsnipp Profile</a><br/>
                <a href="">Bootply Profile</a>
                <a href="">Web Designer</a><br/>
                <a href="">Web Developer</a><br/>
                <a href="">WordPress</a><br/>
                <a href="">WooCommerce</a><br/>
                <a href="">PHP, .Net</a><br/>
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

        });
    </script>
</asp:Content>
