namespace MigrationAssistant
{
    partial class MigrationAssistantControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MigrationAssistantControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.btn_Close = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_SaveCSV = new System.Windows.Forms.ToolStripButton();
            this.btn_ImportCSV = new System.Windows.Forms.ToolStripButton();
            this.btn_ExportExcel = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.box_Server = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.box_TableSchema = new System.Windows.Forms.TextBox();
            this.box_IgnoreEmpty = new System.Windows.Forms.CheckBox();
            this.box_InspectData = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.box_Mode = new System.Windows.Forms.ComboBox();
            this.box_Database = new System.Windows.Forms.TextBox();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.connectionSettings = new System.Windows.Forms.GroupBox();
            this.sourceTabControl = new System.Windows.Forms.TabControl();
            this.fieldCreation = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_FieldName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.box_Table = new System.Windows.Forms.ComboBox();
            this.num_FieldLength = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_FieldDisplayName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.box_DataType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.box_Requirement = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.btn_PublishField = new System.Windows.Forms.Button();
            this.btn_CreateField = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.box_Solution = new System.Windows.Forms.ComboBox();
            this.box_Format = new System.Windows.Forms.ComboBox();
            this.destinationTabControl = new System.Windows.Forms.TabControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.lst_Solutions = new System.Windows.Forms.CheckedListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_LoadSolutionEntities = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_TableName = new System.Windows.Forms.TextBox();
            this.btn_PublishEntity = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txt_TableDisplayName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txt_TablePluralName = new System.Windows.Forms.TextBox();
            this.txt_tableDescription = new System.Windows.Forms.TextBox();
            this.btn_CreateTable = new System.Windows.Forms.Button();
            this.num_PrimaryLength = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.box_PrimaryRequirement = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txt_PrimaryDisplayName = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txt_PrimaryName = new System.Windows.Forms.TextBox();
            this.btn_Unmap = new System.Windows.Forms.Button();
            this.btn_Map = new System.Windows.Forms.Button();
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.connectionSettings.SuspendLayout();
            this.fieldCreation.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_FieldLength)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PrimaryLength)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Close,
            this.toolStripSeparator1,
            this.btn_SaveCSV,
            this.btn_ImportCSV,
            this.btn_ExportExcel});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripMenu.Size = new System.Drawing.Size(2381, 66);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // btn_Close
            // 
            this.btn_Close.Image = global::MigrationAssistant.Properties.Resources.cross1;
            this.btn_Close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Padding = new System.Windows.Forms.Padding(2);
            this.btn_Close.Size = new System.Drawing.Size(63, 57);
            this.btn_Close.Text = "Close";
            this.btn_Close.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 62);
            // 
            // btn_SaveCSV
            // 
            this.btn_SaveCSV.Image = global::MigrationAssistant.Properties.Resources.save1;
            this.btn_SaveCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_SaveCSV.Name = "btn_SaveCSV";
            this.btn_SaveCSV.Padding = new System.Windows.Forms.Padding(2);
            this.btn_SaveCSV.Size = new System.Drawing.Size(57, 57);
            this.btn_SaveCSV.Text = "Save";
            this.btn_SaveCSV.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btn_SaveCSV.Click += new System.EventHandler(this.btn_SaveCSV_Click);
            // 
            // btn_ImportCSV
            // 
            this.btn_ImportCSV.Image = global::MigrationAssistant.Properties.Resources.csv_file_format_extension1;
            this.btn_ImportCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_ImportCSV.Name = "btn_ImportCSV";
            this.btn_ImportCSV.Padding = new System.Windows.Forms.Padding(2);
            this.btn_ImportCSV.Size = new System.Drawing.Size(75, 57);
            this.btn_ImportCSV.Text = "Import";
            this.btn_ImportCSV.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btn_ImportCSV.Click += new System.EventHandler(this.btn_ImportCSV_Click);
            // 
            // btn_ExportExcel
            // 
            this.btn_ExportExcel.Image = global::MigrationAssistant.Properties.Resources.Excel1;
            this.btn_ExportExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_ExportExcel.Name = "btn_ExportExcel";
            this.btn_ExportExcel.Padding = new System.Windows.Forms.Padding(2);
            this.btn_ExportExcel.Size = new System.Drawing.Size(71, 57);
            this.btn_ExportExcel.Text = "Export";
            this.btn_ExportExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btn_ExportExcel.Click += new System.EventHandler(this.btn_ExportExcel_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 34);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 23);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.box_Server, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.box_TableSchema, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.box_IgnoreEmpty, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.box_InspectData, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.box_Mode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.box_Database, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btn_Connect, 0, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 29);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(693, 223);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // box_Server
            // 
            this.box_Server.Location = new System.Drawing.Point(4, 25);
            this.box_Server.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_Server.Name = "box_Server";
            this.box_Server.Size = new System.Drawing.Size(427, 26);
            this.box_Server.TabIndex = 0;
            this.box_Server.Text = resources.GetString("box_Server.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server Connection String";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(439, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Table/Schema";
            // 
            // box_TableSchema
            // 
            this.box_TableSchema.Location = new System.Drawing.Point(439, 83);
            this.box_TableSchema.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_TableSchema.Name = "box_TableSchema";
            this.box_TableSchema.Size = new System.Drawing.Size(247, 26);
            this.box_TableSchema.TabIndex = 9;
            this.box_TableSchema.Text = "dbo";
            // 
            // box_IgnoreEmpty
            // 
            this.box_IgnoreEmpty.AutoSize = true;
            this.box_IgnoreEmpty.Location = new System.Drawing.Point(4, 119);
            this.box_IgnoreEmpty.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_IgnoreEmpty.Name = "box_IgnoreEmpty";
            this.box_IgnoreEmpty.Size = new System.Drawing.Size(238, 24);
            this.box_IgnoreEmpty.TabIndex = 11;
            this.box_IgnoreEmpty.Text = "Ignore empty tables/columns";
            this.box_IgnoreEmpty.UseVisualStyleBackColor = true;
            // 
            // box_InspectData
            // 
            this.box_InspectData.AutoSize = true;
            this.box_InspectData.Location = new System.Drawing.Point(439, 119);
            this.box_InspectData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_InspectData.Name = "box_InspectData";
            this.box_InspectData.Size = new System.Drawing.Size(189, 24);
            this.box_InspectData.TabIndex = 12;
            this.box_InspectData.Text = "Allow Data Inspection";
            this.box_InspectData.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(439, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Mode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database";
            // 
            // box_Mode
            // 
            this.box_Mode.FormattingEnabled = true;
            this.box_Mode.Items.AddRange(new object[] {
            "Table",
            "Schema",
            "Database"});
            this.box_Mode.Location = new System.Drawing.Point(439, 25);
            this.box_Mode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_Mode.Name = "box_Mode";
            this.box_Mode.Size = new System.Drawing.Size(247, 28);
            this.box_Mode.TabIndex = 6;
            this.box_Mode.Text = "Schema";
            // 
            // box_Database
            // 
            this.box_Database.Location = new System.Drawing.Point(4, 83);
            this.box_Database.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_Database.Name = "box_Database";
            this.box_Database.Size = new System.Drawing.Size(427, 26);
            this.box_Database.TabIndex = 1;
            this.box_Database.Text = "JusticeNexus_Maricopa";
            // 
            // btn_Connect
            // 
            this.btn_Connect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.SetColumnSpan(this.btn_Connect, 2);
            this.btn_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Connect.Location = new System.Drawing.Point(4, 153);
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Connect.Name = "btn_Connect";
            this.tableLayoutPanel1.SetRowSpan(this.btn_Connect, 2);
            this.btn_Connect.Size = new System.Drawing.Size(682, 54);
            this.btn_Connect.TabIndex = 4;
            this.btn_Connect.Text = "Connect/Read Database";
            this.btn_Connect.UseVisualStyleBackColor = false;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // connectionSettings
            // 
            this.connectionSettings.Controls.Add(this.tableLayoutPanel1);
            this.connectionSettings.Location = new System.Drawing.Point(4, 75);
            this.connectionSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectionSettings.Name = "connectionSettings";
            this.connectionSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectionSettings.Size = new System.Drawing.Size(716, 263);
            this.connectionSettings.TabIndex = 8;
            this.connectionSettings.TabStop = false;
            this.connectionSettings.Text = "Source Settings";
            // 
            // sourceTabControl
            // 
            this.sourceTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.sourceTabControl.Location = new System.Drawing.Point(9, 28);
            this.sourceTabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sourceTabControl.Name = "sourceTabControl";
            this.sourceTabControl.SelectedIndex = 0;
            this.sourceTabControl.Size = new System.Drawing.Size(1613, 434);
            this.sourceTabControl.TabIndex = 8;
            this.sourceTabControl.SelectedIndexChanged += new System.EventHandler(this.sourceTabControl_SelectedIndexChanged);
            // 
            // fieldCreation
            // 
            this.fieldCreation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldCreation.Controls.Add(this.tableLayoutPanel2);
            this.fieldCreation.Location = new System.Drawing.Point(1644, 75);
            this.fieldCreation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fieldCreation.Name = "fieldCreation";
            this.fieldCreation.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fieldCreation.Size = new System.Drawing.Size(710, 263);
            this.fieldCreation.TabIndex = 9;
            this.fieldCreation.TabStop = false;
            this.fieldCreation.Text = "Field Creation";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.txt_FieldName, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.box_Table, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.num_FieldLength, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.label9, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.txt_FieldDisplayName, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.box_DataType, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.box_Requirement, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label20, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.btn_PublishField, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.btn_CreateField, 0, 6);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(9, 29);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(687, 227);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // txt_FieldName
            // 
            this.txt_FieldName.Location = new System.Drawing.Point(347, 79);
            this.txt_FieldName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_FieldName.Name = "txt_FieldName";
            this.txt_FieldName.Size = new System.Drawing.Size(336, 26);
            this.txt_FieldName.TabIndex = 16;
            this.txt_FieldName.TextChanged += new System.EventHandler(this.txt_FieldName_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 20);
            this.label12.TabIndex = 19;
            this.label12.Text = "Table";
            // 
            // box_Table
            // 
            this.box_Table.Enabled = false;
            this.box_Table.FormattingEnabled = true;
            this.box_Table.Location = new System.Drawing.Point(3, 23);
            this.box_Table.Name = "box_Table";
            this.box_Table.Size = new System.Drawing.Size(337, 28);
            this.box_Table.TabIndex = 20;
            this.box_Table.SelectedIndexChanged += new System.EventHandler(this.box_Table_SelectedIndexChanged);
            // 
            // num_FieldLength
            // 
            this.num_FieldLength.Location = new System.Drawing.Point(346, 133);
            this.num_FieldLength.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.num_FieldLength.Name = "num_FieldLength";
            this.num_FieldLength.Size = new System.Drawing.Size(338, 26);
            this.num_FieldLength.TabIndex = 15;
            this.num_FieldLength.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(347, 110);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 20);
            this.label9.TabIndex = 11;
            this.label9.Text = "Max Length/Precision";
            // 
            // txt_FieldDisplayName
            // 
            this.txt_FieldDisplayName.Location = new System.Drawing.Point(4, 79);
            this.txt_FieldDisplayName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_FieldDisplayName.Name = "txt_FieldDisplayName";
            this.txt_FieldDisplayName.Size = new System.Drawing.Size(335, 26);
            this.txt_FieldDisplayName.TabIndex = 5;
            this.txt_FieldDisplayName.TextChanged += new System.EventHandler(this.txt_FieldDisplayName_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 54);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Display Name";
            // 
            // box_DataType
            // 
            this.box_DataType.FormattingEnabled = true;
            this.box_DataType.Items.AddRange(new object[] {
            "Single Line of Text",
            "Multiple Lines of Text",
            "Whole Number",
            "Date Only",
            "Date and Time"});
            this.box_DataType.Location = new System.Drawing.Point(346, 23);
            this.box_DataType.Name = "box_DataType";
            this.box_DataType.Size = new System.Drawing.Size(338, 28);
            this.box_DataType.TabIndex = 10;
            this.box_DataType.Text = "Single Line of Text";
            this.box_DataType.SelectedIndexChanged += new System.EventHandler(this.box_DataType_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(347, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Data Type";
            // 
            // box_Requirement
            // 
            this.box_Requirement.FormattingEnabled = true;
            this.box_Requirement.Items.AddRange(new object[] {
            "Optional",
            "Required",
            "Recommended"});
            this.box_Requirement.Location = new System.Drawing.Point(3, 133);
            this.box_Requirement.Name = "box_Requirement";
            this.box_Requirement.Size = new System.Drawing.Size(337, 28);
            this.box_Requirement.TabIndex = 7;
            this.box_Requirement.Text = "Optional";
            this.box_Requirement.SelectedIndexChanged += new System.EventHandler(this.box_Requirement_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 110);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Requirement";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(346, 54);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(51, 20);
            this.label20.TabIndex = 22;
            this.label20.Text = "Name";
            // 
            // btn_PublishField
            // 
            this.btn_PublishField.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_PublishField.Enabled = false;
            this.btn_PublishField.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_PublishField.Location = new System.Drawing.Point(347, 169);
            this.btn_PublishField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_PublishField.Name = "btn_PublishField";
            this.btn_PublishField.Size = new System.Drawing.Size(336, 34);
            this.btn_PublishField.TabIndex = 21;
            this.btn_PublishField.Text = "Publish Entity";
            this.btn_PublishField.UseVisualStyleBackColor = false;
            this.btn_PublishField.Click += new System.EventHandler(this.btn_PublishField_Click);
            // 
            // btn_CreateField
            // 
            this.btn_CreateField.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_CreateField.Enabled = false;
            this.btn_CreateField.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CreateField.Location = new System.Drawing.Point(4, 169);
            this.btn_CreateField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_CreateField.Name = "btn_CreateField";
            this.btn_CreateField.Size = new System.Drawing.Size(335, 34);
            this.btn_CreateField.TabIndex = 18;
            this.btn_CreateField.Text = "Create Field";
            this.btn_CreateField.UseVisualStyleBackColor = false;
            this.btn_CreateField.Click += new System.EventHandler(this.btn_CreateField_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(365, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(220, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Solution for New Components";
            // 
            // box_Solution
            // 
            this.box_Solution.Enabled = false;
            this.box_Solution.FormattingEnabled = true;
            this.box_Solution.Location = new System.Drawing.Point(364, 23);
            this.box_Solution.Name = "box_Solution";
            this.box_Solution.Size = new System.Drawing.Size(337, 28);
            this.box_Solution.TabIndex = 3;
            this.box_Solution.SelectedIndexChanged += new System.EventHandler(this.box_Solution_SelectedIndexChanged);
            // 
            // box_Format
            // 
            this.box_Format.FormattingEnabled = true;
            this.box_Format.Items.AddRange(new object[] {
            "camelCase",
            "lowercase",
            "PascalCase",
            "UPPERCASE"});
            this.box_Format.Location = new System.Drawing.Point(365, 79);
            this.box_Format.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.box_Format.Name = "box_Format";
            this.box_Format.Size = new System.Drawing.Size(337, 28);
            this.box_Format.TabIndex = 17;
            this.box_Format.Text = "PascalCase";
            this.box_Format.SelectedIndexChanged += new System.EventHandler(this.box_Format_SelectedIndexChanged);
            // 
            // destinationTabControl
            // 
            this.destinationTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.destinationTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.destinationTabControl.Location = new System.Drawing.Point(9, 28);
            this.destinationTabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.destinationTabControl.Name = "destinationTabControl";
            this.destinationTabControl.SelectedIndex = 0;
            this.destinationTabControl.Size = new System.Drawing.Size(1613, 434);
            this.destinationTabControl.TabIndex = 11;
            this.destinationTabControl.SelectedIndexChanged += new System.EventHandler(this.destinationTabControl_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.destinationTabControl);
            this.groupBox1.Location = new System.Drawing.Point(4, 913);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1633, 471);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Destination Entities";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.sourceTabControl);
            this.groupBox2.Location = new System.Drawing.Point(4, 348);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1633, 469);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Source Tables";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.Location = new System.Drawing.Point(913, 75);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(724, 263);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Destination Settings";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lst_Solutions, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label10, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.box_Format, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.box_Solution, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.btn_LoadSolutionEntities, 0, 5);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 29);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(708, 223);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(163, 20);
            this.label11.TabIndex = 0;
            this.label11.Text = "Solutions to Pull From";
            // 
            // lst_Solutions
            // 
            this.lst_Solutions.CheckOnClick = true;
            this.lst_Solutions.Enabled = false;
            this.lst_Solutions.FormattingEnabled = true;
            this.lst_Solutions.Location = new System.Drawing.Point(3, 23);
            this.lst_Solutions.Name = "lst_Solutions";
            this.tableLayoutPanel3.SetRowSpan(this.lst_Solutions, 4);
            this.lst_Solutions.Size = new System.Drawing.Size(355, 142);
            this.lst_Solutions.TabIndex = 4;
            this.lst_Solutions.SelectedIndexChanged += new System.EventHandler(this.lst_Solutions_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(365, 54);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(149, 20);
            this.label10.TabIndex = 17;
            this.label10.Text = "Schema Formatting";
            // 
            // btn_LoadSolutionEntities
            // 
            this.btn_LoadSolutionEntities.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_LoadSolutionEntities.Enabled = false;
            this.btn_LoadSolutionEntities.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_LoadSolutionEntities.Location = new System.Drawing.Point(3, 171);
            this.btn_LoadSolutionEntities.Name = "btn_LoadSolutionEntities";
            this.btn_LoadSolutionEntities.Size = new System.Drawing.Size(355, 36);
            this.btn_LoadSolutionEntities.TabIndex = 5;
            this.btn_LoadSolutionEntities.Text = "Load Entities";
            this.btn_LoadSolutionEntities.UseVisualStyleBackColor = false;
            this.btn_LoadSolutionEntities.Click += new System.EventHandler(this.btn_LoadSolutionEntities_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.tableLayoutPanel4);
            this.groupBox4.Location = new System.Drawing.Point(1644, 348);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(710, 351);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Entity Creation";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.txt_TableName, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.btn_PublishEntity, 1, 12);
            this.tableLayoutPanel4.Controls.Add(this.label14, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.txt_TableDisplayName, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.label13, 0, 6);
            this.tableLayoutPanel4.Controls.Add(this.label18, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.txt_TablePluralName, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.txt_tableDescription, 1, 5);
            this.tableLayoutPanel4.Controls.Add(this.btn_CreateTable, 0, 12);
            this.tableLayoutPanel4.Controls.Add(this.num_PrimaryLength, 1, 11);
            this.tableLayoutPanel4.Controls.Add(this.label17, 1, 10);
            this.tableLayoutPanel4.Controls.Add(this.box_PrimaryRequirement, 0, 11);
            this.tableLayoutPanel4.Controls.Add(this.label15, 0, 10);
            this.tableLayoutPanel4.Controls.Add(this.label16, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.label21, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.txt_PrimaryDisplayName, 0, 7);
            this.tableLayoutPanel4.Controls.Add(this.label19, 1, 6);
            this.tableLayoutPanel4.Controls.Add(this.txt_PrimaryName, 1, 7);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(9, 29);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 13;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(687, 312);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // txt_TableName
            // 
            this.txt_TableName.Location = new System.Drawing.Point(4, 81);
            this.txt_TableName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TableName.Name = "txt_TableName";
            this.txt_TableName.Size = new System.Drawing.Size(335, 26);
            this.txt_TableName.TabIndex = 21;
            this.txt_TableName.TextChanged += new System.EventHandler(this.txt_TableName_TextChanged);
            // 
            // btn_PublishEntity
            // 
            this.btn_PublishEntity.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_PublishEntity.Enabled = false;
            this.btn_PublishEntity.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_PublishEntity.Location = new System.Drawing.Point(347, 253);
            this.btn_PublishEntity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_PublishEntity.Name = "btn_PublishEntity";
            this.btn_PublishEntity.Size = new System.Drawing.Size(336, 34);
            this.btn_PublishEntity.TabIndex = 24;
            this.btn_PublishEntity.Text = "Publish Entity";
            this.btn_PublishEntity.UseVisualStyleBackColor = false;
            this.btn_PublishEntity.Click += new System.EventHandler(this.btn_PublishTable_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(106, 20);
            this.label14.TabIndex = 19;
            this.label14.Text = "Display Name";
            // 
            // txt_TableDisplayName
            // 
            this.txt_TableDisplayName.Location = new System.Drawing.Point(4, 25);
            this.txt_TableDisplayName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TableDisplayName.Name = "txt_TableDisplayName";
            this.txt_TableDisplayName.Size = new System.Drawing.Size(335, 26);
            this.txt_TableDisplayName.TabIndex = 20;
            this.txt_TableDisplayName.TextChanged += new System.EventHandler(this.txt_TableDisplayName_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 138);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(200, 20);
            this.label13.TabIndex = 4;
            this.label13.Text = "Primary Field Display Name";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(346, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(94, 20);
            this.label18.TabIndex = 22;
            this.label18.Text = "Plural Name";
            // 
            // txt_TablePluralName
            // 
            this.txt_TablePluralName.Location = new System.Drawing.Point(347, 25);
            this.txt_TablePluralName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TablePluralName.Name = "txt_TablePluralName";
            this.txt_TablePluralName.Size = new System.Drawing.Size(336, 26);
            this.txt_TablePluralName.TabIndex = 23;
            this.txt_TablePluralName.TextChanged += new System.EventHandler(this.txt_TablePluralName_TextChanged);
            // 
            // txt_tableDescription
            // 
            this.txt_tableDescription.Location = new System.Drawing.Point(347, 81);
            this.txt_tableDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_tableDescription.Multiline = true;
            this.txt_tableDescription.Name = "txt_tableDescription";
            this.txt_tableDescription.Size = new System.Drawing.Size(336, 52);
            this.txt_tableDescription.TabIndex = 21;
            // 
            // btn_CreateTable
            // 
            this.btn_CreateTable.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_CreateTable.Enabled = false;
            this.btn_CreateTable.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CreateTable.Location = new System.Drawing.Point(4, 253);
            this.btn_CreateTable.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_CreateTable.Name = "btn_CreateTable";
            this.btn_CreateTable.Size = new System.Drawing.Size(335, 34);
            this.btn_CreateTable.TabIndex = 18;
            this.btn_CreateTable.Text = "Create Entity";
            this.btn_CreateTable.UseVisualStyleBackColor = false;
            this.btn_CreateTable.Click += new System.EventHandler(this.btn_CreateTable_Click);
            // 
            // num_PrimaryLength
            // 
            this.num_PrimaryLength.Location = new System.Drawing.Point(346, 217);
            this.num_PrimaryLength.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.num_PrimaryLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_PrimaryLength.Name = "num_PrimaryLength";
            this.num_PrimaryLength.Size = new System.Drawing.Size(338, 26);
            this.num_PrimaryLength.TabIndex = 15;
            this.num_PrimaryLength.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(347, 194);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(186, 20);
            this.label17.TabIndex = 11;
            this.label17.Text = "Primary Field Max Length";
            // 
            // box_PrimaryRequirement
            // 
            this.box_PrimaryRequirement.FormattingEnabled = true;
            this.box_PrimaryRequirement.Items.AddRange(new object[] {
            "Optional",
            "Required",
            "Recommended"});
            this.box_PrimaryRequirement.Location = new System.Drawing.Point(3, 217);
            this.box_PrimaryRequirement.Name = "box_PrimaryRequirement";
            this.box_PrimaryRequirement.Size = new System.Drawing.Size(337, 28);
            this.box_PrimaryRequirement.TabIndex = 7;
            this.box_PrimaryRequirement.Text = "Required";
            this.box_PrimaryRequirement.SelectedIndexChanged += new System.EventHandler(this.box_PrimaryRequirement_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 194);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(195, 20);
            this.label15.TabIndex = 6;
            this.label15.Text = "Primary Field Requirement";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(346, 56);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 20);
            this.label16.TabIndex = 21;
            this.label16.Text = "Description";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(3, 56);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(51, 20);
            this.label21.TabIndex = 26;
            this.label21.Text = "Name";
            // 
            // txt_PrimaryDisplayName
            // 
            this.txt_PrimaryDisplayName.Location = new System.Drawing.Point(4, 163);
            this.txt_PrimaryDisplayName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_PrimaryDisplayName.Name = "txt_PrimaryDisplayName";
            this.txt_PrimaryDisplayName.Size = new System.Drawing.Size(335, 26);
            this.txt_PrimaryDisplayName.TabIndex = 5;
            this.txt_PrimaryDisplayName.Text = "Name";
            this.txt_PrimaryDisplayName.TextChanged += new System.EventHandler(this.txt_PrimaryDisplayName_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(346, 138);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(145, 20);
            this.label19.TabIndex = 25;
            this.label19.Text = "Primary Field Name";
            // 
            // txt_PrimaryName
            // 
            this.txt_PrimaryName.Location = new System.Drawing.Point(347, 163);
            this.txt_PrimaryName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_PrimaryName.Name = "txt_PrimaryName";
            this.txt_PrimaryName.Size = new System.Drawing.Size(336, 26);
            this.txt_PrimaryName.TabIndex = 16;
            this.txt_PrimaryName.Text = "Name";
            this.txt_PrimaryName.TextChanged += new System.EventHandler(this.txt_PrimaryName_TextChanged);
            // 
            // btn_Unmap
            // 
            this.btn_Unmap.BackColor = System.Drawing.Color.IndianRed;
            this.btn_Unmap.Enabled = false;
            this.btn_Unmap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Unmap.Image = global::MigrationAssistant.Properties.Resources.unlink;
            this.btn_Unmap.Location = new System.Drawing.Point(92, 825);
            this.btn_Unmap.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Unmap.Name = "btn_Unmap";
            this.btn_Unmap.Size = new System.Drawing.Size(80, 80);
            this.btn_Unmap.TabIndex = 15;
            this.btn_Unmap.UseVisualStyleBackColor = false;
            this.btn_Unmap.Click += new System.EventHandler(this.btn_Unmap_Click);
            // 
            // btn_Map
            // 
            this.btn_Map.BackColor = System.Drawing.Color.MediumAquamarine;
            this.btn_Map.Enabled = false;
            this.btn_Map.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Map.Image = global::MigrationAssistant.Properties.Resources.link;
            this.btn_Map.Location = new System.Drawing.Point(4, 825);
            this.btn_Map.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btn_Map.Name = "btn_Map";
            this.btn_Map.Size = new System.Drawing.Size(80, 80);
            this.btn_Map.TabIndex = 11;
            this.btn_Map.UseVisualStyleBackColor = false;
            this.btn_Map.Click += new System.EventHandler(this.btn_Map_Click);
            // 
            // MigrationAssistantControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fieldCreation);
            this.Controls.Add(this.connectionSettings);
            this.Controls.Add(this.btn_Unmap);
            this.Controls.Add(this.btn_Map);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MigrationAssistantControl";
            this.Size = new System.Drawing.Size(2381, 1406);
            this.Load += new System.EventHandler(this.MigrationAssistantControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.connectionSettings.ResumeLayout(false);
            this.fieldCreation.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_FieldLength)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_PrimaryLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox box_Server;
        private System.Windows.Forms.TextBox box_Database;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox connectionSettings;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.ComboBox box_Mode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox box_TableSchema;
        private System.Windows.Forms.CheckBox box_IgnoreEmpty;
        private System.Windows.Forms.CheckBox box_InspectData;
        private System.Windows.Forms.GroupBox fieldCreation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox box_Solution;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_FieldDisplayName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox box_Requirement;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox box_DataType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown num_FieldLength;
        private System.Windows.Forms.ComboBox box_Format;
        private System.Windows.Forms.Button btn_Map;
        private System.Windows.Forms.Button btn_CreateField;
        private System.Windows.Forms.TabControl sourceTabControl;
        private System.Windows.Forms.TabControl destinationTabControl;
        private System.Windows.Forms.Button btn_Unmap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox box_Table;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckedListBox lst_Solutions;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn_LoadSolutionEntities;
        private System.Windows.Forms.ToolStripButton btn_Save;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_PrimaryDisplayName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox box_PrimaryRequirement;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btn_CreateTable;
        private System.Windows.Forms.NumericUpDown num_PrimaryLength;
        private System.Windows.Forms.ToolStripButton btn_Import;
        private System.Windows.Forms.TextBox txt_FieldName;
        private System.Windows.Forms.Button btn_PublishField;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txt_PrimaryName;
        private System.Windows.Forms.Button btn_PublishEntity;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt_TableDisplayName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txt_TablePluralName;
        private System.Windows.Forms.TextBox txt_tableDescription;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txt_TableName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btn_Close;
        private System.Windows.Forms.ToolStripButton btn_ImportCSV;
        private System.Windows.Forms.ToolStripButton btn_SaveCSV;
        private System.Windows.Forms.ToolStripButton btn_ExportExcel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
