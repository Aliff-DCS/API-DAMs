<%@ Page Title="" Language="C#" MasterPageFile="~/UI/HeaderHome.Master" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="API_DAMs.UI.HomePage" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
<style>
.carousel-item {
    height: 250px; /* Reduced height */
}

.carousel-control-prev-icon,
.carousel-control-next-icon {
    background-color: rgba(0, 0, 0, 0.3);
    border-radius: 50%;
    padding: 1rem;
}

code {
    background-color: #f8f9fa;
    padding: 0.5rem 1rem;
    border-radius: 4px;
    font-size: 0.875rem;
}

.card {
    margin: 1rem 0;
}

/* Adjust indicator position for smaller carousel */
.carousel-indicators {
    bottom: 0;
}
</style>

    <div class="container-fluid" style="z-index: 100;" >
    <div class="row">
        <div class="col-md-8 mx-auto">
            <!-- Recent APIs Carousel -->
            <div class="card shadow-sm">
                <div class="card-header bg-light">
                    <h5 class="card-title mb-0">Recent APIs</h5>
                </div>
                <div class="card-body p-0">
                    <asp:Label ID="lblNoApis" runat="server" CssClass="text-center w-100 d-block" Visible="false">
                        There's no API created yet.
                    </asp:Label>
                    <div id="recentApisCarousel" class="carousel slide" data-bs-ride="carousel">
                        <!-- Carousel Indicators -->
                        <div class="carousel-indicators">
                            <asp:Repeater ID="rpCarouselIndicators" runat="server">
                                <ItemTemplate>
                                    <button type="button" data-bs-target="#recentApisCarousel" 
                                            data-bs-slide-to="<%# Container.ItemIndex %>"
                                            class="<%# Container.ItemIndex == 0 ? "active" : "" %>"
                                            aria-current="<%# Container.ItemIndex == 0 ? "true" : "false" %>"
                                            aria-label="Slide <%# Container.ItemIndex + 1 %>">
                                    </button>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Carousel Items -->
                        <div class="carousel-inner">
                            <asp:Repeater ID="rpRecentApis" runat="server">
                                <ItemTemplate>
                                    <div class="carousel-item <%# Container.ItemIndex == 0 ? "active" : "" %>">
                                        <div class="d-flex align-items-center justify-content-center bg-light p-4">
                                            <div class="text-center">
                                                <h3 class="mb-3"><%# Eval("ApiName") %></h3>
                                                <p class="mb-2"><%# Eval("Description") %></p>
                                                <code class="d-block mb-2"><%# Eval("Endpoint") %></code>
                                                <small class="text-muted d-block mb-3">
                                                    Last updated: <%# ((DateTime?)Eval("UpdateDate"))?.ToString("MMM dd, yyyy") %>
                                                </small>
                                                <a href='<%# $"ViewDetailPage.aspx?apiId={Eval("ApiId")}" %>' 
                                                    class="btn btn-primary btn-sm">
                                                    <i class="fas fa-edit me-1"></i> Edit
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Navigation Controls -->
                        <button class="carousel-control-prev" type="button" 
                                data-bs-target="#recentApisCarousel" data-bs-slide="prev">
                            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                            <span class="visually-hidden">Previous</span>
                        </button>
                        <button class="carousel-control-next" type="button" 
                                data-bs-target="#recentApisCarousel" data-bs-slide="next">
                            <span class="carousel-control-next-icon" aria-hidden="true"></span>
                            <span class="visually-hidden">Next</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>


    <div class="container mt-4 d-flex justify-content-center">
        <!-- Card -->
        <div class="card" style="background-color: #00c6ff; border: 2px solid black; border-radius: 8px; width: 100%; max-width: 1300px;">
            <!-- Card Header -->
            <div class="card-header text-white bg-black">
                <h4 class="mb-0">Dashboard Summary</h4>
                <p class="mb-0">Overview of your APIs, applications, and collaborator</p>
            </div>


            <!-- Card Body -->
            <div class="card-body">
                <!-- Metrics Cards Row -->
                <div class="row mb-4">
                    <div class="col-md-3">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">Total APIs</h5>
                                <h2 class="card-text">
                                    <asp:Label ID="lblTotalAPIs" runat="server" CssClass="h2"></asp:Label>
                                </h2>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">Shared APIs</h5>
                                <h2 class="card-text">
                                    <asp:Label ID="lblSharedAPIs" runat="server" CssClass="h2"></asp:Label>
                                </h2>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">Total Friends</h5>
                                <h2 class="card-text">
                                    <asp:Label ID="lblTotalFriends" runat="server" CssClass="h2"></asp:Label>
                                </h2>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">Active Applications</h5>
                                <h2 class="card-text">
                                    <asp:Label ID="lblActiveApps" runat="server" CssClass="h2"></asp:Label>
                                </h2>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- New Row for Friends -->
                <div class="row">
                    <div class="col-md-9">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title">Summary Chart</h5>

                                <!-- Google Chart will be rendered in this div -->
                                <div id="piechart" style="width: 900px; height: 500px;"></div>

                                <!-- Hidden field to store the chart data from the backend -->
                                <input type="hidden" id="summaryChartData" runat="server" clientidmode="Static" />
                            </div>
                        </div>
                    </div>

                    <!-- Friends List -->
                    <div class="col-md-3">
                        <div class="card">
                            <div class="card-body" style="height: 300px; overflow-y: auto;">
                                <h5 class="card-title">Friends List</h5>
                                <!-- Global message -->
                                <asp:Label ID="lblMessage" runat="server" CssClass="h2"></asp:Label>
                                <asp:Repeater ID="rpFriendsList" runat="server">
                                    <ItemTemplate>
                                        <div class="friend-item">
                                            <h6><%# Eval("FriendName") %></h6>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
<script type="text/javascript">
    // Put this code at the top of your script section
    google.charts.load('current', { 'packages': ['corechart'] });
    google.charts.setOnLoadCallback(drawChart);

    // Add a console log to verify the library is loaded
    console.log('Google Charts loading...');
    function drawChart() {
        var chartDataElement = document.getElementById('summaryChartData');
        if (!chartDataElement) {
            console.error('Chart data element not found!');
            return;
        }

        var chartData = chartDataElement.value;
        if (!chartData) {
            console.error('No chart data available!');
            return;
        }

        try {
            // Parse the JSON data from the hidden field
            chartData = JSON.parse(chartData);

            // Prepare the data for Google Chart
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'App Name');
            data.addColumn('number', 'API Count');

            // Add the rows to the data table
            for (var i = 0; i < chartData.labels.length; i++) {
                data.addRow([chartData.labels[i], chartData.data[i]]);
            }

            var options = {
                title: 'API Count per Application',
                slices: {
                    0: { offset: 0.1 },
                    1: { offset: 0.1 }
                }
            };

            // Create and draw the chart
            var chart = new google.visualization.PieChart(document.getElementById('piechart'));
            chart.draw(data, options);
        } catch (error) {
            console.error('Error drawing chart:', error);
        }
    }
</script>

</asp:Content>
