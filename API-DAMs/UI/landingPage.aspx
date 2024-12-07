<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Header.Master" AutoEventWireup="true" CodeBehind="landingPage.aspx.cs" Inherits="API_DAMs.UI.landingPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <section class="background">
        <%--<img src="../images/API_background.jpg" class="img-fluid"/>--%>
        <div class="container">
          <div class="image-section">
            <img src="../images/api_logo.png" alt="API Documentation Image" width="50" height="50" />
          </div>
          <div class="text-section">
            <h2>What is API Documentation & Management System?</h2>
            <p>
              This system is a platform designed to streamline the creation, organization, maintenance, and interaction with Application Programming Interfaces (APIs) as their number continues to grow.
            </p>
            <p>
              This system provides tools to keep APIs organized, ensuring easy access, consistency, and proper management throughout their lifecycle.
            </p>
          </div>
        </div>
    </section>

    <section class="features-section ">
        <div class="text-section">
            <h2  Style="color:black;"> This system offers these features:</h2>
        </div>
        <div class="features-container">
        <div class="feature-item">
            <img src="../images/documentation.png" alt="Documentation" class="img-fluid"/>
            <h3>Documentation</h3>
            <p>Add a little bit of body text<br />Add a little bit of body text<br />Add a little bit of body text</p>
        </div>
        <div class="feature-item">
            <img src="../images/collab.png" alt="Collaboration" class="img-fluid"/>
            <h3>Collaboration</h3>
            <p>Add a little bit of body text<br />Add a little bit of body text<br />Add a little bit of body text</p>
        </div>
        <div class="feature-item">
            <img src="../images/personalized.png" alt="Personalised" class="img-fluid"/>
            <h3>Personalised</h3>
            <p>Add a little bit of body text<br />Add a little bit of body text<br />Add a little bit of body text</p>
        </div>
        <div class="feature-item">
            <img src="../images/searching.png" alt="Searching" class="img-fluid"/>
            <h3>Searching</h3>
            <p>Add a little bit of body text<br />Add a little bit of body text<br />Add a little bit of body text</p>
        </div>
        </div>
    </section>
        
</asp:Content>
