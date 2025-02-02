using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class ManageApp : System.Web.UI.Page
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
            if (Session["User"] == null)
            {
                Response.Redirect("~/UI/SignInPage.aspx");
                return;
            }


            int appId;
            string selectedValue = IsPostBack ? ViewState["SelectedAppId"]?.ToString() : ddlApplications.SelectedValue;
            bool isAppIdValid = int.TryParse(selectedValue, out appId);
            if (!IsPostBack)
            {
                if (isAppIdValid && appId > 0)
                {
                    LoadFriendsList();
                    collaboratorList();
                }
                else
                {
                    Console.WriteLine("No AppId selected or invalid AppId.");
                }
            }
        }

        protected async void rptResults_ItemCommand2(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Redirect")
            {
                string apiId = e.CommandArgument.ToString();
                int currentUserId = Convert.ToInt32(Session["UserId"]); // Retrieve current user ID from the session

                // Call the GetUserPermissionAsync method to check the user's permission
                string userPermission = await GetUserPermissionAsync(apiId, currentUserId);

                // Handle the permission result
                if (userPermission == "No Permission")
                {
                    // Show an error message using JavaScript
                    string script = "<script>alert('You do not have permission to access this API.');</script>";
                    ClientScript.RegisterStartupScript(this.GetType(), "NoPermissionAlert", script);
                }
                else
                {
                    // Redirect to the details page if the user has the required permission
                    Response.Redirect($"ViewDetailPage.aspx?apiId={apiId}");
                }
            }
          

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ddlApplications.SelectedValue != null)
            {
                int appId = int.Parse(ddlApplications.SelectedValue);

                // Delete the application and all related data
                bool isDeleted = DeleteApplicationAndRelatedData(appId);

                if (isDeleted)
                {
                    // Show success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "DeleteSuccess", "alert('Application and all its APIs deleted successfully.');", true);
                    Response.Redirect("ManageApp.aspx");


                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "DeleteError", "alert('Failed to delete the application.');", true);
                }
            }
        }

        private bool DeleteApplicationAndRelatedData(int appId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Start a transaction to ensure atomicity
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete from parameter_details
                            string deleteParameterDetailsQuery = @"
                        DELETE FROM parameter_details
                        WHERE API_id IN (SELECT API_id FROM api_methods WHERE app_id = @AppId);";
                            using (SqlCommand cmd = new SqlCommand(deleteParameterDetailsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AppId", appId);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete from api_methods
                            string deleteApiMethodsQuery = @"
                        DELETE FROM api_methods
                        WHERE app_id = @AppId;";
                            using (SqlCommand cmd = new SqlCommand(deleteApiMethodsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AppId", appId);
                                cmd.ExecuteNonQuery();
                            }

                            // Clear app_id from collaborator
                            string clearCollaboratorQuery = @"
                        UPDATE collaborator
                        SET app_id = NULL
                        WHERE app_id = @AppId;";
                            using (SqlCommand cmd = new SqlCommand(clearCollaboratorQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AppId", appId);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete from application
                            string deleteApplicationQuery = @"
                        DELETE FROM application
                        WHERE app_id = @AppId;";
                            using (SqlCommand cmd = new SqlCommand(deleteApplicationQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@AppId", appId);
                                cmd.ExecuteNonQuery();
                            }

                            // Commit the transaction
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            transaction.Rollback();
                            // Log the exception (you can replace this with your logging mechanism)
                            Console.WriteLine("Error deleting application: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine("Error deleting application: " + ex.Message);
                return false;
            }
        }

     

        private async Task<string> GetUserPermissionAsync(string apiId, int currentUserId)
        {
            string permission = "No Permission";

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Query to check ownership or permissions
                string query = @"
            SELECT 
                CASE 
                    WHEN ah.user_id = @currentUserId THEN 'Owner'
                    WHEN EXISTS (
                        SELECT 1 
                        FROM collaborator c 
                        WHERE c.app_id = am.app_id 
                          AND c.shared_id = @currentUserId 
                          AND c.collab_permission = 'write'
                    ) THEN 'Write'
                    WHEN EXISTS (
                        SELECT 1 
                        FROM collaborator c 
                        WHERE c.app_id = am.app_id 
                          AND c.shared_id = @currentUserId 
                          AND c.collab_permission = 'admin'
                    ) THEN 'Admin'
                    WHEN EXISTS (
                        SELECT 1 
                        FROM collaborator c 
                        WHERE c.app_id = am.app_id 
                          AND c.shared_id = @currentUserId 
                          AND c.collab_permission = 'read'
                    ) THEN 'Read'
                    ELSE 'No Permission'
                END AS UserPermission
            FROM api_methods am
            INNER JOIN api_header ah ON am.code_id = ah.code_id
            WHERE am.API_id = @apiId;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@apiId", apiId);
                command.Parameters.AddWithValue("@currentUserId", currentUserId);

                try
                {
                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        permission = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as necessary
                    Console.WriteLine(ex.Message);
                }
            }

            return permission;
        }

        private void CheckEditButtonVisibility(int appId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                CASE 
                    WHEN a.user_id = @UserId THEN 'Owner' -- User is the owner
                    WHEN EXISTS (
                        SELECT 1 
                        FROM collaborator c 
                        WHERE c.app_id = a.app_id 
                        AND c.shared_id = @UserId 
                        AND c.collab_permission = 'write'
                    ) THEN 'Write' -- User has write permission
                    WHEN EXISTS (
                        SELECT 1 
                        FROM collaborator c 
                        WHERE c.app_id = a.app_id 
                        AND c.shared_id = @UserId 
                        AND c.collab_permission = 'admin'
                    ) THEN 'Admin' -- User has admin permission
                    ELSE 'No Permission' -- User has no permission
                END AS UserPermission
            FROM application a
            WHERE a.app_id = @AppId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppId", appId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    string userPermission = cmd.ExecuteScalar()?.ToString();

                    // Set the button visibility based on permission
                    btnEdit.Visible = userPermission != "No Permission";

                    // Display the user's permission in the label
                    if (userPermission == "Owner")
                    {
                        lblPerm.Text = "(You are the owner of this application.)";
                        CollaborationButton.Visible = true;
                        btnDelete.Visible = true;
                    }
                    else if (userPermission == "Write")
                    {
                        lblPerm.Text = "(You have write permissions for this application.)";
                    }
                    else if (userPermission == "Admin")
                    {
                        lblPerm.Text = "(You have admin permissions for this application.)";
                        CollaborationButton.Visible = true;
                        btnDelete.Visible = true;
                    }
                    else
                    {
                        lblPerm.Text = "(You do not have permissions to edit this application.)";
                    }
                }
            }
        }

        private void collaboratorList()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                string currentID = Session["UserID"]?.ToString();
                int appId;

                // Use ViewState value if it's a postback
                string selectedValue = IsPostBack ? ViewState["SelectedAppId"]?.ToString() : ddlApplications.SelectedValue;
                if (!int.TryParse(selectedValue, out appId))
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                    return;
                }

                if (string.IsNullOrEmpty(currentID) || appId <= 0)
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                    return;
                }

                string query = @"
            SELECT 
                u.user_username AS CollaboratorName, 
                c.collab_permission AS Permission,
                c.collab_date AS CollaborationDate
            FROM 
                collaborator c
            JOIN 
                users u ON c.shared_id = u.user_id
            WHERE 
                c.owner_id = @OwnerId AND 
                c.APP_id = @AppId;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerId", currentID);
                        cmd.Parameters.AddWithValue("@AppId", appId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Store the data in ViewState for postback
                        ViewState["CollaboratorData"] = dt;

                        CollaboratorRepeater.DataSource = dt;
                        CollaboratorRepeater.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in collaboratorList: {ex.Message}");
            }
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                // Get the current item's controls
                var item = e.Item;
                var ddlPermission = (DropDownList)item.FindControl("ddlPermission");
                var lblPermission = (Label)item.FindControl("lblPermission");
                var btnEdit = (Button)item.FindControl("btnEdit");
                var btnSave = (Button)item.FindControl("btnSave");
                var btnCancel = (Button)item.FindControl("btnCancel");
                var btnRemove = (Button)item.FindControl("btnRemove");

                switch (e.CommandName)
                {
                    case "Edit":
                        // Set the DropDownList's selected value to match the current permission
                        ddlPermission.SelectedValue = lblPermission.Text;

                        // Toggle visibility
                        lblPermission.Visible = false;
                        ddlPermission.Visible = true;
                        btnEdit.Visible = false;
                        btnSave.Visible = true;
                        btnCancel.Visible = true;
                        btnRemove.Visible = true;
                        break;

                    case "Cancel":
                        // Reset visibility
                        lblPermission.Visible = true;
                        ddlPermission.Visible = false;
                        btnEdit.Visible = true;
                        btnSave.Visible = false;
                        btnCancel.Visible = false;
                        btnRemove.Visible = false;
                        break;

                    case "Save":
                        if (ddlPermission != null)
                        {
                            string newPermission = ddlPermission.SelectedValue;
                            System.Diagnostics.Debug.WriteLine($"newPermission : {newPermission}");
                            string collaboratorName = e.CommandArgument.ToString();

                            // Retrieve the user_id based on the collaboratorName (username)
                            int userId = GetUserIdByUsername(collaboratorName);

                            if (userId >= 0)
                            {
                                // Update the collaborator's permission using the user_id
                                UpdateCollaboratorPermission(userId, newPermission);
                            }
                            else
                            {
                                // Handle the case where the user_id is not found
                                ErrorMessage.Visible = true;
                                ErrorMessage.Text = "User not found.";
                            }
                        }
                        collaboratorList();
                        break;

                    case "Remove":
                        string collabName = e.CommandArgument.ToString();
                        RemoveCollaborator(collabName);
                        collaboratorList();
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Visible = true;
                ErrorMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        private void UpdateCollaboratorPermission(int sharedId, string newPermission)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string query = @"
                UPDATE c
                SET c.collab_permission = @NewPermission
                FROM collaborator c
                JOIN users u ON c.shared_id = u.user_id
                WHERE c.shared_id = @sharedId AND c.owner_id = @OwnerId AND c.APp_id = @AppId;";

            string currentID = Session["UserID"]?.ToString();
            int appId = int.Parse(ddlApplications.SelectedValue);

            if (string.IsNullOrEmpty(currentID) || appId <= 0)
            {
                // Redirect if the session is null or the API ID is missing
                Response.Redirect("~/UI/SignInPage.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NewPermission", newPermission);
                    cmd.Parameters.AddWithValue("@sharedId", sharedId);
                    cmd.Parameters.AddWithValue("@OwnerId", currentID);
                    cmd.Parameters.AddWithValue("@AppId", appId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadFriendsList()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                string currentID = Session["UserID"]?.ToString();
                int appId;

                // Use ViewState value if it's a postback
                string selectedValue = IsPostBack ? ViewState["SelectedAppId"]?.ToString() : ddlApplications.SelectedValue;
                if (!int.TryParse(selectedValue, out appId))
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                    return;
                }

                if (string.IsNullOrEmpty(currentID) || (appId <= 0))
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
                    return;
                }

                string query = @"

                 SELECT 

                    u.user_id,

                    u.user_username

                FROM users u

                INNER JOIN friends c 

                    ON (u.user_id = c.initiator_id AND c.receiver_id = @UserId)

                    OR (u.user_id = c.receiver_id AND c.initiator_id = @UserId)

                WHERE 

                    u.user_visibility = 1 

                    AND c.friend_status = 1

                    AND NOT EXISTS (

                        SELECT 1 

                        FROM collaborator col 

                        WHERE col.shared_id = u.user_id 

                        AND col.APP_id = @AppId);";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentID);
                        cmd.Parameters.AddWithValue("@AppId", appId);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Store the data in ViewState for postback
                        ViewState["FriendsData"] = dt;

                        FriendsRepeater.DataSource = dt;
                        FriendsRepeater.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error logging here
                Console.WriteLine($"Error in LoadFriendsList: {ex.Message}");
            }
        }

        protected void FriendsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddCollaborator")
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                    string currentID = Session["UserID"]?.ToString();
                    int appId;

                    // Use ViewState value
                    string selectedValue = ViewState["SelectedAppId"]?.ToString();
                    if (!int.TryParse(selectedValue, out appId))
                    {
                        ErrorMessage.Visible = true;
                        ErrorMessage.Text = "Invalid application selection.";
                        return;
                    }

                    if (string.IsNullOrEmpty(currentID) || appId <= 0)
                    {
                        Response.Redirect("~/UI/SignInPage.aspx");
                        return;
                    }

                    string friendId = e.CommandArgument.ToString();

                    string query = @"
                    INSERT INTO collaborator (collab_permission, collab_date, APP_id, owner_id, shared_id)
                    VALUES ('Read', GETDATE(), @AppId, @OwnerId, @SharedId);";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@AppId", appId);
                            cmd.Parameters.AddWithValue("@OwnerId", currentID);
                            cmd.Parameters.AddWithValue("@SharedId", friendId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    SuccessMessage.Visible = true;
                    SuccessMessage.Text = "Collaborator added successfully!";

                    // Refresh lists
                    LoadFriendsList();
                    collaboratorList();
                }
                catch (Exception ex)
                {
                    ErrorMessage.Visible = true;
                    ErrorMessage.Text = "An error occurred: " + ex.Message;
                }
            }
        }

        private void RemoveCollaborator(string collaboratorName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string query = @"
        DELETE FROM collaborator
        WHERE shared_id = (SELECT user_id FROM users WHERE user_username = @CollaboratorName)
        AND owner_id = @OwnerId AND APp_id = @AppId;";

            string currentID = Session["UserID"]?.ToString();
            int appId = int.Parse(ddlApplications.SelectedValue);

            if (string.IsNullOrEmpty(currentID) || appId <= 0)
            {
                // Redirect if the session is null or the API ID is missing
                Response.Redirect("~/UI/SignInPage.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CollaboratorName", collaboratorName);
                    cmd.Parameters.AddWithValue("@OwnerId", currentID);
                    cmd.Parameters.AddWithValue("@AppId", appId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            collaboratorList(); // Refresh the Collaborator list to reflect changes
            LoadFriendsList();
        }

        // Dropdown selection changed event
        protected void ddlApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApplications.SelectedValue != null && ddlApplications.SelectedValue != "0")
            {
                ViewState["SelectedAppId"] = ddlApplications.SelectedValue;
                // Check if "Default" is selected
                if (ddlApplications.SelectedValue == "1")
                {
                    // Handle behavior for Default selection
                    lblHeader.Text = "Viewing Default APIs";

                    // Hide the details card for default selection
                    appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";

                    // Clear any previous data in the repeater and hide API results container
                    rptResults.DataSource = null;
                    rptResults.DataBind();
                    apiResultsContainer.Visible = true;

                    // Load APIs by Default (for current user)
                    LoadAPIsByDefault();
                    LoadFriendsList();
                    collaboratorList();
                    lblPerm.Visible = false;
                }
                else
                {
                    int selectedAppId = int.Parse(ddlApplications.SelectedValue);

                    // Load Application details for selected AppId
                    ApplicationDetails appDetails = LoadApplicationDetails(selectedAppId);
                    lblHeader.Text = "Viewing Application: " + ddlApplications.SelectedItem?.Text ?? "Unknown Application";

                    if (appDetails != null)
                    {
                        // Show the details card if not "Default"
                        appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"].Replace("d-none", "").Trim();

                        // Populate the textboxes with the application details
                        txtAppNameEdit.Text = appDetails.Name;
                        txtAppTestingPathEdit.Text = appDetails.TestingPath;
                        txtAppProductionPathEdit.Text = appDetails.ProductionPath;
                        txtAppLanguageEdit.Text = appDetails.Language;

                        // Set controls to view mode
                        SetViewMode();
                    }

                    // Load APIs by AppId for the selected application
                    LoadAPIsByAppId(selectedAppId);
                    apiResultsContainer.Visible = true;
                    LoadFriendsList();
                    collaboratorList();
                    lblPerm.Visible = true;
                }
            }
            else
            {
                // Hide the details card and APIs if no valid selection
                appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";
                rptResults.DataSource = null;
                rptResults.DataBind();
                apiResultsContainer.Visible = false;
            }
        }

        protected void ddlViewmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedViewMode = ddlViewmode.SelectedValue;
            ViewState["SelectedViewMode"] = ddlViewmode.SelectedValue;

            // Clear the ddlApplications dropdown
            ddlApplications.Items.Clear();
            apiResultsContainer.Visible = false;
            CollaborationButton.Visible = false;
            btnDelete.Visible = false;
            appDetailsCard.Attributes["class"] = appDetailsCard.Attributes["class"] + " d-none";
            lblPerm.Visible = false;

            // Handle the default "Choose view mode" option
            if (string.IsNullOrEmpty(selectedViewMode))
            {
                // Optionally, display a message or perform other actions
                lblHeader.Text = "Please select a view mode.";
                return;
            }

            // Load applications based on the selected view mode
            switch (selectedViewMode)
            {
                case "Default":
                    ddlApplications.Items.Clear(); 
                    break;
                case "All":
                    LoadAllApplications();
                    break;
                case "My":
                    LoadMyApplications();
                    break;
                case "Shared":
                    LoadSharedApplications();
                    break;
            }
        }

        private void LoadAllApplications()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = "SELECT app_id, app_name FROM application";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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

            // Add the default options after binding
            ddlApplications.Items.Insert(0, new ListItem("-- Select Application --", "0"));
            if (!IsPostBack)
            {
                ListItem emptyItem = new ListItem("", "");
                emptyItem.Attributes["class"] = "hidden-option"; // Add the CSS class
                ddlApplications.Items.Insert(1, emptyItem);
            }

            lblHeader.Text = "Please select the application";
        }

        private void LoadMyApplications()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = "SELECT app_id, app_name FROM application WHERE user_id = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlApplications.DataSource = reader;
                    ddlApplications.DataTextField = "app_name";
                    ddlApplications.DataValueField = "app_id";
                    ddlApplications.DataBind();
                }
            }

            // Add the default options after binding
            ddlApplications.Items.Insert(0, new ListItem("-- Select Application --", "0"));
            ddlApplications.Items.Insert(1, new ListItem("Default", "1"));

            lblHeader.Text = "Please select the application";
        }

        private void LoadSharedApplications()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
            string query = @"
        SELECT a.app_id, a.app_name 
        FROM application a
        INNER JOIN collaborator c ON a.app_id = c.app_id
        WHERE c.shared_id = @UserId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlApplications.DataSource = reader;
                    ddlApplications.DataTextField = "app_name";
                    ddlApplications.DataValueField = "app_id";
                    ddlApplications.DataBind();
                }
            }

            // Add the default option at index 0
            ddlApplications.Items.Insert(0, new ListItem("-- Select Application --", "0"));

            if (!IsPostBack)
            {
                ListItem emptyItem = new ListItem("", "");
                emptyItem.Attributes["class"] = "hidden-option"; // Add the CSS class
                ddlApplications.Items.Insert(1, emptyItem);
            }

            lblHeader.Text = "Please select the application";
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
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            // Check if the app_name already exists
            bool appNameExists = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM application WHERE app_name = @AppName";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@AppName", appName);
                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();
                    appNameExists = count > 0;
                }
            }

            if (appNameExists)
            {
                // Display an error message to the user
                lblMessageApp.Text = "Application name already exists. Please choose a different name.";
                lblMessageApp.CssClass = "text-danger"; // Optional: Add a CSS class for styling the error message
                return; // Exit the method without inserting the record
            }

            // Logic to insert data into the "application" table
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO application (app_name, app_testing_path, app_production_path, app_language, user_id) " +
                                     "VALUES (@AppName, @TestingPath, @ProductionPath, @Language, @UserId)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@AppName", appName);
                    cmd.Parameters.AddWithValue("@TestingPath", string.IsNullOrEmpty(testingPath) ? DBNull.Value : (object)testingPath);
                    cmd.Parameters.AddWithValue("@ProductionPath", string.IsNullOrEmpty(productionPath) ? DBNull.Value : (object)productionPath);
                    cmd.Parameters.AddWithValue("@Language", string.IsNullOrEmpty(language) ? DBNull.Value : (object)language);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    // Open connection and execute the query
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Optionally, reload the page or update the UI to reflect the changes
            Response.Redirect(Request.RawUrl);
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
            CheckEditButtonVisibility(appId);
            return details;
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
                // Store the current selected ID
                string currentAppId = ddlApplications.SelectedValue;

                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE application 
                           SET app_name = @AppName,
                               app_testing_path = @TestingPath,
                               app_production_path = @ProductionPath,
                               app_language = @Language
                           WHERE app_id = @AppId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppId", int.Parse(currentAppId));
                        cmd.Parameters.AddWithValue("@AppName",
                            string.IsNullOrEmpty(txtAppNameEdit.Text) ? (object)DBNull.Value : txtAppNameEdit.Text);
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
                //LoadApplications();
                // Re-select the saved app ID
                ddlApplications.SelectedValue = currentAppId;
                lblHeader.Text = "Viewing Application: " + ddlApplications.SelectedItem?.Text ?? "Unknown Application";

                // Reset to view mode
                txtAppNameEdit.ReadOnly = true;
                txtAppTestingPathEdit.ReadOnly = true;
                txtAppProductionPathEdit.ReadOnly = true;
                txtAppLanguageEdit.ReadOnly = true;

                btnEdit.Visible = true;
                btnSave.Visible = false;
                btnCancel.Visible = false;
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

            //int appId = int.Parse(ddlApplications.SelectedValue);
            //CheckEditButtonVisibility(appId);
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }

        // Helper method to set edit mode
        private void SetEditMode()
        {
            txtAppNameEdit.ReadOnly = false; // Keep name readonly
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
                        am.API_update_date,
                        am.API_endpoint, 
                        am.API_HTTP_method
                    FROM 
                        api_methods am
                    LEFT JOIN 
                        api_header ah ON am.code_id = ah.code_id
                    LEFT JOIN 
                        file_details fd ON am.file_id = fd.file_id
                    WHERE 
                        am.app_id = @AppId;";

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

        private void LoadAPIsByDefault()
        {
            // Get the current logged-in user's ID from session or authentication context
            int currentUserId = Convert.ToInt32(Session["UserId"]); // Example: User ID from session

            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // SQL query to select APIs based on the logged-in user
            string query = @"
                SELECT
                    am.API_id, 
                    am.API_name, 
                    am.API_desc,
                    am.API_update_date,
                    am.API_endpoint, 
                    am.API_HTTP_method
                FROM 
                    api_methods am
                LEFT JOIN 
                    api_header ah ON am.code_id = ah.code_id  
                LEFT JOIN 
                    file_details fd ON am.file_id = fd.file_id  
                WHERE 
                    (fd.user_id = @UserId OR ah.user_id = @UserId)
                    AND am.app_id IS NULL;";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add the current user's ID as a parameter to the query
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the result set to the repeater control
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
                                am.API_update_date,
                                ah.code_uploadDate, 
                                am.API_endpoint, 
                                am.API_HTTP_method
                            FROM 
                                api_methods am
                            JOIN 
                                api_header ah ON am.code_id = ah.code_id
                            JOIN 
                                file_details fd ON am.file_id = fd.file_id
                            WHERE 
                                (fd.user_id = @UserId OR ah.user_id = @UserId)
                                AND am.app_id IS NULL;
";

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


    }
}