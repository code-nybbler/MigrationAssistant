using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormat;
using System.IO;
using static System.Windows.Forms.CheckedListBox;
using Microsoft.Crm.Sdk.Messages;
using System.Linq;
using System.Data.SqlTypes;

namespace MigrationAssistant
{
    internal sealed class Factory : ReadSQLTables
    {
        public List<Table> TableList;

        public Factory(IOrganizationService OrgService)
        {
            this.OrgService = OrgService;
        }
        public Dictionary<string, List<Field>> ImportFromCSV(string FilePath)
        {
            Dictionary<string, List<Field>> Mappings = new Dictionary<string, List<Field>>();

            StreamReader Reader;
            if (File.Exists(FilePath))
            {
                Reader = new StreamReader(File.OpenRead(FilePath));                
                string SourceFieldName, DestinationFieldName, SourceTable, DestinationTable, DestinationDataType;
                Field SourceField, DestinationField;

                while (!Reader.EndOfStream)
                {
                    var Line = Reader.ReadLine();
                    var Values = Line.Split(',');

                    if (Values.Length == 6)
                    {
                        SourceTable = Values[0];
                        SourceFieldName = Values[1];
                        DestinationTable = Values[3];
                        DestinationFieldName = Values[4];
                        DestinationDataType = Values[5];

                        if (SourceTable != string.Empty && SourceFieldName != string.Empty && DestinationTable != string.Empty && DestinationFieldName != string.Empty && DestinationDataType != string.Empty)
                        {
                            if (TableList.Exists(t => t.NAME == SourceTable))
                            {
                                SourceField = TableList.Find(t => t.NAME == SourceTable).FIELDS.Find(f => f.COLUMN_NAME == SourceFieldName);
                                DestinationField = new Field(DestinationFieldName, DestinationDataType, DestinationTable);

                                SourceField.MAPPED_FIELD = DestinationField;

                                if (SourceField != null)
                                {
                                    if (!Mappings.Keys.Contains(DestinationField.TABLE_NAME))
                                    {
                                        Mappings.Add(DestinationField.TABLE_NAME, new List<Field> { SourceField });
                                    }
                                    else Mappings[DestinationField.TABLE_NAME].Add(SourceField);
                                }
                            }
                        }
                    }
                }
                Reader.Close();
            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }
            return Mappings;
        }
        public void ExportSQLScripts(Dictionary<string, List<Field>> Mappings, string SaveFilePath)
        {
            List<string> columns, joins;
            string file, line;
            Table mainTable = null, relatedTable;
            Field field;

            int maxCount;
            foreach (KeyValuePair<string, List<Field>> kv in Mappings)
            {
                // kv.Key = mapped entity name
                // kv.Value = mapped source fields)
                file = string.Format("{0}/" + kv.Key + ".sql", SaveFilePath);
                columns = new List<string>();
                joins = new List<string>();
                maxCount = -1;

                // get occurence count for each source table in mapped fields to select main table to query from
                var TableCounts = kv.Value.GroupBy(a => a.TABLE_NAME).Select(x => new { key = x.Key, val = x.Count() });

                foreach (var item in TableCounts)
                {
                    if (item.val > maxCount)
                    {
                        maxCount = item.val;
                        mainTable = TableList.Find(t => t.NAME == item.key);
                    }
                }

                if (mainTable != null)
                {
                    using (var stream = File.CreateText(file))
                    {
                        line = "SELECT";
                        stream.WriteLine(line);

                        foreach (Key key in mainTable.KEYS.Where(k => k.KEY_TYPE == 2 && !mainTable.FIELDS.Exists(ff => ff.TABLE_NAME == k.REFERENCED_TABLE)))
                        {
                            field = kv.Value.Find(f => f.TABLE_NAME == key.REFERENCED_TABLE && f.KEY == null);
                            if (field != null)
                            {
                                field.KEY = key;
                                joins.Add(string.Format("LEFT JOIN {0} {1} ON {2}.{3} = {4}.{5}", key.REFERENCED_TABLE, key.REFERENCED_TABLE.Split('.')[1] + "_" + key.COLUMN_NAME, key.REFERENCED_TABLE.Split('.')[1] + "_" + key.COLUMN_NAME, key.REFERENCED_COLUMN, mainTable.NAME.Split('.')[1], key.COLUMN_NAME));
                            }
                        }

                        foreach (Field f in kv.Value) // loop through every mapped column
                        {
                            line = string.Empty;

                            if (f.KEY != null) // key was found
                            {
                                line = string.Format("{0}.{1} AS {2}", f.KEY.REFERENCED_TABLE.Split('.')[1] + "_" + f.KEY.COLUMN_NAME, f.COLUMN_NAME, f.MAPPED_FIELD.COLUMN_NAME);
                            }
                            else if (f.TABLE_NAME != mainTable.NAME) // no key but not from main table
                            {
                                // find a way to link to mapped column's table (within two jumps)
                                foreach (Key key in mainTable.KEYS)
                                {
                                    relatedTable = TableList.Find(t => t.NAME == key.REFERENCED_TABLE);
                                    if (relatedTable != null)
                                    {
                                        foreach (Key k in relatedTable.KEYS)
                                        {
                                            Table table = TableList.Find(t => t.NAME == k.REFERENCED_TABLE);
                                            if (table != null)
                                            {
                                                Field relatedTableField = table.FIELDS.Find(ff => ff.COLUMN_NAME == f.COLUMN_NAME);
                                                if (relatedTableField != null)
                                                {
                                                    joins.Add(string.Format("LEFT JOIN {0} {1} ON {2}.{3} = {4}.{5}", key.REFERENCED_TABLE, key.REFERENCED_TABLE.Split('.')[1] + "_" + key.COLUMN_NAME, key.REFERENCED_TABLE.Split('.')[1] + "_" + key.COLUMN_NAME, key.REFERENCED_COLUMN, mainTable.NAME.Split('.')[1], key.COLUMN_NAME));
                                                    joins.Add(string.Format("LEFT JOIN {0} {1} ON {2}.{3} = {4}.{5}", k.REFERENCED_TABLE, k.REFERENCED_TABLE.Split('.')[1] + "_" + relatedTableField.COLUMN_NAME, k.REFERENCED_TABLE.Split('.')[1] + "_" + relatedTableField.COLUMN_NAME, k.REFERENCED_COLUMN, key.REFERENCED_TABLE.Split('.')[1] + "_" + key.COLUMN_NAME, k.COLUMN_NAME));
                                                    line = string.Format("{0}.{1} AS {2}", k.REFERENCED_TABLE.Split('.')[1] + "_" + relatedTableField.COLUMN_NAME, relatedTableField.COLUMN_NAME, f.MAPPED_FIELD.COLUMN_NAME);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (line != string.Empty) break;
                                }
                            }
                            else // main table column
                            {
                                line = string.Format("{0}.{1} AS {2}", f.TABLE_NAME.Split('.')[1], f.COLUMN_NAME, f.MAPPED_FIELD.COLUMN_NAME);
                            }
                            columns.Add(line);
                        }

                        stream.WriteLine(columns.Aggregate((a, b) => a + ",\n" + b));

                        line = string.Format("FROM {0}", mainTable.NAME);
                        stream.WriteLine(line);

                        // add joins
                        foreach (string join in joins) stream.WriteLine(join);

                        stream.Close();
                    }
                }
            }
        }
        public void ExportMappingToCSV(string SaveFilePath)
        {
            string file = string.Format("{0}/" + Database + "_to_D365_mapping.csv", SaveFilePath);
            string row;

            using (var stream = File.CreateText(file))
            {
                row = "Source Table,Source Column,Source Type,Destination Entity,Destination Field,Destination Type";
                stream.WriteLine(row);
                foreach (Table table in TableList)
                {
                    foreach (Field field in table.FIELDS.Where(f => f.MAPPED_FIELD != null))
                    {
                        row = string.Format("{0},{1},{2},{3},{4},{5}", table.NAME, field.COLUMN_NAME, field.DATA_TYPE, table.MAPPED_ENTITY_NAME, field.MAPPED_FIELD.COLUMN_NAME, field.MAPPED_FIELD.DATA_TYPE);
                        stream.WriteLine(row);
                    }
                }
                stream.Close();
            }
        }
        public void ExportMappingToExcel(string SaveFilePath, bool IncludeUnmapped)
        {
            IXLWorkbook Workbook = new XLWorkbook();
            IXLWorksheet Worksheet = Workbook.Worksheets.Add("Mappings");

            int Row = 1;
            string TableRange;
            foreach (Table table in TableList)
            {
                if (IncludeUnmapped || table.MAPPED_ENTITY_NAME != string.Empty)
                {
                    TableRange = "A" + Row;

                    Worksheet.Range("A" + Row + ":D" + Row).Style.Font.Bold = true;
                    Worksheet.Range("A" + Row + ":D" + Row).Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                    Worksheet.Cell("A" + Row).Value = "Source Table";
                    Worksheet.Cell("C" + Row).Value = "Destination Table";
                    Worksheet.Cell("A" + ++Row).Value = table.NAME;
                    Worksheet.Cell("C" + Row++).Value = table.MAPPED_ENTITY_NAME;

                    Worksheet.Row(Row).Style.Font.Bold = true;
                    Worksheet.Range("A" + Row + ":D" + Row).Style.Fill.SetBackgroundColor(XLColor.LightBlue);

                    Worksheet.Cell("A" + Row).Value = "Source Column";
                    Worksheet.Cell("B" + Row).Value = "Source Type";
                    Worksheet.Cell("C" + Row).Value = "Destination Column";
                    Worksheet.Cell("D" + Row++).Value = "Destination Type";

                    for (int Idx = 0; Idx < table.FIELDS.Count; Idx++)
                    {
                        if (table.FIELDS[Idx].MAPPED_FIELD != null || IncludeUnmapped)
                        {
                            Worksheet.Cell("A" + Row).Value = table.FIELDS[Idx].COLUMN_NAME;
                            Worksheet.Cell("B" + Row).Value = table.FIELDS[Idx].DATA_TYPE;
                            Worksheet.Cell("C" + Row).Value = table.FIELDS[Idx].MAPPED_FIELD != null ? table.FIELDS[Idx].MAPPED_FIELD.COLUMN_NAME : string.Empty;
                            Worksheet.Cell("D" + Row).Value = table.FIELDS[Idx].MAPPED_FIELD != null ? table.FIELDS[Idx].MAPPED_FIELD.DATA_TYPE : string.Empty;

                            Row++;
                        }
                    }

                    TableRange += ":D" + (Row - 1);

                    Worksheet.Range(TableRange).Style.Border.OutsideBorder = XLBorderStyleValues.Medium; // apply outside border

                    Row++; // Skip row
                }
            }
            
            Workbook.SaveAs(string.Format("{0}/" + Database + "_to_D365_mapping.xlsx", SaveFilePath));
        }
        public void CreateTable(string SolutionName, string PublisherPrefix, string TableDisplayName, string TablePluralName, string TableName, string TableDescription, string FieldLabel, string FieldSchema, int LengthOrPrecision, int Required, int LanguageCode)
        {
            this.LanguageCode = LanguageCode;

            try
            {
                CreateEntityRequest createrequest = new CreateEntityRequest
                {
                    // Define the entity
                    Entity = new EntityMetadata
                    {
                        SchemaName = PublisherPrefix + TableName,
                        DisplayName = new Label(TableDisplayName, LanguageCode),
                        DisplayCollectionName = new Label(TablePluralName, LanguageCode),
                        Description = new Label(TableDescription, LanguageCode),
                        OwnershipType = OwnershipTypes.UserOwned
                    },

                    // Define the primary attribute for the entity
                    PrimaryAttribute = new StringAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        MaxLength = LengthOrPrecision,
                        FormatName = StringFormatName.Text,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        Description = new Label("The primary attribute for the " + TableDisplayName + " table.", LanguageCode)
                    },

                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(createrequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void CreateTableAttribute(string SolutionName, string PublisherPrefix, int OptionValuePrefix, string TableName, string FieldLabel, string FieldSchema, string DataType, int LengthOrPrecision, CheckedItemCollection OptionSetValues, int Required, EntityMetadata Table, EntityMetadata Target, string RelationshipName, int LanguageCode)
        {
            this.LanguageCode = LanguageCode;

            switch (DataType)
            {
                // Text
                case "Single Line of Text": CreateSingleLineAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Multiple Lines of Text": CreateMemoAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // Two Option
                case "Two Options": CreateTwoOptionAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Required); break;
                // Picklist
                case "Option Set": CreatePicklistAttribute(SolutionName, PublisherPrefix, OptionValuePrefix, TableName, FieldLabel, FieldSchema, OptionSetValues, Required); break;
                case "MultiSelect Option Set": CreateMultiSelectPicklistAttribute(SolutionName, PublisherPrefix, OptionValuePrefix, TableName, FieldLabel, FieldSchema, OptionSetValues, Required); break;
                // Number
                case "Whole Number": CreateWholeNumberAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Required); break;
                case "Decimal Number": CreateDecimalAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Floating Point Number": CreateDoubleAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Currency": CreateCurrencyAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // DateTime
                case "Date Only":
                case "Date and Time": CreateDateTimeAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // File
                case "File": CreateFileAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Image": CreateImageAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // Lookup
                case "Lookup": CreateLookupAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Table, Target, RelationshipName, Required); break;
                case "Customer": CreateCustomerAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Table, Required); break;
                default: break;
            }
        }
        private void CreateFileAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new FileAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxSizeInKB = Length
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateImageAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new ImageAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxSizeInKB = Length
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateSingleLineAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new StringAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        FormatName = StringFormatName.Text,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxLength = Length
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateMemoAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new MemoAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        Format = StringFormat.TextArea,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxLength = Length
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateWholeNumberAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new IntegerAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxValue = IntegerAttributeMetadata.MaxSupportedValue,
                        MinValue = IntegerAttributeMetadata.MinSupportedValue
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateDecimalAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new DecimalAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxValue = IntegerAttributeMetadata.MaxSupportedValue,
                        MinValue = IntegerAttributeMetadata.MinSupportedValue,
                        Precision = Precision
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateDoubleAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new DoubleAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxValue = IntegerAttributeMetadata.MaxSupportedValue,
                        MinValue = IntegerAttributeMetadata.MinSupportedValue,
                        Precision = Precision
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateCurrencyAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new MoneyAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        MaxValue = IntegerAttributeMetadata.MaxSupportedValue,
                        MinValue = IntegerAttributeMetadata.MinSupportedValue,
                        Precision = Precision
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateMultiSelectPicklistAttribute(string SolutionName, string PublisherPrefix, int OptionValuePrefix, string TableName, string FieldLabel, string FieldSchema, CheckedItemCollection OptionSetValues, int Required)
        {
            OptionSetMetadata OptionSetMeta = new OptionSetMetadata
            {
                IsGlobal = false,
                OptionSetType = OptionSetType.Picklist
            };

            int OptionValue = OptionValuePrefix;
            foreach (string OptionSetValue in OptionSetValues)
            {
                Label label = new Label(OptionSetValue, LanguageCode);
                OptionSetMeta.Options.Add(new OptionMetadata(label, OptionValue++));
            }

            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new MultiSelectPicklistAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        OptionSet = OptionSetMeta
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreatePicklistAttribute(string SolutionName, string PublisherPrefix, int OptionValuePrefix, string TableName, string FieldLabel, string FieldSchema, CheckedItemCollection OptionSetValues, int Required)
        {
            OptionSetMetadata OptionSetMeta = new OptionSetMetadata
            {
                IsGlobal = false,
                OptionSetType = OptionSetType.Picklist
            };

            int OptionValue = OptionValuePrefix;
            foreach (string OptionSetValue in OptionSetValues)
            {
                Label label = new Label(OptionSetValue, LanguageCode);
                OptionSetMeta.Options.Add(new OptionMetadata(label, OptionValue++));
            }

            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new PicklistAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        OptionSet = OptionSetMeta
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateDateTimeAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
        {
            DateTimeFormat Format = Precision > 0 ? DateTimeFormat.DateAndTime : DateTimeFormat.DateOnly;
         
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new DateTimeAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        Format = Format
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateTwoOptionAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Required)
        {
            try
            {
                CreateAttributeRequest CreateAttributeRequest = new CreateAttributeRequest
                {
                    EntityName = TableName,
                    Attribute = new BooleanAttributeMetadata
                    {
                        SchemaName = PublisherPrefix + FieldSchema,
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required),
                        OptionSet = new BooleanOptionSetMetadata(
                                        new OptionMetadata(new Label("No", LanguageCode), 0),
                                        new OptionMetadata(new Label("Yes", LanguageCode), 1)                                    
                                    )
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateLookupAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, EntityMetadata Table, EntityMetadata Target, string RelationshipName, int Required)
        {
            try
            {
                CreateOneToManyRequest CreateAttributeRequest = new CreateOneToManyRequest()
                {
                    Lookup = new LookupAttributeMetadata()
                    {
                        DisplayName = new Label(FieldLabel, LanguageCode),
                        LogicalName = PublisherPrefix + FieldSchema.ToLower(),
                        SchemaName = PublisherPrefix + FieldSchema,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required)

                    },
                    OneToManyRelationship = new OneToManyRelationshipMetadata()
                    {
                        AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                        {
                            Behavior = AssociatedMenuBehavior.UseCollectionName,
                            Group = AssociatedMenuGroup.Details,
                            Label = new Label(Table.DisplayCollectionName.UserLocalizedLabel.Label, LanguageCode),
                            Order = 10000
                        },
                        CascadeConfiguration = new CascadeConfiguration()
                        {
                            Assign = CascadeType.NoCascade,
                            Delete = CascadeType.RemoveLink,
                            Merge = CascadeType.NoCascade,
                            Reparent = CascadeType.NoCascade,
                            Share = CascadeType.NoCascade,
                            Unshare = CascadeType.NoCascade
                        },
                        ReferencedEntity = Target.LogicalName,
                        ReferencedAttribute = Target.PrimaryIdAttribute,
                        ReferencingEntity = TableName,
                        SchemaName = PublisherPrefix + RelationshipName
                    },
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void CreateCustomerAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, EntityMetadata Table, int Required)
        {
            string ToAccount = PublisherPrefix + "account_" + Table.LogicalName + "_" + FieldSchema;
            string ToContact = PublisherPrefix + "contact_" + Table.LogicalName + "_" + FieldSchema;

            List<OneToManyRelationshipMetadata> Relationships = new List<OneToManyRelationshipMetadata>
            {
                new OneToManyRelationshipMetadata
                {
                    AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                    {
                        Behavior = AssociatedMenuBehavior.UseCollectionName,
                        Group = AssociatedMenuGroup.Details,
                        Label = new Label(Table.DisplayCollectionName.UserLocalizedLabel.Label, LanguageCode),
                        Order = 10000
                    },
                    CascadeConfiguration = new CascadeConfiguration()
                    {
                        Assign = CascadeType.NoCascade,
                        Delete = CascadeType.RemoveLink,
                        Merge = CascadeType.NoCascade,
                        Reparent = CascadeType.NoCascade,
                        Share = CascadeType.NoCascade,
                        Unshare = CascadeType.NoCascade
                    },
                    ReferencedEntity = "account",
                    ReferencedAttribute = "accountid",
                    ReferencingEntity = Table.LogicalName,
                    SchemaName = ToAccount
                },
                new OneToManyRelationshipMetadata
                {
                    AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                    {
                        Behavior = AssociatedMenuBehavior.UseCollectionName,
                        Group = AssociatedMenuGroup.Details,
                        Label = new Label(Table.DisplayCollectionName.UserLocalizedLabel.Label, LanguageCode),
                        Order = 10000
                    },
                    CascadeConfiguration = new CascadeConfiguration()
                    {
                        Assign = CascadeType.NoCascade,
                        Delete = CascadeType.RemoveLink,
                        Merge = CascadeType.NoCascade,
                        Reparent = CascadeType.NoCascade,
                        Share = CascadeType.NoCascade,
                        Unshare = CascadeType.NoCascade
                    },
                    ReferencedEntity = "contact",
                    ReferencedAttribute = "contactid",
                    ReferencingEntity = Table.LogicalName,
                    SchemaName = ToContact
                }
            };

            try
            {
                CreatePolymorphicLookupAttributeRequest CreateAttributeRequest = new CreatePolymorphicLookupAttributeRequest
                {
                    Lookup = new LookupAttributeMetadata()
                    {
                        LogicalName = PublisherPrefix + FieldSchema.ToLower(),
                        SchemaName = PublisherPrefix + FieldSchema,
                        DisplayName = new Label(FieldLabel, 1033),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty((AttributeRequiredLevel)Required)
                    },
                    OneToManyRelationships = Relationships.ToArray(),
                    SolutionUniqueName = SolutionName
                };

                OrgService.Execute(CreateAttributeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}