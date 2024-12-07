using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;


namespace API_DAMs.UI
{
    public partial class landingPage : System.Web.UI.Page
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Any page load logic
            if (!IsPostBack)
            {
                // Initial page setup if needed
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // User Registration Method
        protected void SignUp_Click(object sender, EventArgs e)
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string repassword = Request.Form["repassword"];
            string username = Request.Form["username"];

            // Validation
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(username))
            {
                // Show error message
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('All fields are required!');", true);
                return;
            }

            // Check if passwords match
            if (password != repassword)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Passwords do not match!');", true);
                return;
            }

            // Hash the password
            string hashedPassword = HashPassword(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if user already exists
                    string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email OR Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        checkCmd.Parameters.AddWithValue("@Username", username);

                        int userCount = (int)checkCmd.ExecuteScalar();
                        if (userCount > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Email or Username already exists!');", true);
                            return;
                        }
                    }

                    // Insert new user
                    string insertQuery = @"
                        INSERT INTO Users (Email, Username, PasswordHash) 
                        VALUES (@Email, @Username, @Password)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        cmd.ExecuteNonQuery();
                    }

                    // Create authentication cookie
                    CreateAuthCookie(username);

                    // Redirect to dashboard or home page
                    Response.Redirect("Dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                // Log the full exception
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Registration failed: {ex.Message}');", true);
            }
        }

        // User Login Method
        protected void SignIn_Click(object sender, EventArgs e)
        {
            string loginIdentifier = Request.Form["login-email"];
            string password = Request.Form["login-password"];

            // Validation
            if (string.IsNullOrWhiteSpace(loginIdentifier) || string.IsNullOrWhiteSpace(password))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please enter both email/username and password!');", true);
                return;
            }

            // Hash the password for comparison
            string hashedPassword = HashPassword(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check user credentials (allow login with either email or username)
                    string loginQuery = @"
                        SELECT UserId, Username 
                        FROM Users 
                        WHERE (Email = @LoginIdentifier OR Username = @LoginIdentifier) 
                        AND Password = @Password";

                    using (SqlCommand cmd = new SqlCommand(loginQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoginIdentifier", loginIdentifier);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User found, create authentication cookie
                                int userId = reader.GetInt32(0);
                                string username = reader.GetString(1);

                                CreateAuthCookie(username);

                                // Redirect to dashboard
                                Response.Redirect("Dashboard.aspx");
                            }
                            else
                            {
                                // Invalid credentials
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid login credentials!');", true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Login failed: {ex.Message}');", true);
            }
        }

        // Create Authentication Cookie
        private void CreateAuthCookie(string username)
        {
            // Create an authentication ticket
            HttpCookie authCookie = new HttpCookie("UserAuth");
            authCookie.Value = username;
            authCookie.Expires = DateTime.Now.AddDays(7); // Cookie valid for 7 days
            Response.Cookies.Add(authCookie);
        }

        // Logout Method (you can call this from a logout button)
        protected void Logout_Click(object sender, EventArgs e)
        {
            // Remove the authentication cookie
            HttpCookie authCookie = new HttpCookie("UserAuth");
            authCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(authCookie);

            // Redirect to login page
            Response.Redirect("SignPage.aspx");
        }
    }
}
