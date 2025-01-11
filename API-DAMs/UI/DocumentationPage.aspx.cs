using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using System.Diagnostics;

namespace API_DAMs.UI
{
    public partial class DocumentationPage : System.Web.UI.Page
    {
        private const string DeletedPanelsKey = "DeletedPanels";

        [Serializable]
        public class ParameterControlData
        {
            public string ContainerID { get; set; } // Change this to string to store the Panel's ID
            public int ParamIndex { get; set; }
            public int MethodIndex { get; set; }
            public List<string> ParameterDataTypes { get; set; }
            public List<string> ParameterNames { get; set; }
        }


        private List<ParameterControlData> globalParameterControlList
        {
            get
            {
                // Ensure we are getting the list from the session state
                if (Session["GlobalParameterControlList"] is List<ParameterControlData> list)
                {
                    return list;
                }
                else
                {
                    return new List<ParameterControlData>();
                }
            }
            set
            {
                Session["GlobalParameterControlList"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is logged in
            if (Session["User"] != null)
            {
                string username = Session["User"].ToString();

                // Retrieve the user ID based on the username
                int userId = GetUserId(username);

                if (userId > 0)
                {
                    // Store the user ID in the session for later use
                    Session["UserId"] = userId;
                }
                else
                {
                    // If user ID is not found, redirect to the login page
                    Response.Redirect("~/UI/SignInPage.aspx");
                }
            }
            else
            {
                // Redirect to login page if the user is not logged in
                Response.Redirect("~/UI/SignInPage.aspx");
            }

            // Existing Page_Load logic
            if (!IsPostBack)
            {
                code_doc.Visible = false;
                btnSubmit.Visible = false;
            }
            else
            {
                DynamicCodeText();
                RecreateControls();
                System.Diagnostics.Debug.WriteLine($"List count on postback: {globalParameterControlList.Count}");
            }
        }

        private int GetUserId(string username)
        {
            int userId = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

            string query = "SELECT user_id FROM users WHERE user_username = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }
            }

            return userId;
        }

        private void RecreateControls()
        {
            if (Session["GlobalParameterControlList"] is List<ParameterControlData> savedList)
            {
                globalParameterControlList = savedList;
                foreach (var controlData in globalParameterControlList)
                {
                    Control containerControl = FindControlRecursive(this, controlData.ContainerID);
                    if (containerControl is Panel containerPanel)
                    {
                        AddParameterControls(containerPanel, controlData.ParamIndex, controlData.MethodIndex, controlData.ParameterDataTypes, controlData.ParameterNames);
                    }
                }
            }
        }

        public Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control child in root.Controls)
            {
                Control found = FindControlRecursive(child, id);
                if (found != null)
                    return found;
            }

            return null;
        }

        private void ExtractParameterValues()
        {
            foreach (var controlData in globalParameterControlList)
            {
                // Find the container panel using the stored ContainerID
                Panel containerPanel = FindControlRecursive(this, controlData.ContainerID) as Panel;
                if (containerPanel != null)
                {
                    // Clear the current lists to prepare for new values
                    controlData.ParameterDataTypes.Clear();
                    controlData.ParameterNames.Clear();

                    // Loop through the parameters based on the ParamIndex
                    for (int i = 0; i <= controlData.ParamIndex; i++)
                    {
                        // Find the TextBox for Parameter Type
                        TextBox parameterTypeBox = containerPanel.FindControl($"parameter_type_{i}_{controlData.MethodIndex}") as TextBox;
                        if (parameterTypeBox != null)
                        {
                            controlData.ParameterDataTypes.Add(parameterTypeBox.Text);
                            System.Diagnostics.Debug.WriteLine($"Added Parameter Type: {parameterTypeBox.Text} for ParamIndex: {i}, MethodIndex: {controlData.MethodIndex}");
                        }

                        // Find the TextBox for Parameter Name
                        TextBox parameterNameBox = containerPanel.FindControl($"parameter_name_{i}_{controlData.MethodIndex}") as TextBox;
                        if (parameterNameBox != null)
                        {
                            controlData.ParameterNames.Add(parameterNameBox.Text);
                            System.Diagnostics.Debug.WriteLine($"Added Parameter Name: {parameterNameBox.Text} for ParamIndex: {i}, MethodIndex: {controlData.MethodIndex}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Container panel with ID {controlData.ContainerID} not found.");
                }
            }

            // Update the globalParameterControlList with the extracted values
            Session["GlobalParameterControlList"] = globalParameterControlList;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ExtractParameterValues();
            //System.Diagnostics.Debug.WriteLine($"List count on postback: {globalParameterControlList.Count}");
        }

        private void DynamicCodeText()
        {
            var deletedPanels = GetDeletedPanels();
            //ViewState.Clear();
            bool shouldShowAlert = false;
            List<int> methodIndices = new List<int>();

            if (!string.IsNullOrWhiteSpace(SC_text.Text))
            {
                var codeText = SC_text.Text;
                var (methodNames, jsonKeys) = ExtractMethodNamesAndJsonKeys(codeText);
                var parameterCounts = ExtractParameterCounts(codeText);
                var returnTypes = ExtractReturnTypes(codeText);
                var parameterDataTypes = ExtractParameterDataTypes(codeText);
                var parameterNames = ExtractParameterNames(codeText);

                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

                // Debug output
                System.Diagnostics.Debug.WriteLine($"MethodNames Count: {methodNames.Count}");
                System.Diagnostics.Debug.WriteLine($"MethodNames names: {string.Join(", ", methodNames)}");
                System.Diagnostics.Debug.WriteLine($"ParameterCounts Count: {parameterCounts.Count}");
                System.Diagnostics.Debug.WriteLine($"ReturnTypes Count: {returnTypes.Count}");
                System.Diagnostics.Debug.WriteLine($"ParameterDataTypes Count: {parameterDataTypes.Count}");
                System.Diagnostics.Debug.WriteLine($"ParameterNames Count: {parameterNames.Count}");

                int parameterIndex = 0;
                bool panelCreated = false;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < methodNames.Count; i++)
                    {
                        string methodName = methodNames[i];
                        int parameterCount = parameterCounts[i];
                        string returnType = returnTypes[i];

                        if (parameterCount >= 0 && !deletedPanels.Contains(i.ToString()))
                        {
                            // Retrieve user ID from the session
                            if (Session["UserId"] == null)
                            {
                                // Redirect to login page if the session is invalid
                                Response.Redirect("~/UI/SignInPage.aspx");
                                return;
                            }

                            int userId = Convert.ToInt32(Session["UserId"]);
                            // Check if method name already exists in the database for the current user
                            string checkApiQuery = @"
                                SELECT COUNT(*)
                                FROM api_methods am
                                JOIN api_header ah ON am.code_id = ah.code_id
                                WHERE am.API_Name = @API_Name AND ah.user_id = @UserId";

                            using (SqlCommand checkApiCommand = new SqlCommand(checkApiQuery, connection))
                            {
                                checkApiCommand.Parameters.AddWithValue("@API_Name", methodName);
                                checkApiCommand.Parameters.AddWithValue("@UserId", userId); // Include user ID in the query

                                int count = (int)checkApiCommand.ExecuteScalar();

                                if (count > 0)
                                {
                                    // Show confirmation popup if method already exists (but don't break the loop)
                                    string script = $"confirm('The API method already exists for the current user: {methodName}. Do you want to proceed?');";
                                    ScriptManager.RegisterStartupScript(this, GetType(), "confirmMessage", script, true);
                                }
                            }


                            panelCreated = true;
                            methodIndices.Add(i);

                            var methodParameterDataTypes = parameterDataTypes.Skip(parameterIndex).Take(parameterCount).ToList();
                            var methodParameterNames = parameterNames.Skip(parameterIndex).Take(parameterCount).ToList();

                            List<string> jsonKeyList = jsonKeys.ContainsKey(methodName) ? jsonKeys[methodName] : new List<string>();

                            System.Diagnostics.Debug.WriteLine($"\nMethod {methodName}'s JSON Keys: {string.Join(", ", jsonKeyList)}");

                            Panel newPanel = CreateCodeDocPanel(i, methodName, parameterCount, returnType, methodParameterDataTypes, methodParameterNames, jsonKeys);
                            code_doc.Controls.Add(newPanel);

                            parameterIndex += parameterCount;
                        }
                    }
                }
                if (code_doc.Controls.OfType<Panel>().Count() == 0)
                {
                    panelCreated = false;
                }

                code_doc.Visible = panelCreated;
                btnSubmit.Visible = panelCreated;

                if (!panelCreated)
                {
                    shouldShowAlert = true;
                }
            }
            else
            {
                shouldShowAlert = true;
                code_doc.Visible = false;
                btnSubmit.Visible = false;
            }

            if (shouldShowAlert)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", "alert('Invalid code format. No valid methods found.');", true);
            }
        }

        protected void handleCode(object sender, EventArgs e)
        {
            // Clear only Panel type controls within code_doc
            ClearPanels(code_doc);
            ResetControlValues();

            ViewState[DeletedPanelsKey] = new List<string>();
            Page_Load(sender, e); // Trigger Page_Load to process data
        }

        private void ClearPanels(Control parent)
        {
            // Create a list to hold panels to be removed
            var panelsToRemove = new List<Panel>();

            // Iterate through controls and find panels
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel)
                {
                    // Add panel to the removal list
                    panelsToRemove.Add(panel);
                }
                else if (control.HasControls())
                {
                    // Recursively check for panels in nested controls
                    ClearPanels(control);
                }
            }

            // Remove collected panels
            foreach (var panel in panelsToRemove)
            {
                parent.Controls.Remove(panel);
            }
        }

        private void ResetControlValues()
        {
            // Reset TextBox for Description
            TextBox descriptionBox = (TextBox)code_doc.FindControl("Description");
            if (descriptionBox != null)
            {
                descriptionBox.Text = string.Empty;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Description TextBox not found.");
            }

            // Reset DropDownLists for Platform and Language
            DropDownList platformDropDown = (DropDownList)code_doc.FindControl("Platform");
            if (platformDropDown != null)
            {
                platformDropDown.SelectedIndex = -1; // Reset to no selection
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Platform DropDownList not found.");
            }

            DropDownList languageDropDown = (DropDownList)code_doc.FindControl("Language");
            if (languageDropDown != null)
            {
                languageDropDown.SelectedIndex = -1; // Reset to no selection
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Language DropDownList not found.");
            }
        }

        private Panel CreateCodeDocPanel(int index, string methodName, int parameterCount, string returnType, List<string> parameterDataTypes, List<string> parameterNames, Dictionary<string, List<string>> jsonKeys, string httpMethod = null)
        {
            System.Diagnostics.Debug.WriteLine($"Creating panel for method {index}: {methodName}");
            System.Diagnostics.Debug.WriteLine($"Parameter Count: {parameterCount}");
            System.Diagnostics.Debug.WriteLine($"Parameter Types: {string.Join(", ", parameterDataTypes)}");
            System.Diagnostics.Debug.WriteLine($"Parameter Names: {string.Join(", ", parameterNames)}");

            Panel panel = new Panel // Ensure unique ID, Bootstrap card class for uniform styling
            { ID = "code_doc_" + index, CssClass = "card mb-4" };
            System.Diagnostics.Debug.WriteLine("code_doc_" + index);

            // Function details header
            panel.Controls.Add(new Literal { Text = $"<div class='card-header text-center border'><h4 class='card-title text-light'><span class='text-primary'>{methodName}</span> Method Details</h4></div>" });

            // Card body
            Panel cardBody = new Panel { CssClass = "card-body border" };

            // Method name
            cardBody.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='method_name_{index}'>Method Name:</label>" });
            TextBox methodNameBox = new TextBox { ID = "method_name_" + index, CssClass = "form-control", Text = methodName };
            cardBody.Controls.Add(methodNameBox);
            cardBody.Controls.Add(new Literal { Text = "</div>" });

            // API Endpoint
            cardBody.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='api_endpoint_{index}'>API Endpoint:</label>" });
            TextBox apiEndpointBox = new TextBox { ID = "api_endpoint_" + index, CssClass = "form-control", Text = string.Empty };
            cardBody.Controls.Add(apiEndpointBox);
            cardBody.Controls.Add(new Literal { Text = "</div>" });

            // HTTP Method (DropDownList)
            var httpMethodLabel = new Literal { Text = $"<div class='form-group flx'><label for='http_method_{index}'>HTTP Method:</label>" };
            cardBody.Controls.Add(httpMethodLabel);
            DropDownList httpMethodDropDown = new DropDownList { ID = "http_method_" + index, CssClass = "form-control", AutoPostBack = true };
            httpMethodDropDown.SelectedIndexChanged += new EventHandler(HttpMethodDropDown_SelectedIndexChanged);
            var httpMethods = new List<string> { "", "GET", "POST", "PUT", "DELETE", "PATCH" };
            foreach (var method in httpMethods) httpMethodDropDown.Items.Add(new ListItem(string.IsNullOrEmpty(method) ? "None" : method, method));
            if (httpMethods.Contains(httpMethod)) httpMethodDropDown.SelectedValue = httpMethod;
            cardBody.Controls.Add(httpMethodDropDown);

            // Return type (DropDownList)
            cardBody.Controls.Add(new Literal { Text = $"<label for='return_type_{index}' class='mg-left' >Return Type:</label>" });
            DropDownList returnTypeDropDown = new DropDownList { ID = "return_type_" + index, CssClass = "form-control" };
            var returnTypes = new List<string> { "void", "int", "float", "string", "bool" };
            foreach (var type in returnTypes) returnTypeDropDown.Items.Add(new ListItem(type, type));
            if (returnTypes.Contains(returnType)) returnTypeDropDown.SelectedValue = returnType;
            cardBody.Controls.Add(returnTypeDropDown);
            cardBody.Controls.Add(new Literal { Text = "</div>" });


            // POST Type Dropdown (URL/JSON)
            Panel postTypePanel = new Panel { ID = $"post_type_panel_{index}" };
            postTypePanel.Visible = false;

            var paramTypeLabel = new Literal { Text = $"<div class='form-group flex'><label for='param_type_{index}'>POST Type (URL/JSON):</label>" };
            postTypePanel.Controls.Add(paramTypeLabel);

            DropDownList paramTypeDropDown = new DropDownList
            {
                ID = "param_type_" + index,
                CssClass = "form-control",
                AutoPostBack = true
            };

            // Add event handler for selection change
            paramTypeDropDown.SelectedIndexChanged += new EventHandler(ParamTypeDropDown_SelectedIndexChanged);

            var paramTypes = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("NONE", "NONE"),
                new Tuple<string, string>("URL PARAMETER", "url_param"),
                new Tuple<string, string>("JSON STRING", "json_string")
            };

            foreach (var type in paramTypes)
            {
                paramTypeDropDown.Items.Add(new ListItem(type.Item1, type.Item2));
            }

            postTypePanel.Controls.Add(paramTypeDropDown);
            postTypePanel.Controls.Add(new Literal { Text = "</div>" });

            cardBody.Controls.Add(postTypePanel);

            //JSON PArt
            if (jsonKeys.TryGetValue(methodName, out var methodJsonKeys))
            {
                // Wrap the JSON keys section in a Panel
                Panel jsonKeysPanel = new Panel { ID = $"json_keys_panel_{index}", CssClass = "json-keys-panel" };
                jsonKeysPanel.Visible = false; // Initially set visibility

                // Create a single label and textbox for all JSON keys
                var jsonKeyString = string.Join(", ", methodJsonKeys); // Join all JSON keys with commas

                // Add the label
                jsonKeysPanel.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='json_keys_{index}'>JSON Keys:</label>" });

                // Add the textbox with all JSON keys as the text
                TextBox jsonKeyTextBox = new TextBox
                {
                    ID = $"json_keys_{index}",
                    CssClass = "form-control",
                    Text = jsonKeyString
                };
                jsonKeysPanel.Controls.Add(jsonKeyTextBox);

                // Close the div
                jsonKeysPanel.Controls.Add(new Literal { Text = "</div>" });

                // Add the JSON keys panel to the parameter content panel
                cardBody.Controls.Add(jsonKeysPanel);
            }
            else
            {
                // Wrap the JSON keys section in a Panel
                Panel jsonKeysPanel = new Panel { ID = $"json_keys_panel_{index}", CssClass = "json-keys-panel" };
                jsonKeysPanel.Visible = false; // Initially set visibility

                // Add the label
                jsonKeysPanel.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='json_keys_{index}'>JSON Keys:</label>" });

                // Add the textbox with all JSON keys as the text
                TextBox jsonKeyTextBox = new TextBox
                {
                    ID = $"json_keys_{index}",
                    CssClass = "form-control",
                    Text = "No JSON key found, please choose another method"
                };
                jsonKeysPanel.Controls.Add(jsonKeyTextBox);

                // Close the div
                jsonKeysPanel.Controls.Add(new Literal { Text = "</div>" });

                // Add the JSON keys panel to the parameter content panel
                cardBody.Controls.Add(jsonKeysPanel);
            }

            // Create the main panel for the parameter details with a collapse trigger
            Panel parameterDetailsPanel = new Panel { ID = "parameterDetailsPanel_" + index };
            parameterDetailsPanel.Visible = parameterCount > 0;

            // Add the heading with the collapse toggle
            parameterDetailsPanel.Controls.Add(new Literal
            {
                Text = "<h5 class='text-center mb-3 d-flex justify-content-center align-items-center collapse-heading color-para' "
                     + "data-bs-toggle='collapse' href='#ContentPlaceHolder1_parameterContent_" + index + "' role='button' aria-expanded='false' aria-controls='parameterContent_" + index + "'>"
                     + "<span class='text-light'>Parameter Details</span>"
                     + "<span class='ms-auto chevron-icon'><i class='bi bi-chevron-down'></i></span>"
                     + "</h5>"
            });


            Panel parameterContentPanel = new Panel
            {
                ID = "parameterContent_" + index,
                CssClass = "collapse border pd-10 white"
            };

            // Add a hidden field to store the current parameter count
            HiddenField parameterCountField = new HiddenField
            {
                ID = $"parameterCount_{index}",
                Value = parameterCount.ToString()
            };
            parameterContentPanel.Controls.Add(parameterCountField);

            // Parameter count display
            parameterContentPanel.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='parameters_{index}'>Parameter Count:</label>" });
            TextBox parametersBox = new TextBox { ID = "parameters_" + index, CssClass = "form-control", Text = parameterCount.ToString(), ReadOnly = true };
            parameterContentPanel.Controls.Add(parametersBox);
            parameterContentPanel.Controls.Add(new Literal { Text = "</div>" });

            // Create a container for parameter controls
            Panel parameterControlsContainer = new Panel { ID = $"parameterControlsContainer_{index}" };
            parameterContentPanel.Controls.Add(parameterControlsContainer);


            // Add initial parameter controls
            for (int i = 0; i < parameterCount; i++)
            {
                ParameterControlData parameterData = new ParameterControlData
                {
                    ContainerID = parameterControlsContainer.ID,
                    ParamIndex = i,
                    MethodIndex = index,
                    ParameterDataTypes = parameterDataTypes,  // Make sure these lists are initialized
                    ParameterNames = parameterNames  // Make sure these lists are initialized
                };

                // Get the current list from session state
                var list = globalParameterControlList;

                // Check if the ParameterControlData object already exists in the list
                bool alreadyExists = list.Any(p =>
                            p.ContainerID == parameterData.ContainerID &&
                            p.ParamIndex == parameterData.ParamIndex &&
                            p.MethodIndex == parameterData.MethodIndex);

                // Add the ParameterControlData object only if it does not already exist
                if (!alreadyExists)
                {
                    list.Add(parameterData);
                    // Save the updated list back to session state
                    globalParameterControlList = list;

                    System.Diagnostics.Debug.WriteLine($"List count after add: {globalParameterControlList.Count}");
                    foreach (var item in globalParameterControlList)
                    {
                        System.Diagnostics.Debug.WriteLine($"ContainerID: {item.ContainerID}, ParamIndex: {item.ParamIndex}, MethodIndex: {item.MethodIndex}");
                        System.Diagnostics.Debug.WriteLine("ParameterDataTypes: " + string.Join(", ", item.ParameterDataTypes));
                        System.Diagnostics.Debug.WriteLine("ParameterNames: " + string.Join(", ", item.ParameterNames));
                        System.Diagnostics.Debug.WriteLine("-----");
                    }
                }
            }

            // Add button
            Button addButton = new Button
            {
                ID = $"addParameter_{index}",
                Text = "+ Add Parameter +",
                CssClass = "btn btn-primary btn-block mt-3",
            };
            addButton.Click += new EventHandler(AddParameterButton_Click);

            // Add the button to the panel
            parameterContentPanel.Controls.Add(addButton);

            // Add the collapsible content panel to the main parameter details panel
            parameterDetailsPanel.Controls.Add(parameterContentPanel);

            // Add the main parameter details panel to the card body if it's visible
            if (parameterDetailsPanel.Visible)
            {
                cardBody.Controls.Add(parameterDetailsPanel);
            }

            // Description
            cardBody.Controls.Add(new Literal { Text = $"<div class='form-group flex'><label for='description_{index}'>Description:</label>" });
            TextBox descriptionBox = new TextBox { ID = "description_" + index, CssClass = "form-control", Text = string.Empty, TextMode = TextBoxMode.MultiLine, Rows = 4 };
            cardBody.Controls.Add(descriptionBox);
            cardBody.Controls.Add(new Literal { Text = "</div>" });

            // Create and add delete button directly
            Button deleteButton = new Button { ID = "deleteButton_" + index, CssClass = "btn btn-danger btn-sm", Text = "Delete Method", CommandArgument = panel.ID, OnClientClick = "return confirm('Are you sure you want to delete this method?');" };
            deleteButton.Click += DeleteButton_Click;
            Panel buttonPanel = new Panel { CssClass = "text-center mt-3" };
            buttonPanel.Controls.Add(deleteButton);
            cardBody.Controls.Add(buttonPanel);

            panel.Controls.Add(cardBody);
            panel.EnableViewState = false;
            return panel;
        }

        protected void AddParameterButton_Click(object sender, EventArgs e)
        {
            // Existing code for setting up the new control
            ViewState["ButtonClicked"] = true;
            Button addButton = (Button)sender;
            string methodIndex = addButton.ID.Split('_')[1];
            Panel parameterContentPanel = (Panel)addButton.Parent;

            if (parameterContentPanel == null)
            {
                return;
            }

            Panel parameterControlsContainer = (Panel)parameterContentPanel.FindControl($"parameterControlsContainer_{methodIndex}");
            if (parameterControlsContainer == null)
            {
                return;
            }
            //parameterControlsContainer.Controls.Clear();

            HiddenField parameterCountField = (HiddenField)parameterContentPanel.FindControl($"parameterCount_{methodIndex}");
            if (parameterCountField == null)
            {
                return;
            }

            int currentCount = int.Parse(parameterCountField.Value);
            currentCount++;
            parameterCountField.Value = currentCount.ToString();

            // Update the visible TextBox as well
            TextBox parametersBox = (TextBox)parameterContentPanel.FindControl($"parameters_{methodIndex}");
            if (parametersBox != null)
            {
                parametersBox.Text = currentCount.ToString();
            }

            List<string> parameterDataTypes = new List<string>();
            List<string> parameterNames = new List<string>();

            // Add the new control
            AddParameterControls(parameterControlsContainer, currentCount - 1, int.Parse(methodIndex), parameterDataTypes, parameterNames);

            // Update globalParameterControlList
            ParameterControlData newControlData = new ParameterControlData
            {
                ContainerID = parameterControlsContainer.ID,
                ParamIndex = currentCount - 1,
                MethodIndex = int.Parse(methodIndex),
                ParameterDataTypes = parameterDataTypes,
                ParameterNames = parameterNames
            };

            globalParameterControlList.Add(newControlData);

            // Update Session
            Session["GlobalParameterControlList"] = globalParameterControlList;

            // Extract values after adding new control
            ExtractParameterValues();
        }

        private void AddParameterControls(Panel container, int paramIndex, int methodIndex, List<string> parameterDataTypes, List<string> parameterNames)
        {
            container.Controls.Add(new Literal { Text = "<div class='flex-container'>" });

            // Parameter Type
            container.Controls.Add(new Literal { Text = $"<div class='form-group flex-item'><label for='parameter_type_{paramIndex}_{methodIndex}'>Parameter {paramIndex + 1} Type:</label>" });
            TextBox parameterTypeBox = new TextBox { ID = $"parameter_type_{paramIndex}_{methodIndex}", CssClass = "form-control", Text = paramIndex < parameterDataTypes.Count ? parameterDataTypes[paramIndex] : string.Empty };
            container.Controls.Add(parameterTypeBox);
            container.Controls.Add(new Literal { Text = "</div>" });

            // Parameter Name and Checkbox
            container.Controls.Add(new Literal { Text = $"<div class='form-group flex-item'>" });

            // Parameter Name Label
            container.Controls.Add(new Literal { Text = $"<label for='parameter_name_{paramIndex}_{methodIndex}'>Parameter {paramIndex + 1} Name:</label>" });

            // Parameter Name TextBox
            TextBox parameterNameBox = new TextBox { ID = $"parameter_name_{paramIndex}_{methodIndex}", CssClass = "form-control", Text = paramIndex < parameterNames.Count ? parameterNames[paramIndex] : string.Empty };
            container.Controls.Add(parameterNameBox);


            container.Controls.Add(new Literal { Text = "</div>" }); // Close the form-group div
            container.Controls.Add(new Literal { Text = "</div>" }); // Close the flex-container div
        }

        protected void ParamTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList paramTypeDropDown = sender as DropDownList;
            if (paramTypeDropDown != null)
            {
                string selectedValue = paramTypeDropDown.SelectedValue;

                // Find the panel that contains this dropdown to access jsonKeysPanel
                Panel parentPanel = paramTypeDropDown.Parent as Panel;
                if (parentPanel != null)
                {
                    int index = int.Parse(paramTypeDropDown.ID.Split('_').Last()); // Extract index from dropdown ID

                    // Find the corresponding jsonKeysPanel
                    Panel jsonKeysPanel = parentPanel.FindControl($"json_keys_panel_{index}") as Panel;
                    if (jsonKeysPanel != null)
                    {
                        if (selectedValue == "json_string")
                        {
                            jsonKeysPanel.Visible = true;
                        }
                        else
                        {
                            jsonKeysPanel.Visible = false;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"jsonKeysPanel_{index} not found.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Parent panel not found.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ParamTypeDropDown is null.");
            }
        }

        protected void HttpMethodDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList httpMethodDropDown = (DropDownList)sender;
            string selectedMethod = httpMethodDropDown.SelectedValue;

            // Find the index from the ID of the DropDownList
            int index = int.Parse(httpMethodDropDown.ID.Split('_')[2]);

            // Traverse up the control hierarchy to find the Panel
            Control parent = httpMethodDropDown;
            while (parent != null && !(parent is Panel))
            {
                parent = parent.Parent;
            }

            if (parent is Panel parentPanel)
            {
                // Find the corresponding postTypePanel control within the Panel
                Panel postTypePanel = (Panel)parentPanel.FindControl($"post_type_panel_{index}");

                if (postTypePanel != null)
                {
                    // Toggle visibility based on HTTP method
                    postTypePanel.Visible = selectedMethod == "POST";
                }
            }
        }

        private int GetIndexFromHttpMethodDropDown(DropDownList httpMethodDropDown)
        {
            // Extract the index from the ID of the DropDownList
            string id = httpMethodDropDown.ID;
            int index;
            if (int.TryParse(id.Substring(id.LastIndexOf('_') + 1), out index))
            {
                return index;
            }
            return -1; // Default to -1 if index extraction fails
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            // Get the button that triggered the event
            var button = (Button)sender;
            string panelID = button.CommandArgument;

            // Find the panel by ID and remove it
            Panel panelToRemove = code_doc.FindControl(panelID) as Panel;
            if (panelToRemove != null)
            {
                code_doc.Controls.Remove(panelToRemove);

                // Add the panel ID to the list of deleted panels
                var deletedPanels = GetDeletedPanels();
                deletedPanels.Add(panelID.Split('_')[2]); // Add the panel index to the list
                ViewState[DeletedPanelsKey] = deletedPanels; // Store the updated list in ViewState
            }
        }

        private List<string> GetDeletedPanels()
        {
            if (ViewState[DeletedPanelsKey] == null)
            {
                ViewState[DeletedPanelsKey] = new List<string>();
            }
            return (List<string>)ViewState[DeletedPanelsKey];
        }

        public static (List<string> MethodNames, Dictionary<string, List<string>> JsonKeys) ExtractMethodNamesAndJsonKeys(string codeText)
        {
            var methodNames = new List<string>();
            var jsonKeys = new Dictionary<string, List<string>>();

            // Updated regex pattern to extract methods and their bodies
            string methodPattern = @"(?<attributes>(\[\s*\w+\s*\([^\)]*\)\s*\])?\s*)?(?<access>public|private|protected|internal|static)\s+(?<modifiers>(async\s+)?(static\s+)?(partial\s+)?)?(?<returnType>\w+(\<\w+\>)?)\s+(?<methodName>\w+)\s*\(.*?\)\s*{(?<methodBody>[\s\S]*?)}";
            var methodRegex = new Regex(methodPattern, RegexOptions.Singleline);
            var methodMatches = methodRegex.Matches(codeText);

            foreach (Match match in methodMatches)
            {
                string methodName = match.Groups["methodName"].Value;
                string methodBody = match.Groups["methodBody"].Value;

                methodNames.Add(methodName);

                var keys = new List<string>();

                // Debugging: Output the method name and body for verification
                Console.WriteLine($"Processing method: {methodName}");
                Console.WriteLine($"Method body: {methodBody}");

                // Extract JSON keys from object initialization blocks
                var initBlockRegex = new Regex(@"new\s+\w+\s*\(\)\s*{[\s\S]*?}", RegexOptions.Singleline);
                var initBlockMatches = initBlockRegex.Matches(methodBody);

                foreach (Match initBlockMatch in initBlockMatches)
                {
                    string initBlock = initBlockMatch.Value;

                    // Debugging: Output the initialization block for verification
                    Console.WriteLine($"Initialization block: {initBlock}");

                    // Regex to find assignments within object initialization blocks
                    var assignmentRegex = new Regex(@"\s*(\w+)\s*=\s*(?:\w+\.)?(\w+)\s*,?", RegexOptions.Singleline);
                    var assignmentMatches = assignmentRegex.Matches(initBlock);

                    foreach (Match assignMatch in assignmentMatches)
                    {
                        string property = assignMatch.Groups[2].Value;

                        // Debugging: Output each matched assignment
                        Console.WriteLine($"Matched property: {property}");

                        keys.Add(property);
                    }
                }

                // Extract JSON keys from LINQ Where clause
                var whereClauseRegex = new Regex(@"Where\s*\(\s*\w+\s*=>\s*\w+\.(\w+)\s*==", RegexOptions.Singleline);
                var whereClauseMatches = whereClauseRegex.Matches(methodBody);

                foreach (Match whereClauseMatch in whereClauseMatches)
                {
                    keys.Add(whereClauseMatch.Groups[1].Value);
                }

                // Remove duplicates and add to dictionary
                if (keys.Count > 0)
                {
                    jsonKeys[methodName] = keys.Distinct().ToList();
                }
            }

            return (methodNames, jsonKeys);
        }

        public static List<string> ExtractMethodNames(string codeText)
        {
            string pattern = @"(?<attributes>(\[\s*\w+\s*\([^\)]*\)\s*\])?\s*)?(?<access>public|private|protected|internal|static)\s+(?<modifiers>(async\s+)?(static\s+)?(partial\s+)?)?(?<returnType>\w+(\<\w+\>)?)\s+(?<methodName>\w+)\s*\(.*?\)\s*{";
            Regex regex = new Regex(pattern);
            var matches = regex.Matches(codeText);

            List<string> methodNames = new List<string>();
            foreach (Match match in matches)
            {
                methodNames.Add(match.Groups["methodName"].Value);
            }

            return methodNames;
        }

        private bool IsValidMethodDeclaration(string codeText, int methodIndex)
        {
            // Extract surrounding context
            int startIndex = Math.Max(0, methodIndex - 50);
            int length = Math.Min(100, codeText.Length - startIndex);
            string context = codeText.Substring(startIndex, length);

            // Check if context contains method calls or other statements
            var invalidPatterns = new[] { "Console.WriteLine", "string.Equals" }; // Add other patterns to exclude if needed

            foreach (var pattern in invalidPatterns)
            {
                if (context.Contains(pattern))
                {
                    return false;
                }
            }

            // Additional checks for other non-method declarations can be added here
            return !IsControlFlowStatement(context);
        }

        private bool IsControlFlowStatement(string context)
        {
            // List of control flow keywords to exclude
            var controlFlowKeywords = new[] { "if", "for", "while", "foreach", "switch", "do", "else" };

            foreach (var keyword in controlFlowKeywords)
            {
                if (context.Contains(keyword))
                {
                    return true;
                }
            }

            return false;
        }

        private List<int> ExtractParameterCounts(string codeText)
        {
            var parameterCounts = new List<int>();

            // Regular expression to match method declarations
            var methodPattern = @"\b(?:public|private|protected|internal|static|abstract|virtual|override|sealed|new|readonly|volatile|unsafe)?\s+[\w<>,]+\s+\w+\s*\(([^)]*)\)\s*{";
            var methodRegex = new Regex(methodPattern);
            var methodMatches = methodRegex.Matches(codeText);

            foreach (Match methodMatch in methodMatches)
            {
                // Split the parameters by commas and trim whitespace from each parameter
                var parameters = methodMatch.Groups[1].Value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToArray();

                parameterCounts.Add(parameters.Length);
            }

            return parameterCounts;
        }

        private List<string> ExtractReturnTypes(string codeText)
        {
            var returnTypes = new List<string>();

            // Regular expression to match method declarations
            var methodPattern = @"\b(?:public|private|protected|internal|static|abstract|virtual|override|sealed|new|readonly|volatile|unsafe)?\s+([\w<>,]+)\s+\w+\s*\(.*?\)\s*{";
            var methodRegex = new Regex(methodPattern);
            var methodMatches = methodRegex.Matches(codeText);

            foreach (Match methodMatch in methodMatches)
            {
                var returnType = methodMatch.Groups[1].Value;
                returnTypes.Add(string.IsNullOrWhiteSpace(returnType) ? "void" : returnType);
            }

            return returnTypes;
        }

        private List<string> ExtractParameterDataTypes(string codeText)
        {
            var parameterDataTypes = new List<string>();

            // Regular expression to match method declarations
            var methodPattern = @"\b(?:public|private|protected|internal|static|abstract|virtual|override|sealed|new|readonly|volatile|unsafe)?\s+[\w<>,]+\s+\w+\s*\(([^)]*)\)\s*{";
            var methodRegex = new Regex(methodPattern);
            var methodMatches = methodRegex.Matches(codeText);

            foreach (Match methodMatch in methodMatches)
            {
                var parameters = methodMatch.Groups[1].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var param in parameters)
                {
                    var trimmedParam = param.Trim();

                    // Check if [FromBody] is present in the parameter definition
                    bool isFromBody = trimmedParam.Contains("[FromBody]");

                    // Extract the data type (first word in the parameter, excluding attributes)
                    var dataTypeMatch = Regex.Match(trimmedParam, @"(?:\[.*?\]\s*)?(\w+)\s+\w+");

                    if (dataTypeMatch.Success)
                    {
                        if (isFromBody)
                        {
                            parameterDataTypes.Add("string");
                        }
                        else
                        {
                            parameterDataTypes.Add(dataTypeMatch.Groups[1].Value);
                        }
                    }
                }
            }

            return parameterDataTypes;
        }

        private List<string> ExtractParameterNames(string codeText)
        {
            var parameterNames = new List<string>();

            // Regular expression to match method declarations
            var methodPattern = @"\b(?:public|private|protected|internal|static|abstract|virtual|override|sealed|new|readonly|volatile|unsafe)?\s+[\w<>,]+\s+\w+\s*\(([^)]*)\)\s*{";
            var parameterPattern = @"(\w+)\s+(\w+)"; // To extract data type and parameter name

            var methodRegex = new Regex(methodPattern);
            var parameterRegex = new Regex(parameterPattern);

            var methodMatches = methodRegex.Matches(codeText);

            foreach (Match methodMatch in methodMatches)
            {
                var parameters = methodMatch.Groups[1].Value;
                var parameterMatches = parameterRegex.Matches(parameters);

                foreach (Match parameterMatch in parameterMatches)
                {
                    parameterNames.Add(parameterMatch.Groups[2].Value); // Parameter name is in the second group
                }
            }

            return parameterNames;
        }

        protected void handleSubmit(object sender, EventArgs e)
        {
            try
            {

                // Retrieve user ID from the session
                if (Session["UserId"] == null)
                {
                    // Redirect to login page if the session is invalid
                    Response.Redirect("~/UI/SignInPage.aspx");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                string codeText = SC_text.Text;
                string platform = Platform.SelectedValue;
                string description = Description.Text;

                string connectionString = ConfigurationManager.ConnectionStrings["MyDbContext"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertCodeTextDetailsQuery = @"
                        INSERT INTO api_header (code_text, code_platform, code_description, code_uploadDate, user_id)
                        VALUES (@CodeText, @Platform, @Description, @UploadDate, @UserId);
                        SELECT SCOPE_IDENTITY();";

                    int codeID;
                    using (SqlCommand codeTextCommand = new SqlCommand(insertCodeTextDetailsQuery, connection))
                    {
                        codeTextCommand.Parameters.AddWithValue("@CodeText", codeText);
                        codeTextCommand.Parameters.AddWithValue("@Platform", platform);
                        codeTextCommand.Parameters.AddWithValue("@Description", description);
                        codeTextCommand.Parameters.AddWithValue("@UploadDate", DateTime.Now);
                        codeTextCommand.Parameters.AddWithValue("@UserId", userId); // Add the user ID here

                        codeID = Convert.ToInt32(codeTextCommand.ExecuteScalar());
                    }


                    foreach (Control control in code_doc.Controls)
                    {
                        if (control is Panel panel)
                        {
                            // Extract method details from the panel
                            string methodName = ((TextBox)panel.FindControl($"method_name_{panel.ID.Split('_')[2]}")).Text;
                            string checkApiQuery = "SELECT COUNT(*) FROM api_methods am JOIN api_header ah ON am.code_id = ah.code_id WHERE am.API_Name = @API_Name AND ah.user_id = @UserId";

                            using (SqlCommand checkApiCommand = new SqlCommand(checkApiQuery, connection))
                            {
                                checkApiCommand.Parameters.AddWithValue("@API_Name", methodName);
                                checkApiCommand.Parameters.AddWithValue("@UserId", userId); // Include user ID in the query

                                int count = (int)checkApiCommand.ExecuteScalar();

                                //if (count > 0)
                                //{
                                //    // Show confirmation popup if method already exists for the current user
                                //    string script = $"if (!confirm('The API method already exists for the current user: {methodName}. Are you sure you want to save it again?')) {{ return false; }}";
                                //    ScriptManager.RegisterStartupScript(this, GetType(), "confirmMessage", script, true);
                                //    break;
                                //}
                            }

                            string descriptionText = ((TextBox)panel.FindControl($"description_{panel.ID.Split('_')[2]}")).Text;
                            string apiEndpointText = ((TextBox)panel.FindControl($"api_endpoint_{panel.ID.Split('_')[2]}")).Text;
                            string httpMethod = ((DropDownList)panel.FindControl($"http_method_{panel.ID.Split('_')[2]}")).SelectedValue;

                            TextBox textBox = (TextBox)panel.FindControl($"parameters_{panel.ID.Split('_')[2]}");
                            int parameterCount;
                            if (textBox != null && int.TryParse(textBox.Text, out parameterCount))
                            {
                                // Successfully parsed the integer
                            }
                            else
                            {
                                parameterCount = 0;
                            }


                            string returnType = ((DropDownList)panel.FindControl($"return_type_{panel.ID.Split('_')[2]}")).SelectedValue;
                            string paramType;

                            // Set paramType to "NONE" if httpMethod is not "POST"
                            if (httpMethod != "POST")
                            {
                                paramType = "NONE";
                            }
                            else
                            {
                                paramType = ((DropDownList)panel.FindControl($"param_type_{panel.ID.Split('_')[2]}")).SelectedValue;
                            }


                            // Insert into API_Details table and get the inserted API_ID
                            string insertApiDetailsQuery = "INSERT INTO api_methods (API_name, API_paracount, API_returnType, API_HTTP_method, API_desc, API_endpoint, API_post_method, code_id) " +
                               "VALUES (@API_Name, @ParaCount, @ReturnDataType, @HTTP_Method, @Description, @ApiEndpoint, @POST_Method, @CodeID); " +
                               "SELECT SCOPE_IDENTITY();";

                            int apiID;
                            using (SqlCommand apiDetailsCommand = new SqlCommand(insertApiDetailsQuery, connection))
                            {
                                apiDetailsCommand.Parameters.AddWithValue("@API_Name", methodName);
                                apiDetailsCommand.Parameters.AddWithValue("@ParaCount", parameterCount);
                                apiDetailsCommand.Parameters.AddWithValue("@ReturnDataType", returnType);
                                apiDetailsCommand.Parameters.AddWithValue("@HTTP_Method", httpMethod);
                                apiDetailsCommand.Parameters.AddWithValue("@Description", descriptionText);
                                apiDetailsCommand.Parameters.AddWithValue("@ApiEndpoint", apiEndpointText);
                                apiDetailsCommand.Parameters.AddWithValue("@POST_Method", paramType);
                                apiDetailsCommand.Parameters.AddWithValue("@CodeID", codeID);

                                apiID = Convert.ToInt32(apiDetailsCommand.ExecuteScalar());
                            }

                            string jsonKeyValue = ((TextBox)panel.FindControl($"json_keys_{panel.ID.Split('_')[2]}")).Text;
                            if (jsonKeyValue == "No JSON key found, please choose another method")
                            {
                                jsonKeyValue = "NONE";
                            }

                            // Insert into ParameterDetails table for each parameter
                            string insertParameterDetailsQuery = "INSERT INTO parameter_details (para_type, para_name, para_json_keys, API_id) " +
                                     "VALUES (@ParaType, @ParaName, @json_Keys, @API_ID);";

                            for (int i = 0; i < parameterCount; i++)
                            {
                                using (SqlCommand parameterDetailsCommand = new SqlCommand(insertParameterDetailsQuery, connection))
                                {
                                    parameterDetailsCommand.Parameters.AddWithValue("@ParaType", ((TextBox)panel.FindControl($"parameter_type_{i}_{panel.ID.Split('_')[2]}")).Text);
                                    parameterDetailsCommand.Parameters.AddWithValue("@ParaName", ((TextBox)panel.FindControl($"parameter_name_{i}_{panel.ID.Split('_')[2]}")).Text);
                                    parameterDetailsCommand.Parameters.AddWithValue("@json_Keys", jsonKeyValue);
                                    parameterDetailsCommand.Parameters.AddWithValue("@API_ID", apiID);

                                    parameterDetailsCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("One or more controls not found within the panel.");
                        }
                    }

                    // Optionally, redirect or display a success message
                    Response.Write("<script>alert('Code details submitted successfully!');</script>");
                    code_doc.Visible = false;
                    btnSubmit.Visible = false;
                }
                ClearAllControls();
            }
            catch (Exception ex)
            {
                // Log the detailed error
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                Response.Write($"<script>alert('An error occurred: {ex.Message}');</script>");
            }

        }

        private void ClearAllControls()
        {
            // Clear textboxes
            SC_text.Text = string.Empty;
            Description.Text = string.Empty;

            // Reset dropdown lists
            Platform.SelectedIndex = -1;

            // Clear code_doc panels
            code_doc.Controls.Clear();
        }
    }
}