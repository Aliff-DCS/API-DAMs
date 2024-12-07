using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace API_DAMs.UI
{
    public partial class SignPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear any previous error messages on initial page load
            if (!IsPostBack)
            {
                lblErrorMessage.Text = string.Empty;
            }
        }

        protected void SignUp_Click(object sender, EventArgs e)
        {
            // Clear previous error message
            lblErrorMessage.Text = string.Empty;

            // Retrieve input values from the form controls
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string rePassword = txtRePassword.Text;
            string username = txtUsername.Text.Trim();

            // Validate input
            if (!ValidateSignUpForm(email, password, rePassword, username))
            {
                return; // Stop further processing if validation fails
            }

            try
            {
                // Hash the password
                string hashedPassword = HashPassword(password);

                // Attempt to register the user
                if (RegisterUser(email, username, hashedPassword))
                {
                    // Successful registration
                    ScriptManager.RegisterStartupScript(this, GetType(), "RegistrationSuccess",
                        "alert('Registration Successful! Please Sign In.'); window.location.href='SignInPage.aspx';", true);
                }
                else
                {
                    // Registration failed (e.g., email or username already exists)
                    lblErrorMessage.Text = "Registration failed. Email or Username might already exist.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (in a real-world scenario, use proper logging)
                lblErrorMessage.Text = $"An error occurred: {ex.Message}";
            }
        }

        private bool ValidateSignUpForm(string email, string password, string rePassword, string username)
        {
            // Email validation
            if (string.IsNullOrWhiteSpace(email))
            {
                lblErrorMessage.Text = "Email is required.";
                return false;
            }

            // Email format validation using regex
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblErrorMessage.Text = "Invalid email format.";
                return false;
            }

            // Username validation
            if (string.IsNullOrWhiteSpace(username))
            {
                lblErrorMessage.Text = "Username is required.";
                return false;
            }

            // Username length check
            if (username.Length < 3 || username.Length > 50)
            {
                lblErrorMessage.Text = "Username must be between 3 and 50 characters.";
                return false;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(password))
            {
                lblErrorMessage.Text = "Password is required.";
                return false;
            }

            // Password complexity check
            if (password.Length < 8)
            {
                lblErrorMessage.Text = "Password must be at least 8 characters long.";
                return false;
            }

            // Check if passwords match
            if (password != rePassword)
            {
                lblErrorMessage.Text = "Passwords do not match.";
                return false;
            }

            return true;
        }

        private string HashPassword(string password)
        {
            // Use SHA-256 for password hashing
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash from password
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool RegisterUser(string email, string username, string hashedPassword)
        {
            // Connection string from web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL command to insert new user
                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM users WHERE user_email = @Email OR user_username = @Username)
                    BEGIN
                        INSERT INTO users (user_email, user_username, user_password, user_joined_date) 
                        VALUES (@Email, @Username, @Password, @CreatedAt)
                        SELECT 1
                    END
                    ELSE
                    BEGIN
                        SELECT 0
                    END";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                    connection.Open();

                    // Execute the command and check the result
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    return result == 1; // 1 means successful registration, 0 means user already exists
                }
            }
        }
    }
}