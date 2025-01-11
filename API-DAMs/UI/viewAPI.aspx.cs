﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity.Core.Common.CommandTrees;

namespace API_DAMs.UI
{
    public partial class viewAPI : System.Web.UI.Page
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

        protected void btnSaveApp_Click(object sender, EventArgs e)
        {
            // Get the connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // Retrieve values from the form
            string appName = txtAppName.Text;
            string testingPath = txtAppTestingPath.Text;
            string productionPath = txtAppProductionPath.Text;
            string language = txtAppLanguage.Text;

            // Logic to insert data into the "application" table
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO application (app_name, app_testing_path, app_production_path, app_language) " +
                               "VALUES (@AppName, @TestingPath, @ProductionPath, @Language)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@AppName", appName);
                    cmd.Parameters.AddWithValue("@TestingPath", string.IsNullOrEmpty(testingPath) ? DBNull.Value : (object)testingPath);
                    cmd.Parameters.AddWithValue("@ProductionPath", string.IsNullOrEmpty(productionPath) ? DBNull.Value : (object)productionPath);
                    cmd.Parameters.AddWithValue("@Language", string.IsNullOrEmpty(language) ? DBNull.Value : (object)language);

                    // Open connection and execute the query
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Optionally, reload the page or update the UI to reflect the changes
            Response.Redirect(Request.RawUrl);
        }


        // Method to load applications dropdown
        private void LoadApplications()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT app_id, app_name FROM application";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlApplications.DataSource = reader;
                    ddlApplications.DataTextField = "app_name";
                    ddlApplications.DataValueField = "app_id";
                    ddlApplications.DataBind();
                }
            }
            ddlApplications.Items.Insert(0, new ListItem("-- Select Application --", "0"));
        }

        // Method to load application details
        private ApplicationDetails LoadApplicationDetails(int appId)
        {
            ApplicationDetails details = null;
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT app_name, app_testing_path, app_production_path, app_language 
                        FROM application 
                        WHERE app_id = @AppId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppId", appId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            details = new ApplicationDetails
                            {
                                Name = reader["app_name"].ToString(),
                                TestingPath = reader["app_testing_path"] != DBNull.Value ? reader["app_testing_path"].ToString() : "N/A",
                                ProductionPath = reader["app_production_path"] != DBNull.Value ? reader["app_production_path"].ToString() : "N/A",
                                Language = reader["app_language"] != DBNull.Value ? reader["app_language"].ToString() : "N/A"
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
                int selectedAppId = int.Parse(ddlApplications.SelectedValue);
                ApplicationDetails appDetails = LoadApplicationDetails(selectedAppId);
                lblHeader.Text = "Viewing Application: " + ddlApplications.SelectedItem?.Text ?? "Unknown Application";

                if (appDetails != null)
                {
                    // Show the details card
                    appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"].Replace("d-none", "").Trim();

                    // Populate the textboxes
                    txtAppNameEdit.Text = appDetails.Name;
                    txtAppTestingPathEdit.Text = appDetails.TestingPath;
                    txtAppProductionPathEdit.Text = appDetails.ProductionPath;
                    txtAppLanguageEdit.Text = appDetails.Language;

                    // Set controls to view mode
                    SetViewMode();
                }
                LoadAPIsByAppId(selectedAppId);
                apiResultsContainer.Visible = true;
            }
            else
            {
                // Hide the details card
                appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";
                rptResults.DataSource = null;
                rptResults.DataBind();
                apiResultsContainer.Visible = false;
            }
        }

        // Edit button click
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            SetEditMode();
        }

        // Save button click
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlApplications.SelectedValue != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE application 
                           SET app_testing_path = @TestingPath,
                               app_production_path = @ProductionPath,
                               app_language = @Language
                           WHERE app_id = @AppId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppId", int.Parse(ddlApplications.SelectedValue));
                        cmd.Parameters.AddWithValue("@TestingPath",
                            string.IsNullOrEmpty(txtAppTestingPathEdit.Text) || txtAppTestingPathEdit.Text == "N/A"
                                ? (object)DBNull.Value
                                : txtAppTestingPathEdit.Text);
                        cmd.Parameters.AddWithValue("@ProductionPath",
                            string.IsNullOrEmpty(txtAppProductionPathEdit.Text) || txtAppProductionPathEdit.Text == "N/A"
                                ? (object)DBNull.Value
                                : txtAppProductionPathEdit.Text);
                        cmd.Parameters.AddWithValue("@Language",
                            string.IsNullOrEmpty(txtAppLanguageEdit.Text) || txtAppLanguageEdit.Text == "N/A"
                                ? (object)DBNull.Value
                                : txtAppLanguageEdit.Text);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reset to view mode and refresh data
                SetViewMode();
                ddlApplications_SelectedIndexChanged(null, null);
            }
        }

        // Cancel button click
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SetViewMode();
            ddlApplications_SelectedIndexChanged(null, null);
        }

        // Helper method to set view mode
        private void SetViewMode()
        {
            txtAppNameEdit.ReadOnly = true;
            txtAppTestingPathEdit.ReadOnly = true;
            txtAppProductionPathEdit.ReadOnly = true;
            txtAppLanguageEdit.ReadOnly = true;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }

        // Helper method to set edit mode
        private void SetEditMode()
        {
            txtAppNameEdit.ReadOnly = true; // Keep name readonly
            txtAppTestingPathEdit.ReadOnly = false;
            txtAppProductionPathEdit.ReadOnly = false;
            txtAppLanguageEdit.ReadOnly = false;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;
        }

        private void LoadAPIsByAppId(int appId)
        {
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
            api_methods am
        JOIN 
            api_header ah ON am.code_id = ah.code_id
        WHERE 
            am.app_id = @AppId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppId", appId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

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