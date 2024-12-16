using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class CollaboratorProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["User"] != null)
            {
                // Retrieve user_id from the query string
                string userId = Request.QueryString["userId"];

                if (!string.IsNullOrEmpty(userId))
                {
                    // Load the profile for the specified user_id
                    LoadUserProfile(userId);
                }
                else
                {
                    // Optionally redirect or show an error if no user_id is provided
                    Response.Redirect("~/UI/CollaboratorPage.aspx");
                }
            }
            else
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("~/UI/SignInPage.aspx");
            }
        }


        private void LoadUserProfile(string userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT user_id, user_username, user_email, user_phone, user_name, user_image, user_joined_date, user_visibility 
                    FROM users 
                    WHERE user_id = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            litUsername.Text = reader["user_username"].ToString();
                            litEmail.Text = reader["user_email"].ToString();
                            litID.Text = reader["user_id"].ToString();
                            litName.Text = reader["user_name"].ToString();
                            litPhone.Text = reader["user_phone"].ToString();
                            litJD.Text = reader["user_joined_date"].ToString();

                            bool isPublic = Convert.ToBoolean(reader["user_visibility"]);
                            LitVisibility.Text = isPublic ? "Public" : "Private";

                            string imagePath = reader["user_image"] as string;
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                litImage.Text = $"<img src='{ResolveUrl(imagePath)}' alt='Profile Image' />";
                            }
                            else
                            {
                                litImage.Text = $"<img src='{ResolveUrl("~/icon/default-profile.png")}' alt='Default Profile Image' />";
                            }
                        }
                    }
                }
            }
        }

    }
}