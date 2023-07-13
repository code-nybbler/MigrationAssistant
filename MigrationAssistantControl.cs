using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using static System.Windows.Forms.CheckedListBox;
using SelectionMode = System.Windows.Forms.SelectionMode;

namespace MigrationAssistant
{
    public partial class MigrationAssistantControl : PluginControlBase
    {
        private List<Entity> solutions;
        private Dictionary<string, List<Field>> mappings;
        Dictionary<string, IEnumerable<EntityMetadata>> entities;
        private ListBox sourceTab, destinationTab;
        private Field sourceField, destinationField;
        private string solution, prefix, schemaFormat, serverConnectionString, database, tableOrSchema, tableToPublish, saveFilePath;
        private int optionValuePrefix, sourceItemIdx, mode, languageCode;
        private Factory EntityFactory;
        private Dictionary<int, string> languages;

        public MigrationAssistantControl()
        {
            InitializeComponent();
        }

        private void box_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteMethod(ToggleFieldInputs);
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
        private void box_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (box_Language.SelectedItem != null)
            {
                string pattern = @"[0-9]+";

                Match match = Regex.Match(box_Language.SelectedItem.ToString(), pattern);
                if (match != null)
                {
                    languageCode = int.Parse(match.Value);
                }
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

            if (box_Target.SelectedItem != null && box_Table.SelectedItem != null && txt_FieldName.Text != string.Empty)
            {
                txt_RelationshipName.Text = box_Target.SelectedItem.ToString() + "_" + box_Table.SelectedItem.ToString() + "_" + txt_FieldName.Text;
            }

            if ((box_Table.SelectedItem.ToString() == "account" || box_Table.SelectedItem.ToString() == "contact") && box_DataType.SelectedItem.ToString() == "Customer")
            {
                MessageBox.Show("Polymorphic lookups cannot be self-referential.");
            }
        }
        private void box_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (box_Target.SelectedItem != null && box_Table.SelectedItem != null && txt_FieldName.Text != string.Empty)
            {
                txt_RelationshipName.Text = box_Target.SelectedItem.ToString() + "_" + box_Table.SelectedItem.ToString() + "_" + txt_FieldName.Text;
            }
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
                tableOrSchema = box_TableSchema.Text.ToLower();

                EntityFactory.Database = database;
                EntityFactory.IgnoreEmpty = box_IgnoreEmpty.Checked;
                EntityFactory.Connection = new SqlConnection(serverConnectionString);

                ExecuteMethod(ReadDatabase);
            }
        }
        private void btn_CreateField_Click(object sender, EventArgs e)
        {
            if (solution != null && prefix != null && box_Table.SelectedItem != null && txt_FieldDisplayName.Text != string.Empty && box_DataType.SelectedItem != null)
            {
                CheckedItemCollection optionSetValues = null;
                bool createField = true;

                if (box_DataType.SelectedItem.ToString() == "Option Set" || box_DataType.SelectedItem.ToString() == "MultiSelect Option Set")
                {
                    bool useValues;
                    var useValuesConfirmation = MessageBox.Show("Do you want to use the selected distinct values?", "Confirmation", MessageBoxButtons.YesNo);
                    useValues = useValuesConfirmation == DialogResult.Yes;

                    if (useValues)
                    {
                        if (lst_Values.CheckedItems.Count > 0)
                        {
                            optionSetValues = lst_Values.CheckedItems;
                        }
                        else
                        {
                            var continueConfirmation = MessageBox.Show("No values have been selected. Do you want to continue?", "Confirmation", MessageBoxButtons.YesNo);
                            createField = continueConfirmation == DialogResult.Yes;
                        }
                    }
                }

                if (createField) CreateField(optionSetValues);
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
        private void btn_ExportSQL_Click(object sender, EventArgs e)
        {
            using (var selectedFolder = new FolderBrowserDialog())
            {
                if (selectedFolder.ShowDialog() == DialogResult.OK)
                {
                    saveFilePath = selectedFolder.SelectedPath;
                    ExecuteMethod(ExportSQLScripts);
                }
            }
        }
        private void btn_GetValues_Click(object sender, EventArgs e)
        {
            lst_Values.Items.Clear();
            Table sourceTable = EntityFactory.TableList.Find(t => t.NAME == sourceField.TABLE_NAME);

            if (sourceTable != null)
            {
                btn_GetValues.Enabled = false;

                Dictionary<int, string> values = EntityFactory.GetDistinctValues(sourceField.COLUMN_NAME, sourceTable, null, string.Empty);
                foreach (KeyValuePair<int, string> kv in values)
                {
                    lst_Values.Items.Add(kv.Value);
                }
            }
        }
        private void btn_ImportCSV_Click(object sender, EventArgs e)
        {
            if (entities != null && EntityFactory.TableList != null)
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
                        foreach (KeyValuePair<string, List<Field>> kv in mappings)
                        {
                            foreach (Field field in kv.Value)
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
                        MessageBox.Show("Mappings imported successfully.", "CSV Import");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please load the source and destination tables before importing.", "CSV Import");
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
                if (mappings[destinationField.TABLE_NAME].Find(r => r.COLUMN_NAME == sourceField.COLUMN_NAME && r.TABLE_NAME == sourceField.TABLE_NAME) == null)
                {
                    sourceItemIdx = sourceTab.SelectedIndex;                    
                }
                else sourceItemIdx = sourceTab.SelectedIndex + 1;

                sourceField.MAPPED_FIELD = destinationField;

                if (!mappings.Keys.Contains(destinationField.TABLE_NAME))
                {
                    mappings.Add(destinationField.TABLE_NAME, new List<Field> { sourceField });
                }
                else mappings[destinationField.TABLE_NAME].Add(sourceField);

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
        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            if (destinationTab != null)
            {
                RefreshEntity(destinationTab.Name);
            }
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

                    mappings[sourceField.MAPPED_FIELD.TABLE_NAME].Remove(mappings[sourceField.MAPPED_FIELD.TABLE_NAME].Find(r => r.COLUMN_NAME == sourceField.COLUMN_NAME && r.MAPPED_FIELD.COLUMN_NAME == destinationFieldName));
                    btn_Unmap.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please select a source field to unmap.", "Missing Selection");
            }
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
        private void DestinationTab_Click(object sender, EventArgs e)
        {
            if (((TabControl)sender).SelectedIndex > -1)
            {
                destinationTab = (ListBox)((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Controls[0];
                destinationField = null;
            }
        }
        private void lst_Solutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_Solutions.CheckedItems.Count > 0)
            {
                btn_LoadSolutionEntities.Enabled = true;

                box_Solution.Items.Clear();
                foreach (int idx in lst_Solutions.CheckedIndices)
                {
                    if (solutions.Find(s => s["uniquename"].ToString() == lst_Solutions.Items[idx].ToString() && s.GetAttributeValue<bool>("ismanaged") == false) != null)
                    {
                        box_Solution.Items.Add(lst_Solutions.Items[idx]);
                        // set selected solution for new components to found unmanaged solution
                        box_Solution.SelectedIndex = idx;
                    }
                }
            }
            else
            {
                btn_LoadSolutionEntities.Enabled = false;
            }
        }
        private void sourceTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((TabControl)sender).SelectedIndex > -1)
            {
                sourceTab = (ListBox)((TabControl)sender).TabPages[((TabControl)sender).SelectedIndex].Controls[0];
                sourceField = null;

                txt_TableDisplayName.Text = sourceTab.Name;
                txt_PrimaryDisplayName.Text = "Name";
            }
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

                lst_Values.Items.Clear();
                btn_GetValues.Enabled = true;

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

                ExecuteMethod(ToggleFieldInputs);
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

            if (box_Target.SelectedItem != null && box_Table.SelectedItem != null && txt_FieldName.Text != string.Empty)
            {
                txt_RelationshipName.Text = box_Target.SelectedItem.ToString() + "_" + box_Table.SelectedItem.ToString() + "_" + txt_FieldName.Text;
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
                txt_tableDescription.Text = "A table to hold " + txt_TableDisplayName.Text + " records.";

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
            return txt_FieldDisplayName.Text != string.Empty && txt_FieldName.Text != string.Empty && box_DataType.Text != string.Empty && box_Table.Text != string.Empty && box_Requirement.SelectedItem != null
                && ((box_Target.SelectedItem != null && txt_RelationshipName.Text != string.Empty) || box_DataType.SelectedItem.ToString() != "Lookup")
                && ((box_Table.SelectedItem.ToString() != "account" && box_Table.SelectedItem.ToString() != "contact") || box_DataType.SelectedItem.ToString() != "Customer");
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
                    LogicalName = tableName,
                    RetrieveAsIfPublished = true
                };

                if ((Service.Execute(req) as RetrieveEntityResponse).EntityMetadata != null)
                {
                    foreach (AttributeMetadata am in (Service.Execute(req) as RetrieveEntityResponse).EntityMetadata.Attributes)
                    {
                        if (am.LogicalName == (prefix + fieldName).ToLower())
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
        private void CreateField(CheckedItemCollection optionSetValues)
        {
            string table = box_Table.SelectedItem.ToString();
            string fieldLabel = txt_FieldDisplayName.Text;
            string fieldSchema = txt_FieldName.Text;
            string dataType = box_DataType.SelectedItem.ToString();
            int length = (int)num_FieldLength.Value;
            int required = box_Requirement.SelectedIndex == 0 ? 0 : box_Requirement.SelectedIndex + 1;
            EntityMetadata tableEM = null;
            EntityMetadata targetEM = null;
            string relationshipName = string.Empty;

            string targetSolution = entities.Keys.ToList().Find(s => entities[s].ToList().Find(e => e.LogicalName == table) != null);
            if (targetSolution != null)
            {
                tableEM = entities[targetSolution].ToList().Find(e => e.LogicalName == table);
            }
            else
            {
                MessageBox.Show("Unable to find encompassing solution from solutions list.");
            }

            if (box_Target.SelectedItem != null)
            {
                targetSolution = entities.Keys.ToList().Find(s => entities[s].ToList().Find(e => e.LogicalName == box_Target.SelectedItem.ToString()) != null);
                if (targetSolution != null)
                {
                    targetEM = entities[targetSolution].ToList().Find(e => e.LogicalName == box_Target.SelectedItem.ToString());
                    relationshipName = txt_RelationshipName.Text;
                }
                else
                {
                    MessageBox.Show("Unable to find encompassing solution from solutions list.");
                }
            }

            if (!CheckFieldExists(table, fieldSchema))
            {
                btn_CreateField.Enabled = false;
                ExecuteMethod(DisableInputs);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Creating attribute...",
                    Work = (w, e) =>
                    {
                        // This code is executed in another thread
                        EntityFactory.CreateTableAttribute(solution, prefix, optionValuePrefix, table, fieldLabel, fieldSchema, dataType, length, optionSetValues, required, tableEM, targetEM, relationshipName, languageCode);

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
                MessageBox.Show("There is already a field with the name " + prefix+fieldSchema + " on the " + table + " table.");
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
            if (GetEntityMetaData(prefix + tableName.ToLower()) == null)
            {
                btn_CreateTable.Enabled = false;
                ExecuteMethod(DisableInputs);

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Creating entity...",
                    Work = (w, e) =>
                    {
                        // This code is executed in another thread
                        EntityFactory.CreateTable(solution, prefix, tableDisplayName, tablePluralName, tableName, tableDesc, primaryDisplayName, primaryName, primaryLength, primaryReq, languageCode);

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
                        EntityMetadata resultEntity = GetEntityMetaData(prefix + tableName.ToLower()); // Retrieve newly created entity
                        tableToPublish = resultEntity.LogicalName;

                        btn_PublishEntity.Enabled = true;
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
            grp_DestinationSettings.Enabled = false;
            grp_FieldCreation.Enabled = false;
            grp_EntityCreation.Enabled = false;

            btn_Refresh.Enabled = false;
        }
        private void EnableInputs()
        {
            grp_DestinationSettings.Enabled = true;
            grp_FieldCreation.Enabled = true;
            grp_EntityCreation.Enabled = true;

            btn_Refresh.Enabled = true;
        }
        private void ExportExcel(bool includeUnmapped)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Exporting to Excel...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.ExportMappingToExcel(saveFilePath, includeUnmapped);

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
        private void ExportSQLScripts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Generating scripts...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.ExportSQLScripts(mappings, saveFilePath);

                    w.ReportProgress(-1, "Scripts saved successfully.");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    MessageBox.Show(string.Format("Files saved to {0}", saveFilePath), "Files saved successfully");
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private EntityMetadata GetEntityMetaData(string tableName)
        {
            try
            {
                RetrieveEntityRequest req = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = tableName,
                    RetrieveAsIfPublished = true
                };
                return (Service.Execute(req) as RetrieveEntityResponse).EntityMetadata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void GetOrganizationLanguages()
        {
            var req = new RetrieveProvisionedLanguagesRequest();
            var res = (RetrieveProvisionedLanguagesResponse)Service.Execute(req);

            string language;
            foreach (int code in res.RetrieveProvisionedLanguages)
            {
                language = languages[code] + " ("+code+")";
                box_Language.Items.Add(language);
            }

            if (res.RetrieveProvisionedLanguages.Length > 0)
            {
                box_Language.SelectedIndex = 0;
            }
        }
        private void GetPublisher(Guid publisherId)
        {
            QueryByAttribute query = new QueryByAttribute
            {
                EntityName = "publisher",
                ColumnSet = new ColumnSet("customizationprefix", "customizationoptionvalueprefix")
            };

            query.Attributes.Add("publisherid");
            query.Values.Add(publisherId);

            try
            {
                EntityCollection retrievedPublishers = Service.RetrieveMultiple(query);

                if (retrievedPublishers.Entities.Count > 0)
                {
                    prefix = retrievedPublishers.Entities[0]["customizationprefix"].ToString() + "_";
                    optionValuePrefix = int.Parse(retrievedPublishers.Entities[0]["customizationoptionvalueprefix"].ToString());
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void GetSolutions()
        {
            lst_Solutions.Items.Clear();
            solutions = new List<Entity>();

            QueryExpression query = new QueryExpression
            {
                EntityName = "solution",
                ColumnSet = new ColumnSet("uniquename", "publisherid", "ismanaged"),
                Criteria = new FilterExpression()
            };

            LinkEntity publisherLink = new LinkEntity("solution", "publisher", "publisherid", "publisherid", JoinOperator.Inner)
            {
                LinkCriteria = new FilterExpression()
            };

            publisherLink.LinkCriteria.AddCondition(new ConditionExpression("customizationprefix", ConditionOperator.NotEqual, "new"));
            publisherLink.LinkCriteria.AddCondition(new ConditionExpression("customizationprefix", ConditionOperator.NotEqual, "cr7d6"));
            publisherLink.LinkCriteria.AddCondition(new ConditionExpression("customizationprefix", ConditionOperator.NotEqual, "msdyn"));
            publisherLink.LinkCriteria.AddCondition(new ConditionExpression("customizationprefix", ConditionOperator.NotEqual, "msdynce"));
            publisherLink.LinkCriteria.AddCondition(new ConditionExpression("customizationprefix", ConditionOperator.NotEqual, string.Empty));
            query.LinkEntities.Add(publisherLink);

            try
            {
                foreach (Entity solution in Service.RetrieveMultiple(query).Entities)
                {
                    solutions.Add(solution);
                    lst_Solutions.Items.Add(solution["uniquename"].ToString());
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
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
                    TabControl tabControl;
                    TabPage solutionTabPage, tableTabPage;
                    ListBox box;
                    object fieldObj;

                    destinationTabControl.TabPages.Clear();
                    box_Table.Items.Clear();
                    box_Target.Items.Clear();

                    int tabIdx = 0;
                    foreach (KeyValuePair<string, IEnumerable<EntityMetadata>> kv in entities)
                    {
                        solutionTabPage = new TabPage
                        {
                            Name = kv.Key,
                            Text = kv.Key,
                            BorderStyle = BorderStyle.None,
                            Height = 434,
                            Width = 9999,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                        };

                        destinationTabControl.TabPages.Add(solutionTabPage);

                        tabControl = new TabControl
                        {
                            Name = kv.Key,
                            Appearance = TabAppearance.FlatButtons,
                            Height = 434,
                            Width = 9999,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                        };

                        tabControl.SelectedIndexChanged += new EventHandler(DestinationTab_Click);

                        solutionTabPage.Controls.Add(tabControl);

                        foreach (EntityMetadata entity in kv.Value)
                        {
                            tableTabPage = new TabPage
                            {
                                Name = entity.LogicalName,
                                Text = entity.LogicalName,
                                BorderStyle = BorderStyle.None
                            };

                            tabControl.TabPages.Add(tableTabPage);

                            box = new ListBox
                            {
                                Name = tableTabPage.Name,
                                SelectionMode = SelectionMode.One,
                                Height = 434,
                                Width = 9999,
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                            };

                            box.SelectedIndexChanged += new EventHandler(DestinationField_Click);

                            foreach (AttributeMetadata field in entity.Attributes)
                            {
                                field.AttributeTypeName.Value = field.AttributeTypeName.Value.Replace("Type", string.Empty);
                                fieldObj = field.LogicalName + " (" + field.AttributeTypeName.Value + ")";
                                box.Items.Add(fieldObj);
                            }

                            tableTabPage.Controls.Add(box);
                            box_Table.Items.Add(entity.LogicalName);
                            box_Target.Items.Add(entity.LogicalName);                            

                            if (tabIdx == 0)
                            {
                                destinationTab = box;
                                tabIdx++;
                            }
                        }
                    }

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
            mappings = new Dictionary<string, List<Field>>();
            schemaFormat = "PascalCase";
            languages = new Dictionary<int, string>{
                {1078,"Afrikaans – South Africa"},
                {1052,"Albanian – Albania"},
                {1118,"Amharic – Ethiopia"},
                {5121,"Arabic – Algeria"},
                {15361,"Arabic – Bahrain"},
                {3073,"Arabic – Egypt"},
                {2049,"Arabic – Iraq"},
                {11265,"Arabic – Jordan"},
                {13313,"Arabic – Kuwait"},
                {12289,"Arabic – Lebanon"},
                {4097,"Arabic – Libya"},
                {6145,"Arabic – Morocco"},
                {8193,"Arabic – Oman"},
                {16385,"Arabic – Qatar"},
                {1025,"Arabic – Saudi Arabia"},
                {10241,"Arabic – Syria"},
                {7169,"Arabic – Tunisia"},
                {14337,"Arabic – U.A.E."},
                {9217,"Arabic – Yemen"},
                {1067,"Armenian – Armenia"},
                {1101,"Assamese"},
                {2092,"Azeri (Cyrillic)"},
                {1068,"Azeri (Latin)"},
                {1069,"Basque"},
                {1059,"Belarusian"},
                {2117,"Bengali (Bangladesh)"},
                {1093,"Bengali (India)"},
                {5146,"Bosnian (Bosnia/Herzegovina)"},
                {1026,"Bulgarian"},
                {1109,"Burmese"},
                {1027,"Catalan"},
                {1116,"Cherokee – United States"},
                {3076,"Chinese – Hong Kong SAR"},
                {5124,"Chinese – Macao SAR"},
                {2052,"Chinese – People’s Republic of China"},
                {4100,"Chinese – Singapore"},
                {1028,"Chinese – Taiwan"},
                {1050,"Croatian"},
                {4122,"Croatian (Bosnia/Herzegovina)"},
                {1029,"Czech"},
                {1030,"Danish"},
                {1125,"Divehi"},
                {2067,"Dutch – Belgium"},
                {1043,"Dutch – Netherlands"},
                {1126,"Edo"},
                {3081,"English – Australia"},
                {10249,"English – Belize"},
                {4105,"English – Canada"},
                {9225,"English – Caribbean"},
                {15369,"English – Hong Kong SAR"},
                {16393,"English – India"},
                {14345,"English – Indonesia"},
                {6153,"English – Ireland"},
                {8201,"English – Jamaica"},
                {17417,"English – Malaysia"},
                {5129,"English – New Zealand"},
                {13321,"English – Philippines"},
                {18441,"English – Singapore"},
                {7177,"English – South Africa"},
                {11273,"English – Trinidad"},
                {2057,"English – United Kingdom"},
                {1033,"English – United States"},
                {12297,"English – Zimbabwe"},
                {1061,"Estonian"},
                {1071,"F.Y.R.O. Macedonian"},
                {1080,"Faroese"},
                {1124,"Filipino"},
                {1035,"Finnish"},
                {2060,"French – Belgium"},
                {11276,"French – Cameroon"},
                {3084,"French – Canada"},
                {12300,"French – Cote d’Ivoire"},
                {9228,"French – Democratic Rep. of Congo"},
                {1036,"French – France"},
                {15372,"French – Haiti"},
                {5132,"French – Luxembourg"},
                {13324,"French – Mali"},
                {6156,"French – Monaco"},
                {14348,"French – Morocco"},
                {58380,"French – North Africa"},
                {8204,"French – Reunion"},
                {10252,"French – Senegal"},
                {4108,"French – Switzerland"},
                {7180,"French – West Indies"},
                {1122,"Frisian – Netherlands"},
                {1127,"Fulfulde – Nigeria"},
                {2108,"Gaelic (Ireland)"},
                {1084,"Gaelic (Scotland)"},
                {1110,"Galician"},
                {1079,"Georgian"},
                {3079,"German – Austria"},
                {1031,"German – Germany"},
                {5127,"German – Liechtenstein"},
                {4103,"German – Luxembourg"},
                {2055,"German – Switzerland"},
                {1032,"Greek"},
                {1140,"Guarani – Paraguay"},
                {1095,"Gujarati"},
                {1128,"Hausa – Nigeria"},
                {1141,"Hawaiian – United States"},
                {1037,"Hebrew"},
                {1081,"Hindi"},
                {1038,"Hungarian"},
                {1129,"Ibibio – Nigeria"},
                {1039,"Icelandic"},
                {1136,"Igbo – Nigeria"},
                {1057,"Indonesian"},
                {1117,"Inuktitut"},
                {1040,"Italian – Italy"},
                {2064,"Italian – Switzerland"},
                {1041,"Japanese"},
                {1099,"Kannada"},
                {1137,"Kanuri – Nigeria"},
                {1120,"Kashmiri (Arabic)"},
                {2144,"Kashmiri (Devanagari)"},
                {1087,"Kazakh"},
                {1107,"Khmer"},
                {1111,"Konkani"},
                {1042,"Korean"},
                {1088,"Kyrgyz (Cyrillic)"},
                {1108,"Lao"},
                {1142,"Latin"},
                {1062,"Latvian"},
                {1063,"Lithuanian"},
                {2110,"Malay – Brunei Darussalam"},
                {1086,"Malay – Malaysia"},
                {1100,"Malayalam"},
                {1082,"Maltese"},
                {1112,"Manipuri"},
                {1153,"Maori – New Zealand"},
                {1102,"Marathi"},
                {1104,"Mongolian (Cyrillic)"},
                {2128,"Mongolian (Mongolian)"},
                {1121,"Nepali"},
                {2145,"Nepali – India"},
                {1044,"Norwegian (Bokmål)"},
                {2068,"Norwegian (Nynorsk)"},
                {1096,"Oriya"},
                {1138,"Oromo"},
                {1145,"Papiamentu"},
                {1123,"Pashto"},
                {1065,"Persian"},
                {1045,"Polish"},
                {1046,"Portuguese – Brazil"},
                {2070,"Portuguese – Portugal"},
                {1094,"Punjabi"},
                {2118,"Punjabi (Pakistan)"},
                {1131,"Quecha – Bolivia"},
                {2155,"Quecha – Ecuador"},
                {3179,"Quecha – Peru"},
                {1047,"Rhaeto-Romanic"},
                {1048,"Romanian"},
                {2072,"Romanian – Moldava"},
                {1049,"Russian"},
                {2073,"Russian – Moldava"},
                {1083,"Sami"},
                {1103,"Sanskrit"},
                {1132,"Sepedi"},
                {3098,"Serbian (Cyrillic)"},
                {2074,"Serbian (Latin)"},
                {1113,"Sindhi – India"},
                {2137,"Sindhi – Pakistan"},
                {1115,"Sinhalese – Sri Lanka"},
                {1051,"Slovak"},
                {1060,"Slovenian"},
                {1143,"Somali"},
                {1070,"Sorbian"},
                {11274,"Spanish – Argentina"},
                {16394,"Spanish – Bolivia"},
                {13322,"Spanish – Chile"},
                {9226,"Spanish – Colombia"},
                {5130,"Spanish – Costa Rica"},
                {7178,"Spanish – Dominican Republic"},
                {12298,"Spanish – Ecuador"},
                {17418,"Spanish – El Salvador"},
                {4106,"Spanish – Guatemala"},
                {18442,"Spanish – Honduras"},
                {58378,"Spanish – Latin America"},
                {2058,"Spanish – Mexico"},
                {19466,"Spanish – Nicaragua"},
                {6154,"Spanish – Panama"},
                {15370,"Spanish – Paraguay"},
                {10250,"Spanish – Peru"},
                {20490,"Spanish – Puerto Rico"},
                {3082,"Spanish – Spain (Modern Sort)"},
                {1034,"Spanish – Spain (Traditional Sort)"},
                {21514,"Spanish – United States"},
                {14346,"Spanish – Uruguay"},
                {8202,"Spanish – Venezuela"},
                {1072,"Sutu"},
                {1089,"Swahili"},
                {1053,"Swedish"},
                {2077,"Swedish – Finland"},
                {1114,"Syriac"},
                {1064,"Tajik"},
                {1119,"Tamazight (Arabic)"},
                {2143,"Tamazight (Latin)"},
                {1097,"Tamil"},
                {1092,"Tatar"},
                {1098,"Telugu"},
                {1054,"Thai"},
                {2129,"Tibetan – Bhutan"},
                {1105,"Tibetan – People’s Republic of China"},
                {2163,"Tigrigna – Eritrea"},
                {1139,"Tigrigna – Ethiopia"},
                {1073,"Tsonga"},
                {1074,"Tswana"},
                {1055,"Turkish"},
                {1090,"Turkmen"},
                {1152,"Uighur – China"},
                {1058,"Ukrainian"},
                {2080,"Urdu – India"},
                {1056,"Urdu – Pakistan"},
                {2115,"Uzbek (Cyrillic)"},
                {1091,"Uzbek (Latin)"},
                {1075,"Venda"},
                {1066,"Vietnamese"},
                {1106,"Welsh"},
                {1076,"Xhosa"},
                {1144,"Yi"},
                {1085,"Yiddish"},
                {1130,"Yoruba"},
                {1077,"Zulu"}
            };
            ExecuteMethod(GetOrganizationLanguages);

            ExecuteMethod(GetSolutions);
        }
        private void PublishCustomizations()
        {
            btn_PublishEntity.Enabled = false;
            btn_PublishField.Enabled = false;
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
                            Text = table.NAME,
                            BorderStyle = BorderStyle.None
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
                    ExecuteMethod(EnableInputs);
                },
                AsyncArgument = null,
                // Progress information panel size
                MessageWidth = 340,
                MessageHeight = 150
            });
        }
        private void RefreshEntity(string entityName)
        {
            EntityMetadata entity = null;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Refreshing entity...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    entity = GetEntityMetaData(entityName);

                    w.ReportProgress(-1, "Entity refreshed successfully");
                    e.Result = 1;
                },
                ProgressChanged = e =>
                {
                    SetWorkingMessage(e.UserState.ToString());
                },
                PostWorkCallBack = e =>
                {
                    // This code is executed in the main thread
                    if (entity != null)
                    {
                        destinationTab.Items.Clear();
                        foreach (AttributeMetadata field in entity.Attributes)
                        {
                            destinationTab.Items.Add(field.LogicalName + " (" + field.AttributeTypeName.Value.Replace("Type", string.Empty) + ")");
                        }
                    }
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
        private void SaveCSV()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Saving mappings...",
                Work = (w, e) =>
                {
                    // This code is executed in another thread
                    EntityFactory.ExportMappingToCSV(saveFilePath);

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
            if (box_Solution.SelectedItem != null)
            {
                Entity selectedSolution = solutions.Find(s => s.GetAttributeValue<string>("uniquename") == box_Solution.Text);

                if (selectedSolution != null)
                {
                    box_Table.Items.Clear();
                    box_Target.Items.Clear();
                    solution = selectedSolution.GetAttributeValue<string>("uniquename");
                    GetPublisher(selectedSolution.GetAttributeValue<EntityReference>("publisherid").Id);

                    if (entities != null && entities.Keys.Contains(selectedSolution.GetAttributeValue<string>("uniquename")))
                    {
                        foreach (EntityMetadata entity in entities[selectedSolution.GetAttributeValue<string>("uniquename")])
                        {
                            box_Table.Items.Add(entity.LogicalName);
                            box_Target.Items.Add(entity.LogicalName);
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
        private void ToggleFieldInputs()
        {
            lbl_FieldLength.Visible = false;
            num_FieldLength.Visible = false;

            lbl_MinValue.Visible = false;
            num_MinValue.Visible = false;
            lbl_MaxValue.Visible = false;
            num_MaxValue.Visible = false;

            lbl_Target.Visible = false;
            box_Target.Visible = false;
            lbl_RelationshipName.Visible = false;
            txt_RelationshipName.Visible = false;

            switch (box_DataType.Text)
            {
                case "Single Line of Text":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 4000;
                    num_FieldLength.Value = 100;
                    break;
                case "Multiple Lines of Text":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 1048576;
                    num_FieldLength.Value = 2000;
                    break;
                case "Date Only":
                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 0;
                    num_FieldLength.Value = 0;
                    break;
                case "Date and Time":
                    num_FieldLength.Minimum = 1;
                    num_FieldLength.Maximum = 1;
                    num_FieldLength.Value = 1;
                    break;
                case "Whole Number":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    lbl_MinValue.Visible = true;
                    num_MinValue.Visible = true;
                    lbl_MaxValue.Visible = true;
                    num_MaxValue.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 0;
                    num_FieldLength.Value = 0;

                    num_MinValue.Minimum = -2147483648;
                    num_MinValue.Maximum = 2147483647;
                    num_MinValue.Value = -2147483648;

                    num_MaxValue.Minimum = -2147483648;
                    num_MaxValue.Maximum = 2147483647;
                    num_MaxValue.Value = 2147483647;
                    break;
                case "Decimal Number":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    lbl_MinValue.Visible = true;
                    num_MinValue.Visible = true;
                    lbl_MaxValue.Visible = true;
                    num_MaxValue.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 10;
                    num_FieldLength.Value = 2;

                    num_MinValue.Minimum = -100000000000;
                    num_MinValue.Maximum = 100000000000;
                    num_MinValue.Value = -100000000000;

                    num_MaxValue.Minimum = -100000000000;
                    num_MaxValue.Maximum = 100000000000;
                    num_MaxValue.Value = 100000000000;
                    break;
                case "Floating Point Number":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    lbl_MinValue.Visible = true;
                    num_MinValue.Visible = true;
                    lbl_MaxValue.Visible = true;
                    num_MaxValue.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 5;
                    num_FieldLength.Value = 2;

                    num_MinValue.Minimum = -100000000000;
                    num_MinValue.Maximum = 100000000000;
                    num_MinValue.Value = 0;

                    num_MaxValue.Minimum = -100000000000;
                    num_MaxValue.Maximum = 100000000000;
                    num_MaxValue.Value = 100000000000;
                    break;
                case "Currency":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    lbl_MinValue.Visible = true;
                    num_MinValue.Visible = true;
                    lbl_MaxValue.Visible = true;
                    num_MaxValue.Visible = true;

                    num_FieldLength.Minimum = 0;
                    num_FieldLength.Maximum = 4;
                    num_FieldLength.Value = 2;

                    num_MinValue.Minimum = -922337203685477;
                    num_MinValue.Maximum = 922337203685477;
                    num_MinValue.Value = -922337203685477;

                    num_MaxValue.Minimum = -922337203685477;
                    num_MaxValue.Maximum = 922337203685477;
                    num_MaxValue.Value = 922337203685477;
                    break;
                case "Image":
                case "File":
                    lbl_FieldLength.Visible = true;
                    num_FieldLength.Visible = true;

                    num_FieldLength.Minimum = 1;
                    num_FieldLength.Maximum = 131072;
                    num_FieldLength.Value = 32768;
                    break;
                case "Lookup":
                    lbl_Target.Visible = true;
                    box_Target.Visible = true;
                    lbl_RelationshipName.Visible = true;
                    txt_RelationshipName.Visible = true;
                    break;
                default: break;
            }

            if (CanCreateField()) btn_CreateField.Enabled = true;

            if (box_Table.SelectedItem != null && box_DataType.SelectedItem != null
                && (box_Table.SelectedItem.ToString() == "account" || box_Table.SelectedItem.ToString() == "contact") && box_DataType.SelectedItem.ToString() == "Customer")
            {
                MessageBox.Show("Polymorphic lookups cannot be self-referential.");
            }
        }
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            ExecuteMethod(GetSolutions);
        }
    }
}