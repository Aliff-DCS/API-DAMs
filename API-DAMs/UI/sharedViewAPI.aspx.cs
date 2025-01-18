using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class sharedViewAPI : System.Web.UI.Page
    {
        public class ApplicationDetails
        {
            public string Name { get; set; }
            public string TestingPath { get; set; }
            public string ProductionPath { get; set; }
            public string Language { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    LoadApplications();
                    apiResultsContainer.Visible = false; // Ensure the Repeater container is hidden initially
                }
                else
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                }
            }

        }

        private void LoadApplications()
        {
            // Get the current user ID from session or authentication context
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query to join tables and filter based on shared_id, excluding NULL app_id values
                string query = @"
                SELECT DISTINCT
                    a.app_id,
                    a.app_name
                FROM 
                    collaborator c
                LEFT JOIN 
                    api_methods am ON c.API_id = am.API_id
                LEFT JOIN 
                    application a ON am.app_id = a.app_id
                WHERE 
                    c.shared_id = @UserId
                    AND a.app_id IS NOT NULL"; // Ensure that app_id is not NULL

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameter to avoid SQL injection
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Bind the results to the dropdown list
                    ddlApplications.DataSource = reader;
                    ddlApplications.DataTextField = "app_name";
                    ddlApplications.DataValueField = "app_id";
                    ddlApplications.DataBind();
                }
            }

            // Insert the default item at the top of the dropdown
            ddlApplications.Items.Insert(0, new ListItem("-- Select Application --", "0"));

            // Insert the "Default" item at index 1
            ddlApplications.Items.Insert(1, new ListItem("Default", "1"));
        }


        private ApplicationDetails LoadApplicationDetails(int appId)
        {
            // Get the current logged-in user's shared_id
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            ApplicationDetails details = null;
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string query = @"
        SELECT 
            ISNULL(a.app_name, 'default') AS app_name,
            ISNULL(a.app_testing_path, 'N/A') AS app_testing_path,
            ISNULL(a.app_production_path, 'N/A') AS app_production_path,
            ISNULL(a.app_language, 'N/A') AS app_language
        FROM 
            collaborator c
        LEFT JOIN 
            api_methods am ON c.API_id = am.API_id
        LEFT JOIN 
            application a ON am.app_id = a.app_id
        WHERE 
            c.shared_id = @SharedId AND am.app_id = @AppId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@SharedId", currentUserId);
                    cmd.Parameters.AddWithValue("@AppId", appId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Map the data to the ApplicationDetails object
                            details = new ApplicationDetails
                            {
                                Name = reader["app_name"].ToString(),
                                TestingPath = reader["app_testing_path"].ToString(),
                                ProductionPath = reader["app_production_path"].ToString(),
                                Language = reader["app_language"].ToString()
                            };
                        }
                    }
                }
            }

            return details;
        }

        // Dropdown selection changed event
        protected void ddlApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApplications.SelectedValue != null && ddlApplications.SelectedValue != "0")
            {
                // Check if "Default" is selected
                if (ddlApplications.SelectedValue == "1")
                {
                    // Handle behavior for Default selection
                    lblHeader.Text = "Viewing Default APIs";

                    // Hide the details card for default selection
                    appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";

                    // Clear any previous data in the repeater and hide API results container
                    rptResults.DataSource = null;
                    rptResults.DataBind();
                    apiResultsContainer.Visible = true;

                    // Load APIs by Default (for current user)
                    LoadAPIsByDefault();
                }
                else
                {
                    int selectedAppId = int.Parse(ddlApplications.SelectedValue);

                    // Load Application details for selected AppId
                    ApplicationDetails appDetails = LoadApplicationDetails(selectedAppId);
                    lblHeader.Text = "Viewing Application: " + ddlApplications.SelectedItem?.Text ?? "Unknown Application";

                    if (appDetails != null)
                    {
                        // Show the details card if not "Default"
                        appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"].Replace("d-none", "").Trim();

                        // Populate the textboxes with the application details
                        txtAppNameEdit.Text = appDetails.Name;
                        txtAppTestingPathEdit.Text = appDetails.TestingPath;
                        txtAppProductionPathEdit.Text = appDetails.ProductionPath;
                        txtAppLanguageEdit.Text = appDetails.Language;

                    }

                    // Load APIs by AppId for the selected application
                    LoadAPIsByAppId(selectedAppId);
                    apiResultsContainer.Visible = true;
                }
            }
            else
            {
                // Hide the details card and APIs if no valid selection
                appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";
                rptResults.DataSource = null;
                rptResults.DataBind();
                apiResultsContainer.Visible = false;
            }
        }

        //// Edit button click
        //protected void btnEdit_Click(object sender, EventArgs e)
        //{
        //    SetEditMode();
        //}

        //// Save button click
        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    if (ddlApplications.SelectedValue != null)
        //    {
        //        // Store the current selected ID
        //        string currentAppId = ddlApplications.SelectedValue;

        //        string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            string query = @"UPDATE application 
        //                   SET app_name = @AppName,
        //                       app_testing_path = @TestingPath,
        //                       app_production_path = @ProductionPath,
        //                       app_language = @Language
        //                   WHERE app_id = @AppId";

        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@AppId", int.Parse(currentAppId));
        //                cmd.Parameters.AddWithValue("@AppName",
        //                    string.IsNullOrEmpty(txtAppNameEdit.Text) ? (object)DBNull.Value : txtAppNameEdit.Text);
        //                cmd.Parameters.AddWithValue("@TestingPath",
        //                    string.IsNullOrEmpty(txtAppTestingPathEdit.Text) || txtAppTestingPathEdit.Text == "N/A"
        //                        ? (object)DBNull.Value
        //                        : txtAppTestingPathEdit.Text);
        //                cmd.Parameters.AddWithValue("@ProductionPath",
        //                    string.IsNullOrEmpty(txtAppProductionPathEdit.Text) || txtAppProductionPathEdit.Text == "N/A"
        //                        ? (object)DBNull.Value
        //                        : txtAppProductionPathEdit.Text);
        //                cmd.Parameters.AddWithValue("@Language",
        //                    string.IsNullOrEmpty(txtAppLanguageEdit.Text) || txtAppLanguageEdit.Text == "N/A"
        //                        ? (object)DBNull.Value
        //                        : txtAppLanguageEdit.Text);

        //                conn.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //        // Reset to view mode and refresh data
        //        LoadApplications();
        //        // Re-select the saved app ID
        //        ddlApplications.SelectedValue = currentAppId;
        //        lblHeader.Text = "Viewing Application: " + ddlApplications.SelectedItem?.Text ?? "Unknown Application";

        //        // Reset to view mode
        //        txtAppNameEdit.ReadOnly = true;
        //        txtAppTestingPathEdit.ReadOnly = true;
        //        txtAppProductionPathEdit.ReadOnly = true;
        //        txtAppLanguageEdit.ReadOnly = true;

        //        btnEdit.Visible = true;
        //        btnSave.Visible = false;
        //        btnCancel.Visible = false;
        //    }
        //}

        //// Cancel button click
        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    SetViewMode();
        //    ddlApplications_SelectedIndexChanged(null, null);
        //}

        //// Helper method to set view mode
        //private void SetViewMode()
        //{
        //    txtAppNameEdit.ReadOnly = true;
        //    txtAppTestingPathEdit.ReadOnly = true;
        //    txtAppProductionPathEdit.ReadOnly = true;
        //    txtAppLanguageEdit.ReadOnly = true;

        //    btnEdit.Visible = true;
        //    btnSave.Visible = false;
        //    btnCancel.Visible = false;
        //}

        //// Helper method to set edit mode
        //private void SetEditMode()
        //{
        //    txtAppNameEdit.ReadOnly = false; // Keep name readonly
        //    txtAppTestingPathEdit.ReadOnly = false;
        //    txtAppProductionPathEdit.ReadOnly = false;
        //    txtAppLanguageEdit.ReadOnly = false;

        //    btnEdit.Visible = false;
        //    btnSave.Visible = true;
        //    btnCancel.Visible = true;
        //}

        private void LoadAPIsByAppId(int appId)
        {
            // Get the current logged-in user's shared_id
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string query = @"
        SELECT 
            am.API_id, 
            am.API_name, 
            am.API_desc, 
            ah.code_uploadDate, 
            am.API_endpoint, 
            am.API_HTTP_method
        FROM 
            collaborator c
        JOIN 
            api_methods am ON c.API_id = am.API_id
        JOIN 
            api_header ah ON am.code_id = ah.code_id
        WHERE 
            c.shared_id = @SharedId AND am.app_id = @AppId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@SharedId", currentUserId);
                    cmd.Parameters.AddWithValue("@AppId", appId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the repeater
                    rptResults.DataSource = dt;
                    rptResults.DataBind();
                }
            }
        }

        private void LoadAPIsByDefault()
        {
            // Get the current logged-in user's ID from session or authentication context
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // SQL query to select APIs where app_id is NULL and the current user is the shared_id
            string query = @"
        SELECT 
            am.API_id, 
            am.API_name, 
            am.API_desc, 
            ah.code_uploadDate, 
            am.API_endpoint, 
            am.API_HTTP_method
        FROM 
            collaborator c
        JOIN 
            api_methods am ON c.API_id = am.API_id
        JOIN 
            api_header ah ON am.code_id = ah.code_id
        WHERE 
            c.shared_id = @SharedId AND am.app_id IS NULL";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add the current user's shared_id as a parameter to the query
                    cmd.Parameters.AddWithValue("@SharedId", currentUserId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the result set to the repeater control
                    rptResults.DataSource = dt;
                    rptResults.DataBind();
                }
            }
        }

        private void LoadAllAPIs()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // Retrieve logged-in user's username from session
            string username = Session["User"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                Response.Redirect("~/UI/SignInPage.aspx");
                return;
            }

            // Fetch the user_id for the logged-in username
            int userId = GetUserIdByUsername(username);
            if (userId == 0)
            {
                // Handle case where user ID is not found
                //lblError.Text = "Unable to load user data.";
                return;
            }

            string query = $@"
        SELECT 
            am.API_id, 
            am.API_name, 
            am.API_desc, 
            ah.code_uploadDate, 
            am.API_endpoint, 
            am.API_HTTP_method
        FROM 
            api_methods am
        JOIN 
            api_header ah ON am.code_id = ah.code_id
        WHERE 
            ah.user_id = @UserId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptResults.DataSource = dt;
                    rptResults.DataBind();
                }
            }
        }

        // Helper method to fetch the user_id by username
        private int GetUserIdByUsername(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            int userId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT user_id FROM users WHERE user_username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out userId))
                    {
                        return userId;
                    }
                }
            }

            return userId; // Returns 0 if user not found
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Redirect")
            {
                string apiId = e.CommandArgument.ToString();
                Response.Redirect($"ViewDetailPage.aspx?apiId={apiId}");
            }
            else if (e.CommandName == "Sort")
            {
                // Extract sorting field and direction
                string sortExpression = e.CommandArgument.ToString();

                // Retrieve the search term from ViewState (if exists)
                string searchTerm = ViewState["SearchTerm"] as string;


                // If no search term, just load all APIs with sorting
                LoadAllAPIs();

            }
        }
    }
}