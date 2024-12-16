using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.Json;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;



namespace API_DAMs.UI
{
    public partial class ViewDetailPage : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            string apiId = Request.QueryString["apiId"];

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(apiId))
                {
                    // Load API Details
                    await LoadAPIDetailsAsync(apiId);

                    // Load Collaborators dynamically
                    await LoadCollaboratorsAsync(apiId);
                }
                else
                {
                    DisplayDummyData();
                }
            }
            else
            {
                // Rebuild dynamic controls to ensure they are present during postback
                if (!string.IsNullOrEmpty(apiId))
                {
                    RebuildDynamicControls(apiId);
                }
            }
        }

        private async Task LoadCollaboratorsAsync(string apiId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.collab_id, 
                        u1.user_name AS OwnerName, 
                        u2.user_name AS SharedName,
                        c.collab_permission
                    FROM collaborator c
                    INNER JOIN users u1 ON c.owner_id = u1.user_id
                    INNER JOIN users u2 ON c.shared_id = u2.user_id
                    WHERE c.API_id = @API_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@API_id", apiId);

                    await conn.OpenAsync();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // Bind to Repeater
                        CollaboratorRepeater.DataSource = dt;
                        CollaboratorRepeater.DataBind();
                    }
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<FriendSearchResult> SearchFriends(string searchTerm)
        {
            List<FriendSearchResult> results = new List<FriendSearchResult>();

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    throw new ArgumentException("Search term cannot be empty");
                }

                // Check if user is logged in
                if (HttpContext.Current.Session["UserID"] == null)
                {
                    throw new InvalidOperationException("User not logged in");
                }

                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
            SELECT TOP 10 
                u.user_id AS UserId, 
                u.user_username AS Username, 
                u.user_email AS Email
            FROM users u
            WHERE 
                u.user_id != @CurrentUserID AND 
                u.user_visibility = 1 AND
                (u.user_username LIKE @SearchTerm OR u.user_email LIKE @SearchTerm)
            ";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CurrentUserID", currentUserId);
                    cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        results.Add(new FriendSearchResult
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception
                System.Diagnostics.Debug.WriteLine($"Error in SearchFriends: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Throw the exception to provide more details
                throw;
            }

            return results;
        }

        [WebMethod]
        public static bool AddCollaborators(List<CollaboratorInfo> collaborators)
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (var collaborator in collaborators)
                    {
                        string query = @"
                INSERT INTO collaborator 
                (initiator_id, receiver_id, friend_req, friend_status) 
                VALUES 
                (@InitiatorId, @ReceiverId, 1, 0)
                ";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@InitiatorId", currentUserId);
                            cmd.Parameters.AddWithValue("@ReceiverId", collaborator.UserId);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error in AddCollaborators: {ex.Message}");
                return false;
            }
        }

        // Helper classes for type safety
        public class FriendSearchResult
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }

        public class CollaboratorInfo
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }


        protected void EditButton_Click(object sender, EventArgs e)
        {
            // Enable all TextBoxes for editing (static ones)
            API_Name.ReadOnly = false;
            Endpoint.ReadOnly = false;
            Param_req.ReadOnly = false;
            Method_Type.ReadOnly = false;
            PostMethodText.ReadOnly = false;
            JSONMethodText.ReadOnly = false;
            Description.ReadOnly = false;


            // Show the "Save Edit" button
            SaveEditButton.Visible = true;

            // Hide the ParameterPlaceholder
            ParameterPlaceholder.Visible = false;
        }

        protected void SaveEditButton_Click(object sender, EventArgs e)
        {
            // Get apiId from the query string
            string apiId = Request.QueryString["apiId"];

            if (!string.IsNullOrEmpty(apiId))
            {
                // Call the method to save the changes to the database
                SaveAPIChangesToDatabase(apiId);

                // Re-disable the static TextBoxes after saving
                API_Name.ReadOnly = true;
                Endpoint.ReadOnly = true;
                Param_req.ReadOnly = true;
                Method_Type.ReadOnly = true;
                PostMethodText.ReadOnly = true;
                JSONMethodText.ReadOnly = true;
                Description.ReadOnly = true;

                // Hide the "Save Edit" button after saving
                SaveEditButton.Visible = false;

                // Show the ParameterPlaceholder again after saving
                ParameterPlaceholder.Visible = true;
            }
        }

        private void SaveAPIChangesToDatabase(string apiId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // Determine the value to store for POST_Method based on PostMethodText
            string postMethod = PostMethodText.Text.ToLower() == "url parameter" ? "url_param" :
                                PostMethodText.Text.ToLower() == "json string" ? "json_string" :
                                PostMethodText.Text;  // Default to the input value if it's neither of the above

            // Update query for saving API details
            string updateApiQuery = @"
UPDATE api_methods
SET API_name = @API_Name,
    API_endpoint = @ApiEndpoint,
    API_desc = @Description,
    API_HTTP_method = @HTTP_Method,
    API_post_method = @POST_Method
WHERE API_id = @API_ID";

            // Update query for updating UploadDate in api_header
            string updateUploadDateQuery = @"
UPDATE api_header
SET code_uploadDate = @UploadDate
WHERE code_id = (SELECT code_id FROM api_methods WHERE API_id = @API_ID)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Update api_methods table
                using (SqlCommand cmd = new SqlCommand(updateApiQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@API_Name", API_Name.Text);
                    cmd.Parameters.AddWithValue("@ApiEndpoint", Endpoint.Text);
                    cmd.Parameters.AddWithValue("@Description", Description.Text);
                    cmd.Parameters.AddWithValue("@HTTP_Method", Method_Type.Text);
                    cmd.Parameters.AddWithValue("@POST_Method", postMethod);  // Use the determined postMethod value
                    cmd.Parameters.AddWithValue("@API_ID", apiId);

                    cmd.ExecuteNonQuery();
                }

                // Update the UploadDate in api_header
                using (SqlCommand cmd = new SqlCommand(updateUploadDateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UploadDate", DateTime.Now);  // Set to current date and time
                    cmd.Parameters.AddWithValue("@API_ID", apiId);

                    cmd.ExecuteNonQuery();
                }

                // Update or insert into parameter_details table
                SaveParameterDetails(apiId, conn);
            }
        }

        private void SaveParameterDetails(string apiId, SqlConnection conn)
        {
            // Step 1: Delete all existing parameters for the API_ID
            string deleteOldParamsQuery = @"
DELETE FROM parameter_details
WHERE API_id = @API_ID";

            using (SqlCommand deleteCmd = new SqlCommand(deleteOldParamsQuery, conn))
            {
                deleteCmd.Parameters.AddWithValue("@API_ID", apiId);
                deleteCmd.ExecuteNonQuery();
            }

            // Step 2: Insert new parameters (no need to check existence since we're deleting first)
            string insertQuery = @"
    INSERT INTO parameter_details (para_type, para_name, para_json_keys, API_id)
    VALUES (@ParaType, @ParaName, @json_Keys, @API_ID)";

            // Extract parameter names and types from Param_req TextBox
            string[] paramDetails = Param_req.Text.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            string jsonKey = JSONMethodText.Text;

            for (int i = 0; i < paramDetails.Length; i++)
            {
                // Split the parameters into name and type (e.g., "name(string)")
                string[] parts = paramDetails[i].Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    string paramName = parts[0].Trim();  // Extracted parameter name
                    string paramType = parts[1].Trim();  // Extracted parameter type

                    // Insert the new parameter
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@ParaType", paramType);
                        insertCmd.Parameters.AddWithValue("@ParaName", paramName);
                        insertCmd.Parameters.AddWithValue("@json_Keys", jsonKey);
                        insertCmd.Parameters.AddWithValue("@API_ID", apiId);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }



        private async Task LoadAPIDetailsAsync(string apiId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = @"
        SELECT API_name, API_endpoint, API_desc AS Description, API_returnType, API_HTTP_method AS HTTP_Method, API_post_method AS POST_Method
        FROM api_methods 
        WHERE API_id = @API_ID";

            string parameterQuery = @"
        SELECT para_type AS ParaType, para_name AS ParaName, para_json_keys AS json_Keys
        FROM parameter_details
        WHERE API_id = @API_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Load API details
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@API_ID", apiId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            API_Name.Text = reader["API_name"].ToString();
                            Endpoint.Text = reader["API_endpoint"].ToString();
                            Description.Text = reader["Description"].ToString();
                            Method_Type.Text = reader["HTTP_Method"].ToString();

                            string methodType = reader["HTTP_Method"].ToString();
                            string postMethod = reader["POST_Method"].ToString();
                            string endpoint = reader["API_endpoint"].ToString();

                            reader.Close(); // Close the first reader before executing the second query

                            if (methodType.Equals("GET", StringComparison.OrdinalIgnoreCase))
                            {
                                LoadParameterDetails(apiId);
                                if (endpoint.Contains("{"))
                                {
                                    GenerateParameterInputs(apiId, endpoint);
                                }
                                else
                                {
                                    string jsonString = await GetJsonStringForAPI(endpoint, new Dictionary<string, string>());
                                    Output.Text = jsonString;
                                }
                            }
                            else if (methodType.Equals("POST", StringComparison.OrdinalIgnoreCase))
                            {
                                LoadParameterDetails(apiId);
                                PostMethodPanel.Visible = true;
                                if (postMethod.Equals("url_param", StringComparison.OrdinalIgnoreCase) && endpoint.Contains("{"))
                                {
                                    PostMethodText.Text = "URL Parameter";
                                    GenerateParameterInputs(apiId, endpoint);
                                }
                                else if (postMethod.Equals("json_string", StringComparison.OrdinalIgnoreCase))
                                {
                                    PostMethodText.Text = "JSON String";
                                    JSONKey.Visible = true;
                                    GenerateParameterInputsPOST(apiId, endpoint);

                                    // Load POST parameters
                                    using (SqlCommand paramCmd = new SqlCommand(parameterQuery, conn))
                                    {
                                        paramCmd.Parameters.AddWithValue("@API_ID", apiId);
                                        using (SqlDataReader paramReader = await paramCmd.ExecuteReaderAsync())
                                        {
                                            while (await paramReader.ReadAsync())
                                            {
                                                JSONMethodText.Text = paramReader["json_Keys"]?.ToString();
                                            }
                                        }
                                    }
                                }
                                else if (postMethod.Equals("NONE", StringComparison.OrdinalIgnoreCase))
                                {
                                    PostMethodText.Text = "NONE";
                                    string jsonString = await PostJsonStringForAPI(endpoint, new Dictionary<string, string>());
                                    Output.Text = jsonString;
                                }
                            }
                            else
                            {
                                Output.Text = "Invalid Method Type";
                            }
                        }
                        else
                        {
                            DisplayDummyData();
                        }
                    }
                }
            }
        }


        private void GenerateParameterInputsPOST(string apiId, string endpoint)
        {
            // Clear existing controls if any
            ParameterPlaceholder.Controls.Clear();
            Output.Visible = false;

            // Create a container div for styling purposes
            var containerDiv = new Panel
            {
                CssClass = "parameter-container border p-3" // Add custom and Bootstrap classes
            };

            // Add the header
            var header = new LiteralControl("<h4 class='mb-3 text-center custom-header'>Please insert the JSON String here:</h4>");
            containerDiv.Controls.Add(header);

            // Create a multiline TextBox for JSON input
            var jsonInputTextBox = new TextBox
            {
                ID = "JsonInputTextBox",
                TextMode = TextBoxMode.MultiLine,
                CssClass = "form-control", // Add Bootstrap form-control class for styling
                Rows = 10, // Set the number of rows for the TextBox
                Columns = 50 // Set the number of columns for the TextBox
            };
            containerDiv.Controls.Add(jsonInputTextBox);

            // Optionally, add a button to submit the JSON string
            var submitButton = new Button
            {
                ID = "SubmitJsonButton",
                Text = "Submit JSON",
                CssClass = "btn btn-primary mt-3" // Add Bootstrap button classes for styling
            };
            submitButton.Click += SubmitJsonButton_Click; // Event handler for processing the JSON input
            containerDiv.Controls.Add(submitButton);
            containerDiv.Controls.Add(Output);

            // Add the container to the placeholder
            ParameterPlaceholder.Controls.Add(containerDiv);

        }

        protected async void SubmitJsonButton_Click(object sender, EventArgs e)
        {
            // Retrieve the JSON string entered by the user
            var jsonInputTextBox = (TextBox)ParameterPlaceholder.FindControl("JsonInputTextBox");
            string jsonString = jsonInputTextBox?.Text;

            // Ensure the JSON string is not empty
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                // Assuming the endpoint is stored in a control or passed as a parameter
                string apiEndpoint = Endpoint.Text;

                // Make the POST request with the JSON string
                string result = await PostJsonStringForAPI(apiEndpoint, jsonString);

                // Display the result in the output control
                Output.Visible = true;
                Output.Text = FormatJsonOutput(result);
            }
            else
            {
                // Handle case when JSON input is empty
                Output.Visible = true;
                Output.Text = "Please enter a valid JSON string.";
            }
        }

        private async Task<string> PostJsonStringForAPI(string apiEndpoint, string jsonPayload)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Return detailed error information
                        return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP request errors
                return $"HTTP Request Error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                // Handle other errors
                return $"General Error: {ex.Message}";
            }
        }

        private string FormatJsonOutput(string jsonString)
        {
            try
            {
                // Attempt to format the JSON response
                using (var doc = System.Text.Json.JsonDocument.Parse(jsonString))
                {
                    return System.Text.Json.JsonSerializer.Serialize(doc, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // If parsing fails, return the raw response
                return jsonString;
            }
        }

        private void LoadParameterDetails(string apiId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = @"
        SELECT para_type AS ParaType, para_name AS ParaName
        FROM parameter_details 
        WHERE API_id = @API_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@API_ID", apiId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    StringBuilder parameters = new StringBuilder();
                    while (reader.Read())
                    {
                        if (parameters.Length > 0)
                        {
                            parameters.Append(", ");
                        }
                        parameters.Append($"{reader["ParaName"]}({reader["ParaType"]})");
                    }
                    Param_req.Text = parameters.Length > 0 ? parameters.ToString() : "null";
                }
            }
        }


        private void GenerateParameterInputs(string apiId, string endpointTemplate)
        {
            // Clear existing controls if any
            ParameterPlaceholder.Controls.Clear();
            Output.Visible = false;

            // Create a container div for styling purposes
            var containerDiv = new Panel
            {
                ID = $"test",
                CssClass = "parameter-container border p-3" // Add custom and Bootstrap classes
            };

            // Add the header
            var header = new LiteralControl("<h4 class='mb-3 text-center custom-header'>Please insert the parameter:</h4>");
            containerDiv.Controls.Add(header);

            // Create a Bootstrap row for the parameters
            var rowDiv = new Panel
            {
                CssClass = "row" // Use Bootstrap row class
            };

            string[] endpointParts = endpointTemplate.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < endpointParts.Length; i += 2)
            {
                string paramName = endpointParts[i];

                var colDiv = new Panel
                {
                    CssClass = "col-md-4 mb-3" // Use Bootstrap column class for 3 columns in a row
                };

                Label lbl = new Label
                {
                    Text = $"{paramName}: ",
                    AssociatedControlID = $"txt_{paramName}",
                    CssClass = "form-label"
                };

                TextBox txt = new TextBox
                {
                    ID = $"txt_{paramName}",
                    CssClass = "form-control"
                };

                // Add label and textbox to the column div
                colDiv.Controls.Add(lbl);
                colDiv.Controls.Add(txt);

                // Add column div to the row div
                rowDiv.Controls.Add(colDiv);
            }

            // Add the row div to the container div
            containerDiv.Controls.Add(rowDiv);

            Button btnSubmit = new Button
            {
                Text = "Submit",
                CssClass = "btn btn-primary mt-3"
            };
            btnSubmit.Click += async (sender, e) => await SubmitButton_Click(sender, e, endpointTemplate);

            containerDiv.Controls.Add(btnSubmit);
            containerDiv.Controls.Add(new LiteralControl("<br />"));
            containerDiv.Controls.Add(Output);

            ParameterPlaceholder.Controls.Add(containerDiv);
        }

        private async Task<string> GetJsonStringForAPI(string apiEndpoint, Dictionary<string, string> parameters)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "FYP_MockUp");

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiEndpoint);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Log the status code and raw JSON response
                    Debug.WriteLine($"Status Code: {response.StatusCode}");
                    Debug.WriteLine($"Response Content: {responseContent}");

                    // Return the response content along with status code
                    return responseContent;
                }
                catch (HttpRequestException ex)
                {
                    // Handle the case where the request fails, return the exception message
                    return $"Exception: {ex.Message}";
                }
            }
        }

        private void TraverseControls(Control control, Dictionary<string, string> parameters)
        {
            foreach (Control child in control.Controls)
            {
                if (child is TextBox textBox)
                {
                    // Process the TextBox control
                    string paraName = textBox.ID.Replace("txt_", "");
                    string paraValue = textBox.Text;
                    System.Diagnostics.Debug.WriteLine($"Parameter Name: {paraName}, Parameter Value: {paraValue}");
                    parameters[paraName] = paraValue;
                }
                else
                {
                    // Recursively search child controls
                    TraverseControls(child, parameters);
                }
            }
        }

        private async Task SubmitButton_Click(object sender, EventArgs e, string endpointTemplate)
        {
            var parameters = new Dictionary<string, string>();

            // Collect parameters from dynamically generated TextBoxes
            var containerDiv = ParameterPlaceholder.Controls.OfType<Panel>().FirstOrDefault();
            if (containerDiv != null)
            {
                // Use recursive method to traverse all controls
                TraverseControls(containerDiv, parameters);
            }


            string finalEndpoint = ReplaceParametersInEndpoint(endpointTemplate, parameters);

            // Determine the method type and get the JSON string accordingly
            string jsonString;
            string methodType = Method_Type.Text;
            if (methodType.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                jsonString = await PostJsonStringForAPI(finalEndpoint, parameters);
            }
            else
            {
                jsonString = await GetJsonStringForAPI(finalEndpoint, parameters);
            }

            string formattedJson;
            try
            {
                // Attempt to format the JSON response
                using (var doc = System.Text.Json.JsonDocument.Parse(jsonString))
                {
                    formattedJson = System.Text.Json.JsonSerializer.Serialize(doc, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // If parsing fails, return the raw response
                formattedJson = jsonString;
            }

            Output.Visible = true;
            Output.Text = formattedJson;
        }

        private async Task<string> PostJsonStringForAPI(string apiEndpoint, Dictionary<string, string> parameters)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent content;
                    if (parameters.Count > 0)
                    {
                        content = new FormUrlEncodedContent(parameters);
                    }
                    else
                    {
                        content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    }

                    HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine($"Status Code: {response.StatusCode}");
                    Debug.WriteLine($"Response Content: {responseContent}");

                    return responseContent;
                }
                catch (HttpRequestException ex)
                {
                    return $"Exception: {ex.Message}";
                }
            }
        }

        private string ReplaceParametersInEndpoint(string endpointTemplate, Dictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                endpointTemplate = endpointTemplate.Replace($"{{{param.Key}}}", param.Value);
            }
            return endpointTemplate;
        }

        private void DisplayDummyData()
        {
            API_Name.Text = "Dummy API Name";
            Endpoint.Text = "Dummy Endpoint";
            Description.Text = "Dummy Description";
            Param_req.Text = "null";
            Method_Type.Text = "Dummy Method Type";
            Output.Text = "Dummy Output";
        }

        private void RebuildDynamicControls(string apiId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = @"
                SELECT API_endpoint, API_post_method 
                FROM api_methods 
                WHERE API_id = @API_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@API_ID", apiId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string endpointTemplate = reader["API_endpoint"].ToString();
                        string postMethod = reader["API_post_method"].ToString();

                        if (postMethod.Equals("json_string", StringComparison.OrdinalIgnoreCase))
                        {
                            // Call the method for handling JSON string POST method
                            GenerateParameterInputsPOST(apiId, endpointTemplate);
                        }
                        else if (postMethod.Equals("url_param", StringComparison.OrdinalIgnoreCase))
                        {
                            // Call the method for handling URL parameter POST method
                            GenerateParameterInputs(apiId, endpointTemplate);
                        }
                    }
                }
            }
        }
        protected void SaveCollaborationButton_Click(object sender, EventArgs e)
        {
            // Hardcoded names and permissions for demonstration
            var collaborators = new[]
            {
                new { Name = "John Doe", Permission = Request.Form["Permission1"] },
                new { Name = "Jane Smith", Permission = Request.Form["Permission2"] }
            };

            foreach (var collaborator in collaborators)
            {
                System.Diagnostics.Debug.WriteLine($"Collaborator: {collaborator.Name}, Permission: {collaborator.Permission}");
                // Add database save logic here (if needed)
            }

            // Optionally show a success message
            ScriptManager.RegisterStartupScript(this, GetType(), "collabSuccess", "alert('Collaborators saved successfully!');", true);
        }


    }
}