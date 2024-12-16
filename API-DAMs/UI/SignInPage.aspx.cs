using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;


namespace API_DAMs.UI
{
    public partial class SignInPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear any previous error messages on initial page load
            if (!IsPostBack)
            {
                lblErrorMessage.Text = string.Empty;
            }
        }

        protected void SignIn_Click(object sender, EventArgs e)
        {
            // Clear previous error messages
            lblErrorMessage.Text = string.Empty;

            string emailOrUsername = txtEmailU.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                var userInfo = ValidateUserAndGetUserInfo(emailOrUsername, password);

                if (userInfo != null)
                {
                    // Set session to store user ID and username
                    Session["UserID"] = userInfo.Item1; // User ID
                    Session["User"] = userInfo.Item2; // Username
                    Session.Timeout = 30; // Optional: session timeout in minutes

                    // Redirect to a dashboard or landing page after login
                    Response.Redirect("HomePage.aspx");
                }
                else
                {
                    lblErrorMessage.Text = "Invalid email/username or password.";
                }
            }
            catch (Exception ex)
            {
                // Log the error (use proper logging in a real-world app)
                lblErrorMessage.Text = $"An error occurred: {ex.Message}";
            }
        }

        private Tuple<int, string> ValidateUserAndGetUserInfo(string emailOrUsername, string password)
        {
            // Hash the input password
            string hashedPassword = HashPassword(password);

            // Connection string from web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT user_id, user_username 
                FROM users 
                WHERE (user_email = @EmailOrUsername OR user_username = @EmailOrUsername) 
                  AND user_password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@EmailOrUsername", emailOrUsername);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Read user ID and username from the query result
                            int userId = reader.GetInt32(0);
                            string username = reader.GetString(1);

                            return Tuple.Create(userId, username);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }



        private string HashPassword(string password)
        {
            // Use SHA-256 to hash the password
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
