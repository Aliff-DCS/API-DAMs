using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class HeaderHome : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    string username = Session["User"].ToString();
                    litUsername.Text = username;

                    // Load user profile image
                    LoadUserProfileImage(username);
                }
                else
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                }
            }
        }

        private void LoadUserProfileImage(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT user_image FROM users WHERE user_username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();

                    object imageResult = command.ExecuteScalar();
                    if (imageResult != DBNull.Value && !string.IsNullOrEmpty(imageResult.ToString()))
                    {
                        // If a user image exists, use it
                        string userImagePath = ResolveUrl(imageResult.ToString());
                        imgProfile.Src = userImagePath;
                    }
                    else
                    {
                        // If no image is set, use the default profile image
                        imgProfile.Src = ResolveUrl("~/icon/default-profile.png");
                    }
                }
            }
        }


        protected void Logout_Click(object sender, EventArgs e)
        {
            // Disable event validation for this specific postbac

            // Clear session data
            Session.Clear();
            Session.Abandon();

            // Clear authentication cookies
            if (Request.Cookies[".ASPXAUTH"] != null)
            {
                var cookie = new HttpCookie(".ASPXAUTH")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Client-side script to show alert and redirect
            string script = @"
                alert('You have been successfully logged out.');
                window.location.href='SignInPage.aspx';
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "LogoutAlert", script, true);
        }

        // Handler for Profile Settings click
        protected void ProfileSetting_Click(object sender, EventArgs e)
        {
            // Redirect to the profile settings page
            Response.Redirect("~/UI/ProfilePage.aspx"); // Replace with your actual profile settings page URL
        }

        protected void OwnAPI_Click(object sender, EventArgs e)
        {
            // Redirect to the profile settings page
            Response.Redirect("~/UI/viewAPI.aspx"); // Replace with your actual profile settings page URL
        }

        protected void SharedAPI_Click(object sender, EventArgs e)
        {
            // Redirect to the profile settings page
            Response.Redirect("~/UI/sharedViewAPI.aspx"); // Replace with your actual profile settings page URL
        }

    }
}