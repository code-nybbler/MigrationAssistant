using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using SelectionMode = System.Windows.Forms.SelectionMode;

namespace MigrationAssistant
{
    public partial class MigrationAssistantControl : PluginControlBase
    {
        private Dictionary<string, Guid> solutions;
        private List<Field> mappings;
        Dictionary<string, IEnumerable<EntityMetadata>> entities;
        private ListBox sourceTab, destinationTab;
        private Field sourceField, destinationField;
        private string solution, prefix, schemaFormat, serverConnectionString, database, tableOrSchema, tableToPublish, saveFilePath;
        private int sourceItemIdx, destinationItemIdx, mode;
        private Factory EntityFactory;

        public MigrationAssistantControl()
        {
            InitializeComponent();
        }

        private void box_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (box_DataType.Text)
            {
                case "Single Line of Text":
                    num_FieldLength.Enabled = true;
                    num_FieldLength.Value = 100;
                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 4000;
                    break;
                case "Multiple Lines of Text":
                    num_FieldLength.Enabled = true;
                    num_FieldLength.Value = 2000;
                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 1048576;
                    break;
                case "Whole Number":
                    num_FieldLength.Value = 0;
                    num_FieldLength.Enabled = false;
                    break;
                case "Decimal":
                    num_FieldLength.Enabled = true;
                    num_FieldLength.Value = 2;
                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 10;
                    break;
                case "Money":
                    num_FieldLength.Enabled = true;
                    num_FieldLength.Value = 2;
                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 4;
                    break;
                default: num_FieldLength.Enabled = false; break;
            }

            if (CanCreateField()) btn_CreateField.Enabled = true;
        }
        private void box_Format_SelectedIndexChanged(object sender, EventArgs e)
        {
            schemaFormat = box_Format.SelectedItem.ToString();

            if (txt_FieldDisplayName.Text != string.Empty)
            {
                txt_FieldName.Text = ApplySelectedFormat(txt_FieldDisplayName.Text);
            }

            if (txt_PrimaryDisplayName.Text != string.Empty)
            {
                txt_PrimaryName.Text = ApplySelectedFormat(txt_PrimaryDisplayName.Text);
            }

            if (txt_TableDisplayName.Text != string.Empty)
            {
                txt_TableName.Text = ApplySelectedFormat(txt_TableDisplayName.Text);
            }
        }
        private void box_PrimaryRequirement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CanCreateTable()) btn_CreateTable.Enabled = true;
        }
        private void box_Requirement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CanCreateField()) btn_CreateField.Enabled = true;
        }
        private void box_Solution_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(SelectSolution);
            if (CanCreateTable()) btn_CreateTable.Enabled = true;
        }
        private void box_Table_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CanCreateField()) btn_CreateField.Enabled = true;
        }
        private void btn_Close_Click(object sender, EventArgs e)
        {
            CloseTool();
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (box_Server.Text != string.Empty && box_Database.Text != string.Empty && box_Mode.Text != string.Empty && box_TableSchema.Text != string.Empty)
            {
                serverConnectionString = box_Server.Text;
                database = box_Database.Text;
                mode = box_Mode.SelectedIndex;
                tableOrSchema = box_TableSchema.Text;

                EntityFactory.Database = database;
                EntityFactory.InspectData = box_InspectData.Checked;
                EntityFactory.IgnoreEmpty = box_IgnoreEmpty.Checked;
                EntityFactory.Connection = new SqlConnection(serverConnectionString);

                ExecuteMethod(ReadDatabase);
            }
        }
        private void btn_CreateField_Click(object sender, EventArgs e)
        {
            if (solution != null && prefix != null && box_Table.SelectedItem != null && txt_FieldDisplayName.Text != string.Empty && box_DataType.SelectedItem != null)
            {
                ExecuteMethod(CreateField);
            }
        }
        private void btn_CreateTable_Click(object sender, EventArgs e)
        {
            if (txt_TableDisplayName.Text != string.Empty && txt_TablePluralName.Text != string.Empty && txt_PrimaryDisplayName.Text != string.Empty && txt_PrimaryName.Text != string.Empty)
            {
                ExecuteMethod(CreateTable);
            }
        }
        private void btn_ExportExcel_Click(object sender, EventArgs e)
        {
            using (var selectedFolder = new FolderBrowserDialog())
            {
                if (selectedFolder.ShowDialog() == DialogResult.OK)
                {
                    saveFilePath = selectedFolder.SelectedPath;
                }
            }

            bool includeUnmapped;
            var includeUnmappedConfirmation = MessageBox.Show("Do you want to include unmapped tables and columns?", "Confirmation", MessageBoxButtons.YesNo);
            includeUnmapped = includeUnmappedConfirmation == DialogResult.Yes;

            ExportExcel(includeUnmapped);
        }
        private void btn_ImportCSV_Click(object sender, EventArgs e)
        {
            using (var selectedFile = new OpenFileDialog
                                        {
                                            InitialDirectory = @"c:\",
                                            Title = "Browse CSV Files",
                                            CheckFileExists = true,
                                            CheckPathExists = true,
                                            Filter = "csv files (*.csv)|*.csv",
                                            FilterIndex = 2,
                                            RestoreDirectory = true
                                        })
            {
                if (selectedFile.ShowDialog() == DialogResult.OK)
                {
                    mappings = EntityFactory.ImportFromCSV(selectedFile.FileName);

                    int tabIndex = -1;
                    foreach (Field field in mappings)
                    {
                        foreach (TabPage tp in sourceTabControl.TabPages)
                        {
                            if (tp.Name == field.TABLE_NAME)
                            {
                                tabIndex = sourceTabControl.TabPages.IndexOf(tp);
                                break;
                            }
                        }

                        if (tabIndex != -1)
                        {
                            sourceTab = (ListBox)sourceTabControl.TabPages[tabIndex].Controls[0];
                            sourceField = field;
                            sourceItemIdx = sourceTab.Items.IndexOf(sourceField.COLUMN_NAME + " (" + sourceField.DATA_TYPE + ")");
                            destinationField = field.MAPPED_FIELD;
                            ExecuteMethod(MapColumn);
                        }
                    }
                }
            }
        }
        private void btn_LoadSolutionEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);
            ExecuteMethod(SelectSolution);
        }
        private void btn_Map_Click(object sender, EventArgs e)
        {
            if (sourceField != null && destinationField != null)
            {
                if (mappings.Find(r => r.COLUMN_NAME == sourceField.COLUMN_NAME && r.TABLE_NAME == sourceField.TABLE_NAME) == null)
                {
                    sourceItemIdx = sourceTab.SelectedIndex;                    
                }
                else sourceItemIdx = sourceTab.SelectedIndex + 1;

                sourceField.MAPPED_FIELD = destinationField;
                mappings.Add(sourceField);

                ExecuteMethod(MapColumn);
                btn_Map.Enabled = false;
            }
            else
            {
                MessageBox.Show("Please select a source field and a destination field.", "Missing Selection");
            }
        }
        private void btn_PublishField_Click(object sender, EventArgs e)
        {
            ExecuteMethod(PublishCustomizations);
        }
        private void btn_PublishTable_Click(object sender, EventArgs e)
        {
            ExecuteMethod(PublishCustomizations);
        }
        private void btn_SaveCSV_Click(object sender, EventArgs e)
        {
            using (var selectedFolder = new FolderBrowserDialog())
            {
                if (selectedFolder.ShowDialog() == DialogResult.OK)
                {
                    saveFilePath = selectedFolder.SelectedPath;
                    ExecuteMethod(SaveCSV);
                }
            }
        }
        private void btn_Unmap_Click(object sender, EventArgs e)
        {
            string selection, sourceItem, destinationFieldName, destinationFieldType, destinationItem;

            if (sourceTab.SelectedItem != null)
            {
                selection = sourceTab.SelectedItem.ToString();
                if (sourceField != null && selection.Contains("->"))
                {
                    sourceItem = sourceField.COLUMN_NAME + " (" + sourceField.DATA_TYPE + ")";
                    sourceItemIdx = sourceTab.SelectedIndex;

                    sourceTab.Items.Remove(sourceTab.SelectedItem);

                    int duplicates = 0;
                    foreach (object item in sourceTab.Items)
                    {
                        if (item.ToString().StartsWith(sourceField.COLUMN_NAME)) duplicates++;
                    }

                    if (duplicates == 0) sourceTab.Items.Insert(sourceItemIdx, sourceItem);

                    destinationFieldName = selection.Split('>')[1].Split('.')[1].Split('(')[0].Trim(' ');
                    destinationFieldType = selection.Split('>')[1].Split('.')[1].Split('(')[1].Trim(')');
                    destinationItem = destinationFieldName + " (" + destinationFieldType + ")";

                    mappings.Remove(mappings.Find(r => r.COLUMN_NAME == sourceField.COLUMN_NAME && r.MAPPED_FIELD.COLUMN_NAME == destinationFieldName));
                    btn_Unmap.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please select a source field to unmap.", "Missing Selection");
            }
        }
        private void destinationTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            destinationTab = (ListBox)((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Controls[0];
            destinationField = null;
        }
        private void DestinationField_Click(object sender, EventArgs e)
        {
            destinationTab = (ListBox)sender;
            if (destinationTab.SelectedItem != null)
            {
                string[] parts = destinationTab.SelectedItem.ToString().Split(' ');
                string name = parts[0];
                string type = parts[1].Trim('(').Trim(')');
                destinationField = new Field(name, type, destinationTab.Name);

                if (sourceTab != null && sourceTab.SelectedItem != null) btn_Map.Enabled = true;
            }
        }
        private void lst_Solutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_Solutions.CheckedItems.Count > 0)
            {
                btn_LoadSolutionEntities.Enabled = true;
                box_Solution.Text = lst_Solutions.CheckedItems[0].ToString();
                box_Solution.Enabled = true;

                ExecuteMethod(SelectSolution);

                box_Solution.Items.Clear();
                foreach (string item in lst_Solutions.CheckedItems)
                {
                    box_Solution.Items.Add(item);
                }
            }
            else
            {
                btn_LoadSolutionEntities.Enabled = false;
            }
        }
        private void sourceTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            sourceTab = (ListBox)((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Controls[0];
            sourceField = null;

            txt_TableDisplayName.Text = sourceTab.Name;
            txt_PrimaryDisplayName.Text = "Name";
        }
        private void SourceField_Click(object sender, EventArgs e)
        {
            sourceTab = (ListBox)sender;
            txt_TableDisplayName.Text = sourceTab.Name;
            txt_PrimaryDisplayName.Text = "Name";

            if (sourceTab.SelectedItem != null)
            {
                string fieldName = sourceTab.SelectedItem.ToString().Split('(')[0].Trim(' ');
                string dataType = sourceTab.SelectedItem.ToString().Split('(')[1].Split(')')[0];

                if (destinationTab != null)
                {
                    box_Table.Text = destinationTab.Name;
                }

                txt_FieldDisplayName.Text = fieldName;

                switch (dataType)
                {
                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "binary":
                    case "varbinary":
                    case "image": box_DataType.Text = "Single Line of Text"; break;
                    case "bit": box_DataType.Text = "Two Option"; break;
                    case "tinyint":
                    case "smallint":
                    case "int":
                    case "bigint": box_DataType.Text = "Whole Number"; break;
                    case "decimal":
                    case "numeric":
                    case "float":
                    case "real": box_DataType.Text = "Decimal"; break;
                    case "smallmoney":
                    case "money": box_DataType.Text = "Money"; break;
                    case "datetime":
                    case "datetime2":
                    case "smalldatetime":
                    case "date": box_DataType.Text = "Date Only"; break;
                    case "time":
                    case "datetimeoffset":
                    case "timestamp": box_DataType.Text = "Date and Time"; break;
                    default: break;
                }

                if (sourceTab.SelectedItem.ToString().Contains("->")) btn_Unmap.Enabled = true;

                sourceField = EntityFactory.TableList.Find(t => t.NAME == sourceTab.Name).FIELDS.Find(f => f.COLUMN_NAME == fieldName);

                if (destinationTab != null && destinationTab.SelectedItem != null) btn_Map.Enabled = true;
            }
        }
        private void txt_FieldDisplayName_TextChanged(object sender, EventArgs e)
        {
            if (txt_FieldDisplayName.Text != string.Empty)
            {
                txt_FieldName.Text = ApplySelectedFormat(txt_FieldDisplayName.Text);

                if (CanCreateField()) btn_CreateField.Enabled = true;
            }
        }
        private void txt_FieldName_TextChanged(object sender, EventArgs e)
        {
            if (txt_FieldName.Text != string.Empty)
            {
                txt_FieldName.Text = RemovePunctuations(txt_FieldName.Text);

                if (CanCreateField()) btn_CreateField.Enabled = true;
            }
        }
        private void txt_PrimaryDisplayName_TextChanged(object sender, EventArgs e)
        {
            if (txt_PrimaryDisplayName.Text != string.Empty)
            {
                txt_PrimaryName.Text = ApplySelectedFormat(txt_PrimaryDisplayName.Text);

                if (CanCreateTable()) btn_CreateTable.Enabled = true;
            }
        }
        private void txt_PrimaryName_TextChanged(object sender, EventArgs e)
        {
            if (txt_PrimaryName.Text != string.Empty)
            {
                txt_PrimaryName.Text = RemovePunctuations(txt_PrimaryName.Text);

                if (CanCreateTable()) btn_CreateTable.Enabled = true;
            }
        }
        private void txt_TableDisplayName_TextChanged(object sender, EventArgs e)
        {
            if (txt_TableDisplayName.Text != string.Empty)
            {
                string tablePluralName;

                if (txt_TableDisplayName.Text.EndsWith("s")) tablePluralName = txt_TableDisplayName.Text + "es";
                else if (Regex.IsMatch(txt_TableDisplayName.Text, "\b.*(a|e|i|o|u)y\b")) tablePluralName = txt_TableDisplayName.Text + "s";
                else if (txt_TableDisplayName.Text.EndsWith("y")) tablePluralName = txt_TableDisplayName.Text.Substring(0, txt_TableDisplayName.Text.Length - 1) + "ies";
                else tablePluralName = txt_TableDisplayName.Text + "s";

                txt_TablePluralName.Text = tablePluralName;

                txt_TableName.Text = ApplySelectedFormat(txt_TableDisplayName.Text);

                if (CanCreateTable()) btn_CreateTable.Enabled = true;
            }
        }
        private void txt_TableName_TextChanged(object sender, EventArgs e)
        {
            if (txt_TableName.Text != string.Empty)
            {
                txt_TableName.Text = RemovePunctuations(txt_TableName.Text);

                if (CanCreateTable()) btn_CreateTable.Enabled = true;
            }
        }
        private void txt_TablePluralName_TextChanged(object sender, EventArgs e)
        {
            if (CanCreateTable()) btn_CreateTable.Enabled = true;
        }

        private string ApplySelectedFormat(string source)
        {
            switch (schemaFormat)
            {
                case "camelCase":
                    source = RemovePunctuations(ToCamelCase(source));
                    break;
                case "lowercase":
                    source = RemovePunctuations(source).ToLower();
                    break;
                case "PascalCase":
                    source = RemovePunctuations(source);
                    break;
                case "UPPERCASE":
                    source = RemovePunctuations(source).ToUpper();
                    break;
                default: break;
            }

            return source;
        }
        private bool CanCreateField()
        {
            return txt_FieldDisplayName.Text != string.Empty && txt_FieldName.Text != string.Empty && box_DataType.Text != string.Empty && box_Table.Text != string.Empty && box_Requirement.SelectedItem != null;
        }
        private bool CanCreateTable()
        {
            return solution != string.Empty && txt_TableDisplayName.Text != string.Empty && txt_TablePluralName.Text != string.Empty && txt_TableName.Text != string.Empty && txt_PrimaryDisplayName.Text != string.Empty && txt_PrimaryName.Text != string.Empty && box_PrimaryRequirement.SelectedItem != null;
        }
        private bool CheckFieldExists(string tableName, string fieldName)
        {
            bool exists = false;

            try
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Attributes,
                    LogicalName = prefix + tableName,
                    RetrieveAsIfPublished = true
                };

                if ((Service.Execute(req) as RetrieveEntityResponse).EntityMetadata != null)
                {
                    foreach (AttributeMetadata am in (Service.Execute(req) as RetrieveEntityResponse).EntityMetadata.Attributes)
                    {
                        if (am.LogicalName == fieldName)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                return exists;
            }
            catch (Exception ex)
            {
                return exists;
            }
        }
        private void CreateField()
        {
            string table = box_Table.SelectedItem.ToString();
            string fieldLabel = txt_FieldDisplayName.Text;
            string fieldSchema = txt_FieldName.Text;
            string dataType = box_DataType.SelectedItem.ToString();
            int length = (int)num_FieldLength.Value;
            int required = box_Requirement.SelectedIndex == 0 ? 0 : box_Requirement.SelectedIndex + 1;

            if (!CheckFieldExists(table, fieldSchema))
            {
                btn_CreateField.Enabled = false;
                btn_Connect.Enabled = false;
                btn_LoadSolutionEntities.Enabled = false;
                ExecuteMethod(DisableInputs);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Creating attribute...",
                    Work = (w, e) =>
                    {
                        // This code is executed in another thread
                        EntityFactory.CreateTableAttribute(solution, prefix, table, fieldLabel, fieldSchema, dataType, length, required);

                        w.ReportProgress(-1, "Attribute created successfully.");
                        e.Result = 1;
                    },
                    ProgressChanged = e =>
                    {
                        SetWorkingMessage(e.UserState.ToString());
                    },
                    PostWorkCallBack = e =>
                    {
                        // This code is executed in the main thread
                        tableToPublish = table;

                        btn_PublishField.Enabled = true;
                        btn_Connect.Enabled = true;
                        btn_LoadSolutionEntities.Enabled = true;
                        ExecuteMethod(EnableInputs);
                    },
                    AsyncArgument = null,
                    // Progress information panel size
                    MessageWidth = 340,
                    MessageHeight = 150
                });
            }
        }
        private void CreateTable()
        {
            string tableDisplayName = txt_TableDisplayName.Text;
            string tableName = txt_TableName.Text;
            string tablePluralName = txt_TablePluralName.Text;
            string tableDesc = txt_tableDescription.Text;
            string primaryDisplayName = txt_PrimaryDisplayName.Text;
            string primaryName = txt_PrimaryName.Text;
            int primaryLength = (int)num_PrimaryLength.Value, primaryReq = 0;
            if (box_PrimaryRequirement.SelectedIndex > -1) primaryReq = box_PrimaryRequirement.SelectedIndex == 0 ? 0 : box_PrimaryRequirement.SelectedIndex + 1;

            // Check if entity already exists
            if (RetrieveEntityMetaData(tableName) == null)
            {
                btn_CreateTable.Enabled = false;
                btn_Connect.Enabled = false;
                btn_LoadSolutionEntities.Enabled = false;
                ExecuteMethod(DisableInputs);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Creating entity...",
                    Work = (w, e) =>
                    {
                        // This code is executed in another thread
                        EntityFactory.CreateTable(solution, prefix, tableDisplayName, tablePluralName, tableName, tableDesc, primaryDisplayName, primaryName, primaryLength, primaryReq);

                        w.ReportProgress(-1, "Entity created successfully.");
                        e.Result = 1;
                    },
                    ProgressChanged = e =>
                    {
                        SetWorkingMessage(e.UserState.ToString());
                    },
                    PostWorkCallBack = e =>
                    {
                        // This code is executed in the main thread
                        EntityMetadata resultEntity = RetrieveEntityMetaData(tableName.ToLower()); // Retrieve newly created entity
                        tableToPublish = resultEntity.LogicalName;

                        btn_PublishEntity.Enabled = true;
                        btn_Connect.Enabled = true;
                        btn_LoadSolutionEntities.Enabled = true;
                        ExecuteMethod(EnableInputs);
                    },
                    AsyncArgument = null,
                    // Progress information panel size
                    MessageWidth = 340,
                    MessageHeight = 150
                });
            }
            else
            {
                MessageBox.Show($"An entity with the name \"{tableName}\" already exists.");
            }
        }
        private void DisableInputs()
        {
            lst_Solutions.Enabled = false;
            box_Solution.Enabled = false;
            box_Format.Enabled = false;
            box_Table.Enabled = false;
            box_DataType.Enabled = false;
            txt_FieldDisplayName.Enabled = false;
            txt_FieldName.Enabled = false;
            box_Requirement.Enabled = false;
            num_FieldLength.Enabled = false;
            txt_TableDisplayName.Enabled = false;
            txt_TablePluralName.Enabled = false;
            txt_TableName.Enabled = false;
            txt_tableDescription.Enabled = false;
            txt_PrimaryDisplayName.Enabled = false;
            txt_PrimaryName.Enabled = false;
            box_PrimaryRequirement.Enabled = false;
            num_PrimaryLength.Enabled = false;
        }
        private void EnableInputs()
        {
            lst_Solutions.Enabled = true;
            box_Solution.Enabled = true;
            box_Format.Enabled = true;
            box_Table.Enabled = true;
            box_DataType.Enabled = true;
            txt_FieldDisplayName.Enabled = true;
            txt_FieldName.Enabled = true;
            box_Requirement.Enabled = true;
            num_FieldLength.Enabled = true;
            txt_TableDisplayName.Enabled = true;
            txt_TablePluralName.Enabled = true;
            txt_TableName.Enabled = true;
            txt_tableDescription.Enabled = true;
            txt_PrimaryDisplayName.Enabled = true;
            txt_PrimaryName.Enabled = true;
            box_PrimaryRequirement.Enabled = true;
            num_PrimaryLength.Enabled = true;
        }
        private void ExportExcel(bool includeUnmapped)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Exporting to Excel...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.ExportMappingToExcel(tableOrSchema, saveFilePath, includeUnmapped);

                    w.ReportProgress(-1, "Exported to Excel successfully.");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    MessageBox.Show(string.Format("File saved to {0}", saveFilePath), "File saved successfully");
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private void GetSolutionEntities(string SolutionUniqueName)
        {
            // get solution components for solution unique name
            QueryExpression componentsQuery = new QueryExpression
            {
                EntityName = "solutioncomponent",
                ColumnSet = new ColumnSet("objectid"),
                Criteria = new FilterExpression(),
            };

            LinkEntity solutionLink = new LinkEntity("solutioncomponent", "solution", "solutionid", "solutionid", JoinOperator.Inner)
            {
                LinkCriteria = new FilterExpression()
            };

            solutionLink.LinkCriteria.AddCondition(new ConditionExpression("uniquename", ConditionOperator.Equal, SolutionUniqueName));
            componentsQuery.LinkEntities.Add(solutionLink);
            componentsQuery.Criteria.AddCondition(new ConditionExpression("componenttype", ConditionOperator.Equal, 1));

            EntityCollection ComponentsResult = Service.RetrieveMultiple(componentsQuery);

            //Get all entities
            RetrieveAllEntitiesRequest AllEntitiesrequest = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            RetrieveAllEntitiesResponse AllEntitiesresponse = (RetrieveAllEntitiesResponse)Service.Execute(AllEntitiesrequest);

            //Join entities Id and solution Components Id
            IEnumerable<EntityMetadata> results = AllEntitiesresponse.EntityMetadata.Join(ComponentsResult.Entities.Select(x => x.Attributes["objectid"]), x => x.MetadataId, y => y, (x, y) => x);

            entities.Add(SolutionUniqueName, results);
        }
        private void LoadEntities()
        {
            entities = new Dictionary<string, IEnumerable<EntityMetadata>>();

            List<string> solutionSelections = new List<string>();
            foreach (string sel in lst_Solutions.CheckedItems)
            {
                solutionSelections.Add(sel);
            }

            btn_Connect.Enabled = false;
            btn_LoadSolutionEntities.Enabled = false;
            ExecuteMethod(DisableInputs);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving entities...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    foreach (string sel in solutionSelections)
                    {
                        GetSolutionEntities(sel);
                    }

                    w.ReportProgress(-1, "Entities loaded.");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    TabPage tabPage;
                    ListBox box;
                    object fieldObj;

                    destinationTabControl.TabPages.Clear();

                    int tabIdx = 0;
                    foreach (KeyValuePair<string, IEnumerable<EntityMetadata>> kv in entities)
                    {
                        foreach (EntityMetadata entity in kv.Value)
                        {
                            tabPage = new TabPage
                            {
                                Name = entity.LogicalName,
                                Text = entity.LogicalName
                            };

                            destinationTabControl.TabPages.Add(tabPage);

                            box = new ListBox
                            {
                                Name = tabPage.Name,
                                SelectionMode = SelectionMode.One,
                                Height = 434,
                                Width = 9999,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                            };

                            box.SelectedIndexChanged += new EventHandler(DestinationField_Click);

                            foreach (AttributeMetadata field in entity.Attributes)
                            {
                                field.AttributeTypeName.Value = field.AttributeTypeName.Value.Replace("Type", "");
                                fieldObj = field.LogicalName + " (" + field.AttributeTypeName.Value.ToString() + ")";
                                box.Items.Add(fieldObj);
                            }

                            tabPage.Controls.Add(box);
                            box_Table.Items.Add(entity.LogicalName);
                            box_Table.Enabled = true;

                            if (tabIdx == 0)
                            {
                                destinationTab = box;
                                tabIdx++;
                            }
                        }
                    }

                    btn_Connect.Enabled = true;
                    btn_LoadSolutionEntities.Enabled = true;
                    ExecuteMethod(EnableInputs);
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private void MapColumn()
        {
            EntityFactory.TableList.Find(t => t.NAME == sourceField.TABLE_NAME).MAPPED_ENTITY_NAME = destinationField.TABLE_NAME;

            string item = sourceField.COLUMN_NAME + " (" + sourceField.DATA_TYPE + ")";

            sourceTab.Items.Remove(item);
            sourceTab.Items.Insert(sourceItemIdx, item + " -> " + destinationField.TABLE_NAME + "." + destinationField.COLUMN_NAME + " (" + destinationField.DATA_TYPE + ")");
        }
        private void MigrationAssistantControl_Load(object sender, EventArgs e)
        {
            EntityFactory = new Factory(Service);
            mappings = new List<Field>();
            schemaFormat = "PascalCase";

            ExecuteMethod(RetrieveSolutions);
        }
        private void PublishCustomizations()
        {
            btn_PublishEntity.Enabled = false;
            btn_PublishField.Enabled = false;
            btn_Connect.Enabled = false;
            btn_LoadSolutionEntities.Enabled = false;
            ExecuteMethod(DisableInputs);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Publishing entity...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    PublishXmlRequest PublishRequest = new PublishXmlRequest
                    {
                        ParameterXml = "<importexportxml><entities><entity>" + tableToPublish + "</entity></entities></importexportxml>"
                    };
                    Service.Execute(PublishRequest);

                    w.ReportProgress(-1, "Publishing complete.");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    btn_Connect.Enabled = true;
                    btn_LoadSolutionEntities.Enabled = true;
                    ExecuteMethod(EnableInputs);
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private void ReadDatabase()
        {
            btn_Connect.Enabled = false;
            btn_LoadSolutionEntities.Enabled = false;
            ExecuteMethod(DisableInputs);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Reading database...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.TableList = EntityFactory.ReadAllSQLTables(mode, tableOrSchema);

                    w.ReportProgress(-1, "Database read successfully");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    TabPage tabPage;
                    ListBox box;
                    object fieldObj;

                    sourceTabControl.TabPages.Clear();

                    int tabIdx = 0;
                    foreach (Table table in EntityFactory.TableList)
                    {
                        tabPage = new TabPage
                        {
                            Name = table.NAME,
                            Text = table.NAME
                        };

                        sourceTabControl.TabPages.Add(tabPage);

                        box = new ListBox
                        {
                            Name = tabPage.Name,
                            SelectionMode = SelectionMode.One,
                            Height = 434,
                            Width = 9999,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right                            
                        };

                        box.SelectedIndexChanged += new EventHandler(SourceField_Click);

                        foreach (Field field in table.FIELDS)
                        {
                            fieldObj = field.COLUMN_NAME + " (" + field.DATA_TYPE + ")";
                            box.Items.Add(fieldObj);
                        }

                        tabPage.Controls.Add(box);

                        if (tabIdx == 0)
                        {
                            sourceTab = box;
                            tabIdx++;
                        }
                    }

                    btn_Connect.Enabled = true;
                    btn_LoadSolutionEntities.Enabled = true;
                    ExecuteMethod(EnableInputs);
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private static string RemovePunctuations(string Source)
        {
            return Regex.Replace(Source, "[!\"#$%&'()*+,-./:;<=>?@\\[\\]^_`{|}~ ]", string.Empty);
        }
        private EntityMetadata RetrieveEntityMetaData(string tableName)
        {
            try
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Entity,
                    LogicalName = prefix + tableName,
                    RetrieveAsIfPublished = true
                };
                return (Service.Execute(req) as RetrieveEntityResponse).EntityMetadata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void RetrievePublisher(Guid publisherId)
        {
            QueryByAttribute query = new QueryByAttribute
            {
                EntityName = "publisher",
                ColumnSet = new ColumnSet("customizationprefix")
            };

            query.Attributes.Add("publisherid");
            query.Values.Add(publisherId);

            try
            {
                EntityCollection retrievedPublishers = Service.RetrieveMultiple(query);

                if (retrievedPublishers.Entities.Count > 0)
                {
                    prefix = retrievedPublishers.Entities[0]["customizationprefix"].ToString() + "_";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void RetrieveSolutions()
        {
            lst_Solutions.Items.Clear();
            solutions = new Dictionary<string, Guid>();

            QueryExpression query = new QueryExpression
            {
                EntityName = "solution",
                ColumnSet = new ColumnSet("uniquename", "publisherid"),
                Criteria = new FilterExpression()
            };

            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);

            try
            {
                foreach (Entity solution in Service.RetrieveMultiple(query).Entities)
                {
                    if (solution["uniquename"].ToString() != "System" && solution["uniquename"].ToString() != "Active" && solution["uniquename"].ToString() != "Basic" && solution["uniquename"].ToString() != "ActivityFeeds" && !string.IsNullOrEmpty(solution["publisherid"].ToString()))
                    {
                        solutions.Add(solution["uniquename"].ToString(), solution.GetAttributeValue<EntityReference>("publisherid").Id);
                        lst_Solutions.Items.Add(solution["uniquename"].ToString());
                    }
                }
                lst_Solutions.Enabled = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void SaveCSV()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Saving mappings...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.ExportMappingToCSV(tableOrSchema, saveFilePath);

                    w.ReportProgress(-1, "Mappings saved successfully.");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    MessageBox.Show(string.Format("File saved to {0}", saveFilePath), "File saved successfully");
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private void SelectSolution()
        {
            if (box_Solution.Text != string.Empty)
            {
                string selectedSolution = box_Solution.Text;
                KeyValuePair<string, Guid> solutionKVP = solutions.FirstOrDefault(x => x.Key == selectedSolution);

                if (solutionKVP.Key != null)
                {
                    box_Table.Items.Clear();
                    solution = solutionKVP.Key;
                    RetrievePublisher(solutionKVP.Value);

                    if (entities != null && entities.Keys.Contains(selectedSolution))
                    {
                        foreach (EntityMetadata entity in entities[selectedSolution])
                        {
                            box_Table.Items.Add(entity.LogicalName);
                        }
                    }
                }
            }
        }
        private string ToCamelCase(string source)
        {
            if (source.Contains(' ')) source = source.Split(' ')[0].ToLower() + string.Join(string.Empty, source.Split(' ').Skip(1).ToArray()).Replace(" ", string.Empty);
            else source = source.ToLower();

            return source;
        }
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            ExecuteMethod(RetrieveSolutions);
        }
    }
}