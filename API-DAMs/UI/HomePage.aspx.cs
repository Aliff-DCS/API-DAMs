using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace API_DAMs.UI
{
    public partial class HomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {   
                LoadDashboardData();
                LoadRecentApis();
                LoadApiSummaryChart();
            }
        }
        protected void LoadApiSummaryChart()
        {
            string connString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string query = @"
                SELECT 
                    a.app_name,
                    COUNT(m.API_id) AS API_Count
                FROM 
                    application a
                LEFT JOIN 
                    api_methods m ON a.app_id = m.app_id
                WHERE 
                    a.user_id = @currentUserId
                GROUP BY 
                    a.app_name";

            List<ApiSummaryData> apiSummary = new List<ApiSummaryData>();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUserId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        apiSummary.Add(new ApiSummaryData
                        {
                            AppName = reader["app_name"].ToString(),
                            ApiCount = Convert.ToInt32(reader["API_Count"])
                        });
                    }
                }
            }

            // Bind data to the frontend
            BindChart(apiSummary);
        }

        private void BindChart(List<ApiSummaryData> apiSummary)
        {
            var appNames = apiSummary.Select(a => a.AppName).ToArray();
            var apiCounts = apiSummary.Select(a => a.ApiCount).ToArray();
            var chartData = new
            {
                labels = appNames,
                data = apiCounts
            };
            string jsonChartData = JsonConvert.SerializeObject(chartData);
            // Use this line instead
            summaryChartData.Value = jsonChartData;
        }

        public class ApiSummaryData
        {
            public string AppName { get; set; }
            public int ApiCount { get; set; }
        }

        private void LoadRecentApis()
        {
            string connString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // Query to get recent APIs
            string query = @"SELECT TOP 4 
                m.API_id,
                m.API_name, 
                m.API_desc, 
                m.API_endpoint, 
                h.code_uploadDate
            FROM api_methods AS m
            LEFT JOIN api_header AS h ON m.code_id = h.code_id
            WHERE h.user_id = @CurrentUserId
            ORDER BY h.code_uploadDate DESC, m.API_id DESC;";

            List<RecentApi> recentApis = new List<RecentApi>();

            // Get current user ID from session
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CurrentUserId", currentUserId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        recentApis.Add(new RecentApi
                        {
                            ApiId = reader["API_id"].ToString(),
                            ApiName = reader["API_name"].ToString(),
                            Description = reader["API_desc"]?.ToString(),
                            Endpoint = reader["API_endpoint"].ToString(),
                            UpdateDate = reader["code_uploadDate"] as DateTime?
                        });
                    }
                }
            }

            // If there are no APIs, show the "No API" message
            if (recentApis.Count == 0)
            {
                lblNoApis.Visible = true;
            }
            else
            {
                lblNoApis.Visible = false;
            }

            rpRecentApis.DataSource = recentApis;
            rpRecentApis.DataBind();
            rpCarouselIndicators.DataSource = recentApis;
            rpCarouselIndicators.DataBind();
        }

        // Model class for recent API
        public class RecentApi
        {
            public string ApiId { get; set; }
            public string ApiName { get; set; }
            public string Description { get; set; }
            public string Endpoint { get; set; }
            public DateTime? UpdateDate { get; set; }
        }



        private void LoadDashboardData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int currentUserId = Convert.ToInt32(Session["UserID"]);
                    // Total APIs
                    using (SqlCommand cmd = new SqlCommand(@"
                            SELECT COUNT(DISTINCT am.API_id)
                            FROM api_methods am
                            LEFT JOIN api_header ah ON am.code_id = ah.code_id
                            LEFT JOIN file_details fd ON am.file_id = fd.file_id
                            WHERE ah.user_id = @userId
                              AND am.app_id IS NOT NULL;", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", Session["UserID"]);
                        lblTotalAPIs.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Shared APIs
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(APP_id) FROM collaborator WHERE shared_id = @sharedId", conn))
                    {
                        cmd.Parameters.AddWithValue("@sharedId", currentUserId);
                        var result = cmd.ExecuteScalar();
                        lblSharedAPIs.Text = result != DBNull.Value ? result.ToString() : "0";
                    }


                    // Total Friends
                    using (SqlCommand cmd = new SqlCommand(@"
                            SELECT COUNT(*) 
                            FROM friends 
                            WHERE (initiator_id = @userId OR receiver_id = @userId) 
                            AND friend_status = 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        var result = cmd.ExecuteScalar();
                        lblTotalFriends.Text = result != DBNull.Value ? result.ToString() : "0";
                    }

                    // Active Applications
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM application WHERE user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        var result = cmd.ExecuteScalar();
                        lblActiveApps.Text = cmd.ExecuteScalar().ToString();
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
        SELECT 
            u.user_username AS FriendName, 
            CASE 
                WHEN f.initiator_id = @currentUserId THEN f.receiver_id
                WHEN f.receiver_id = @currentUserId THEN f.initiator_id
            END AS FriendId
        FROM 
            friends f
        JOIN 
            users u ON 
                (u.user_id = f.initiator_id OR u.user_id = f.receiver_id)
        WHERE 
            (f.initiator_id = @currentUserId OR f.receiver_id = @currentUserId)
            AND f.friend_status = 1
            AND u.user_id != @currentUserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Check if the data table contains records
                        if (dt.Rows.Count > 0)
                        {
                            rpFriendsList.DataSource = dt;
                            rpFriendsList.DataBind();
                        }
                        else
                        {
                            // No friends found, handle this case (optional)
                            lblMessage.Text = "No friends found.";
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Log the error and show user-friendly message
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Error loading dashboard data. Please try again later.');", true);
                    // You might want to log the actual error somewhere
                    System.Diagnostics.Debug.WriteLine($"Error in LoadDashboardData: {ex.Message}");
                }
            }
        }
    }
}