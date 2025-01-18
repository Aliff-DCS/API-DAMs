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
    public partial class ProfilePage1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] != null)
            {
                btnEditProfile.Visible = false;
                if (Request.QueryString["userId"] != null)
                {
                    string userId = Request.QueryString["userId"];
                    string username = GetUsernameById(userId);

                    if (!string.IsNullOrEmpty(username))
                    {
                        LoadUserProfile(username);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No username found for the given userId.");
                    }
                }
                else
                {
                    btnEditProfile.Visible = true;

                    string username = Session["User"].ToString();
                    LoadUserProfile(username);
                }
            }
            else
            {
                Response.Redirect("~/UI/SignInPage.aspx");
            }
        }

        private string GetUsernameById(string userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT user_username FROM users WHERE user_id = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    return result != null ? result.ToString() : null;
                }
            }
        }


        private void LoadUserProfile(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT user_id, user_username, user_email, user_phone, user_name, user_image, user_joined_date, user_visibility, user_tagline FROM users WHERE user_username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        litUsername.Text = reader["user_username"].ToString();
                        litEmail.Text = reader["user_email"].ToString();
                        litID.Text = reader["user_id"].ToString();
                        litName.Text = reader["user_name"].ToString();
                        litPhone.Text = reader["user_phone"].ToString();
                        litJD.Text = reader["user_joined_date"].ToString();

                        // Assign the tagline value
                        if (reader["user_tagline"] != DBNull.Value && !string.IsNullOrEmpty(reader["user_tagline"].ToString()))
                        {
                            litTagline.Text = reader["user_tagline"].ToString();
                        }
                        else
                        {
                            litTagline.Text = "Tagline";
                        }

                        // Map user_visibility BIT value to "Public" or "Private"
                        bool isPublic = Convert.ToBoolean(reader["user_visibility"]);
                        LitVisibility.Text = isPublic ? "Public" : "Private";

                        // Assign the profile image source
                        if (reader["user_image"] != DBNull.Value && !string.IsNullOrEmpty(reader["user_image"].ToString()))
                        {
                            string imagePath = reader["user_image"].ToString();
                            litImage.Text = $"<img src='{ResolveUrl(imagePath)}' alt='Profile Image' />";
                        }
                        else
                        {
                            string defaultImagePath = "~/icon/default-profile.png";
                            litImage.Text = $"<img src='{ResolveUrl(defaultImagePath)}' alt='Default Profile Image' />";
                        }
                    }
                }
            }
        }


        protected void btnSubmitEdit_Click(object sender, EventArgs e)
        {
            string username = Session["User"].ToString();
            string name = txtName.Text;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;
            string tagline = txtTagline.Text;
            string userDesc = txtEditUserDesc.Value;



            // Get the selected visibility value
            bool isPublic = rblVisibility.SelectedValue == "Public";

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE users 
                    SET user_name = @Name, 
                        user_email = @Email, 
                        user_phone = @Phone, 
                        user_visibility = @Visibility, 
                        user_tagline = @Tagline, 
                        user_desc = @UserDesc
                    WHERE user_username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Visibility", isPublic); // Pass boolean for BIT column
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Tagline", tagline);
                    command.Parameters.AddWithValue("@UserDesc", userDesc);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Reload profile information
            LoadUserProfile(username);

            // Switch back to view mode
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
            EditableBio.Visible = false;
            StaticBio.Visible = true;
        }

        protected void btnEditProfile_Click(object sender, EventArgs e)
        {
            // Populate textboxes with the current data
            txtID.Text = litID.Text;
            txtName.Text = litName.Text;
            txtEmail.Text = litEmail.Text;
            txtPhone.Text = litPhone.Text;
            txtTagline.Text = litTagline.Text;

            EditableBio.Visible = true;
            StaticBio.Visible = false;

            txtEditUserDesc.Value = txtUserDesc.Value;
            // Set the selected value of the RadioButtonList
            bool isPublic = LitVisibility.Text == "Public";
            rblVisibility.SelectedValue = isPublic ? "Public" : "Private";

            // Toggle panels
            pnlViewMode.Visible = false;
            pnlEditMode.Visible = true;
        }


        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            // Discard changes and switch back to view mode
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
            EditableBio.Visible = false;
            StaticBio.Visible = true;
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblError.Visible = false; // Hide error label initially

            if (fileUpload.HasFile)
            {
                string username = Session["User"].ToString();
                string fileName = Path.GetFileName(fileUpload.PostedFile.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();
                int fileSize = fileUpload.PostedFile.ContentLength;

                // Validate file type
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                {
                    lblError.Text = "Only JPG, JPEG, and PNG formats are allowed.";
                    lblError.Visible = true;
                    return;
                }

                // Validate file size (e.g., max 2MB)
                if (fileSize > 2 * 1024 * 1024)
                {
                    lblError.Text = "The file size must not exceed 3MB.";
                    lblError.Visible = true;
                    return;
                }

                string filePath = "~/UploadedImages/" + fileName;

                // Save the file to the server
                string serverPath = Server.MapPath(filePath);
                fileUpload.SaveAs(serverPath);

                // Save the image path to the database
                SaveImagePathToDatabase(username, filePath);

                // Reload the profile with the new image
                LoadUserProfile(username);
            }
            else
            {
                lblError.Text = "Please select a file to upload.";
                lblError.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel the image change and reload the current profile data
            string username = Session["User"].ToString();
            LoadUserProfile(username);

            // Optionally, hide the preview and buttons again     
            // Example: imagePreview.style.display = 'none';
        }

        private void SaveImagePathToDatabase(string username, string filePath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE users SET user_image = @UserImage WHERE user_username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserImage", filePath);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}