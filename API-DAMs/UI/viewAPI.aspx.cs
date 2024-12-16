using System;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    LoadAllAPIs();
                }
                else
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
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