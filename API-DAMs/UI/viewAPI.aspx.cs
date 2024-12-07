using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace API_DAMs.UI
{
    public partial class viewAPI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAllAPIs();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string apiName = "";//txtMethodName.Text.Trim();

            // Store the search term in ViewState for sorting
            ViewState["SearchTerm"] = apiName;

            if (!string.IsNullOrEmpty(apiName))
            {
                LoadAPIs(apiName);  // Perform the search
            }
            else
            {
                LoadAllAPIs();  // Load all APIs if no search term
            }
        }

        private void LoadAPIs(string apiName, string sortExpression = "code_uploadDate DESC")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            // Ensure only safe sorting columns are allowed
            string safeSortExpression = GetSafeSortExpression(sortExpression);

            string query = $@"
                SELECT 
                    am.API_id, 
                    am.API_name, 
                    am.API_desc, 
                    ah.code_uploadDate, 
                    am.API_endpoint, 
                    am.API_HTTP_method
                FROM 
                    api_methods am
                JOIN 
                    api_header ah ON am.code_id = ah.code_id
                WHERE 
                    am.API_name LIKE @APIName 
                ORDER BY {safeSortExpression};";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@APIName", "%" + apiName + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptResults.DataSource = dt;
                    rptResults.DataBind();
                }
            }
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Redirect")
            {
                string apiId = e.CommandArgument.ToString();
                Response.Redirect($"ViewDetailPage.aspx?apiId={apiId}");
            }
            else if (e.CommandName == "Sort")
            {
                // Extract sorting field and direction
                string sortExpression = e.CommandArgument.ToString();

                // Retrieve the search term from ViewState (if exists)
                string searchTerm = ViewState["SearchTerm"] as string;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    // If there is a search term, load the APIs based on the search with sorting
                    LoadAPIs(searchTerm, sortExpression);
                }
                else
                {
                    // If no search term, just load all APIs with sorting
                    LoadAllAPIs(sortExpression);
                }
            }
        }

        private void LoadAllAPIs(string sortExpression = "code_uploadDate DESC")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string safeSortExpression = GetSafeSortExpression(sortExpression);

            string query = $@"
                SELECT 
                    am.API_id, 
                    am.API_name, 
                    am.API_desc, 
                    ah.code_uploadDate, 
                    am.API_endpoint, 
                    am.API_HTTP_method
                FROM 
                    api_methods am
                JOIN 
                    api_header ah ON am.code_id = ah.code_id
                ORDER BY {safeSortExpression};";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptResults.DataSource = dt;
                    rptResults.DataBind();
                }
            }
        }

        private string GetSafeSortExpression(string sortExpression)
        {
            // Whitelist valid sorting columns
            string[] validColumns = { "code_uploadDate", "API_name", "API_endpoint" };
            string[] parts = sortExpression.Split(' ');

            if (validColumns.Contains(parts[0], StringComparer.OrdinalIgnoreCase) &&
                (parts.Length == 1 || parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("ASC", StringComparison.OrdinalIgnoreCase)))
            {
                return sortExpression;
            }

            // Default to safe sorting
            return "code_uploadDate DESC";
        }

    }
}