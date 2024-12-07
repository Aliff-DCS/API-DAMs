using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class ProfilePage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Existing login check
            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    litUsername.Text = Session["User"].ToString();
                }
                else
                {
                    Response.Redirect("~/UI/SignInPage.aspx");
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
            Response.Redirect("ProfileSettings.aspx"); // Replace with your actual profile settings page URL
        }
    }
}