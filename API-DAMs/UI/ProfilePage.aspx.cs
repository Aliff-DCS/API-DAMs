using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class ProfilePage1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["User"] != null)
            {
                // Retrieve user information from the database
                string username = Session["User"].ToString();
                LoadUserProfile(username);
            }
            else
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("~/UI/SignInPage.aspx");
            }

        }

        private void LoadUserProfile(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT user_id, user_username, user_email, user_phone, user_name, user_image, user_joined_date FROM users WHERE user_username = @Username";

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

                        // Assign the profile image source
                        if (reader["user_image"] != DBNull.Value && !string.IsNullOrEmpty(reader["user_image"].ToString()))
                        {
                            // If user_image is not null or empty, use the value
                            string imagePath = reader["user_image"].ToString();
                            litImage.Text = $"<img src='{ResolveUrl(imagePath)}' alt='Profile Image' />";
                        }
                        else
                        {
                            // If user_image is null or empty, set a default image
                            string defaultImagePath = "~/icon/default-profile.png";
                            litImage.Text = $"<img src='{ResolveUrl(defaultImagePath)}' alt='Default Profile Image' />";
                        }

                    }
                }
            }
        }

        protected void btnAddMore_Click(object sender, EventArgs e)
        {
            // Populate textboxes with the current data
            txtID.Text = litID.Text;
            txtName.Text = litName.Text;
            txtEmail.Text = litEmail.Text;
            txtPhone.Text = litPhone.Text;

            // Toggle panels
            pnlViewMode.Visible = false;
            pnlEditMode.Visible = true;
        }

        protected void btnSubmitEdit_Click(object sender, EventArgs e)
        {
            // Update the database with new values
            string username = Session["User"].ToString();
            string name = txtName.Text;
            string email = txtEmail.Text;
            string phone = txtPhone.Text;

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE users SET user_name = @Name, user_email = @Email, user_phone = @Phone WHERE user_username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Reload profile information
            LoadUserProfile(username);

            // Switch back to view mode
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
        }

        protected void btnEditProfile_Click(object sender, EventArgs e)
        {
            // Populate textboxes with the current data
            txtID.Text = litID.Text;
            txtName.Text = litName.Text;
            txtEmail.Text = litEmail.Text;
            txtPhone.Text = litPhone.Text;

            // Toggle panels
            pnlViewMode.Visible = false;
            pnlEditMode.Visible = true;
        }


        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            // Discard changes and switch back to view mode
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
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
