using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace API_DAMs.UI
{
    public partial class CollaboratorPage : System.Web.UI.Page
    {
        //private HashSet<int> processedUserIds = new HashSet<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected string GetUserProfileImage(string username)
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
                        return imageResult.ToString(); // Assuming the database contains relative image paths
                    }
                    else
                    {
                        return "~/icon/default-profile.png"; // Default profile image
                    }
                }
            }
        }

        private void LoadUsers()
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    u.user_id, 
                    u.user_username, 
                    u.user_email, 
                    u.user_joined_date, 
                    COALESCE(c.friend_id, 0) AS friend_id, 
                    COALESCE(c.initiator_id, 0) AS initiator_id, 
                    COALESCE(c.receiver_id, 0) AS receiver_id, 
                    COALESCE(c.friend_req, 0) AS friend_req,
                    COALESCE(c.friend_status, 0) AS friend_status
                FROM users u
                LEFT JOIN friends c 
                    ON ((c.initiator_id = @CurrentUserID AND c.receiver_id = u.user_id)
                    OR (c.receiver_id = @CurrentUserID AND c.initiator_id = u.user_id))
                WHERE u.user_id != @CurrentUserID AND u.user_visibility = 1;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CurrentUserID", currentUserId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        rptResults.DataSource = dt;
                        rptResults.DataBind();
                    }
                    else
                    {
                        rptResults.DataSource = null;
                        rptResults.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception
                System.Diagnostics.Debug.WriteLine($"Error in LoadUsers: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Optionally show an error message to the user
                // You might want to create a label on your page to display error messages
                // errorLabel.Text = "An error occurred while loading users.";
            }
        }

        protected void rptResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                try
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;

                    int friendReq = Convert.ToInt32(row["friend_req"]);
                    int friendStatus = Convert.ToInt32(row["friend_status"]);
                    int user1Id = Convert.ToInt32(row["initiator_id"]);
                    int user2Id = Convert.ToInt32(row["receiver_id"]);
                    int userId = Convert.ToInt32(row["user_id"]);
                    int currentUserId = Convert.ToInt32(Session["UserID"]);
                    int collabId = Convert.ToInt32(row["friend_id"]);

                    PlaceHolder phFriendRequestActions = (PlaceHolder)e.Item.FindControl("phFriendRequestActions");
                    phFriendRequestActions.Controls.Clear(); // Ensure clean slate

                    // Case: Already Friends
                    if (friendStatus == 1)
                    {
                        Button btnRemoveFriend = new Button
                        {
                            ID = $"btnRemoveFriend_{collabId}",
                            CommandName = "RemoveFriend",
                            CommandArgument = collabId.ToString(),
                            CssClass = "btn btn-danger btn-sm",
                            Text = "Remove Friend"
                        };
                        btnRemoveFriend.Command += new CommandEventHandler(rptResults_ItemCommand);
                        phFriendRequestActions.Controls.Add(btnRemoveFriend);
                    }
                    // Case: Received Friend Request (waiting for approval)
                    else if (friendReq == 1 && friendStatus == 0 && currentUserId == user2Id)
                    {
                        Button btnApprove = new Button
                        {
                            ID = $"btnApprove_{collabId}",
                            CommandName = "Approve",
                            CommandArgument = collabId.ToString(),
                            CssClass = "btn btn-success btn-sm",
                            Text = "Approve"
                        };
                        btnApprove.Command += new CommandEventHandler(rptResults_ItemCommand);
                        phFriendRequestActions.Controls.Add(btnApprove);

                        Button btnReject = new Button
                        {
                            ID = $"btnReject_{collabId}",
                            CommandName = "Reject",
                            CommandArgument = collabId.ToString(),
                            CssClass = "btn btn-danger btn-sm",
                            Text = "Reject"
                        };
                        btnReject.Command += new CommandEventHandler(rptResults_ItemCommand);
                        phFriendRequestActions.Controls.Add(btnReject);
                    }
                    // Case: Friend request sent by current user (pending approval)
                    else if (friendReq == 1 && friendStatus == 0 && currentUserId == user1Id)
                    {
                        // Display a disabled "Pending" button
                        Button btnPending = new Button
                        {
                            ID = $"btnPending_{collabId}",
                            Enabled = false, // Disabled button
                            CssClass = "btn btn-secondary btn-sm",
                            Text = "Pending"
                        };
                        phFriendRequestActions.Controls.Add(btnPending);
                    }
                    // Case: No existing relationship - can send a friend request
                    else if (friendReq == 0 && friendStatus == 0)
                    {
                        Button btnAddFriend = new Button
                        {
                            ID = $"btnAddFriend_{userId}",
                            CommandName = "AddFriend",
                            CommandArgument = userId.ToString(),
                            CssClass = "btn btn-success btn-sm",
                            Text = "Add Friend"
                        };
                        btnAddFriend.Command += new CommandEventHandler(rptResults_ItemCommand);
                        phFriendRequestActions.Controls.Add(btnAddFriend);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in ItemDataBound: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }
        }

        protected void rptResults_ItemCommand(object source, CommandEventArgs e)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);
                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

                System.Diagnostics.Debug.WriteLine($"CommandName: {e.CommandName}, CommandArgument: {e.CommandArgument}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    switch (e.CommandName)
                    {
                        case "Approve":
                            cmd.CommandText = @"
                        UPDATE friends
                        SET friend_status = 1, friend_req = 0
                        WHERE friend_id = @CollabId AND receiver_id = @CurrentUserId";
                            cmd.Parameters.AddWithValue("@CollabId", e.CommandArgument);
                            cmd.Parameters.AddWithValue("@CurrentUserId", currentUserId);
                            break;

                        case "Reject":
                            cmd.CommandText = @"
                        DELETE FROM friends
                        WHERE friend_id = @CollabId AND receiver_id = @CurrentUserId";
                            cmd.Parameters.AddWithValue("@CollabId", e.CommandArgument);
                            cmd.Parameters.AddWithValue("@CurrentUserId", currentUserId);
                            break;

                        case "AddFriend":
                            cmd.CommandText = @"
                        INSERT INTO friends (friend_date, initiator_id, receiver_id, friend_req, friend_status)
                        VALUES (GETDATE(), @User1Id, @User2Id, 1, 0)";
                            cmd.Parameters.AddWithValue("@User1Id", currentUserId);
                            cmd.Parameters.AddWithValue("@User2Id", e.CommandArgument);
                            break;

                        case "RemoveFriend":
                            cmd.CommandText = @"
                        DELETE FROM friends
                        WHERE friend_id = @CollabId";
                            cmd.Parameters.AddWithValue("@CollabId", e.CommandArgument);
                            break;
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine($"{rowsAffected} rows affected.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ItemCommand: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                // Always rebind to refresh the UI
                LoadUsers();
            }
        }



        //private bool IsFriendRequestPending(int user1Id, int user2Id)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //    SELECT COUNT(*) FROM [dbo].[friends]
        //    WHERE ((initiator_id = @User1Id AND receiver_id = @User2Id) OR (initiator_id = @User2Id AND receiver_id = @User1Id))
        //    AND friend_req = 1 AND friend_status = 0";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@User1Id", user1Id);
        //            command.Parameters.AddWithValue("@User2Id", user2Id);
        //            connection.Open();
        //            return (int)command.ExecuteScalar() > 0;
        //        }
        //    }
        //}

        //private bool AreUsersAlreadyFriends(int user1Id, int user2Id)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //    SELECT COUNT(*) FROM [dbo].[friends]
        //    WHERE ((initiator_id = @User1Id AND receiver_id = @User2Id) OR (initiator_id = @User2Id AND receiver_id = @User1Id))
        //    AND friend_status = 1";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@User1Id", user1Id);
        //            command.Parameters.AddWithValue("@User2Id", user2Id);
        //            connection.Open();
        //            return (int)command.ExecuteScalar() > 0;
        //        }
        //    }
        //}

        //private void SendFriendRequest(int senderId, int receiverId)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //INSERT INTO [dbo].[friends] (initiator_id, receiver_id, friend_date, friend_req, friend_status)
        //VALUES (@SenderId, @ReceiverId, GETDATE(), 1, 0)";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@SenderId", senderId);   // Always the sender
        //            command.Parameters.AddWithValue("@ReceiverId", receiverId); // Always the receiver
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //private void ApproveFriendRequest(int senderUserId, int receiverUserId)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //    UPDATE [dbo].[friends]
        //    SET friend_status = 1, friend_req = 0, friend_date = GETDATE()
        //    WHERE initiator_id = @SenderId AND receiver_id = @ReceiverId AND friend_req = 1";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@SenderId", senderUserId);
        //            command.Parameters.AddWithValue("@ReceiverId", receiverUserId);
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //private void RejectFriendRequest(int senderUserId, int receiverUserId)
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //    DELETE FROM [dbo].[friends]
        //    WHERE initiator_id = @SenderId AND receiver_id = @ReceiverId AND friend_req = 1";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@SenderId", senderUserId);
        //            command.Parameters.AddWithValue("@ReceiverId", receiverUserId);
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

    }
}