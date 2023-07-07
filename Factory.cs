using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormat;
using System.IO;

namespace MigrationAssistant
{
    internal sealed class Factory : ReadSQLTables
    {
        public List<Table> TableList;

        public Factory(IOrganizationService OrgService)
        {
            this.OrgService = OrgService;
        }
        public List<Field> ImportFromCSV(string FilePath)
        {
            List<Field> Mappings = new List<Field>();

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
                            if (TableList.Find(t => t.NAME == SourceTable) != null)
                            {
                                SourceField = TableList.Find(t => t.NAME == SourceTable).FIELDS.Find(f => f.COLUMN_NAME == SourceFieldName);
                                DestinationField = new Field(DestinationFieldName, DestinationDataType, DestinationTable);

                                SourceField.MAPPED_FIELD = DestinationField;

                                if (SourceField != null) Mappings.Add(SourceField);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }
            return Mappings;
        }
        public void ExportMappingToCSV(string SelectedTableSchema, string SaveFilePath)
        {
            string file = string.Format("{0}/" + Database + "_to_D365_mapping.csv", SaveFilePath);
            string row;

            using (var stream = File.CreateText(file))
            {
                row = "Source Table,Source Column,Source Type,Destination Entity,Destination Field,Destination Type";
                stream.WriteLine(row);
                foreach (Table table in TableList)
                {
                    if ((table.SCHEMA.ToLower() == SelectedTableSchema) || SelectedTableSchema == "")
                    {
                        for (int Idx = 0; Idx < table.FIELDS.Count; Idx++)
                        {
                            if (table.FIELDS[Idx].MAPPED_FIELD != null)
                            {
                                row = string.Format("{0},{1},{2},{3},{4},{5}", table.NAME, table.FIELDS[Idx].COLUMN_NAME, table.FIELDS[Idx].DATA_TYPE, table.MAPPED_ENTITY_NAME, table.FIELDS[Idx].MAPPED_FIELD.COLUMN_NAME, table.FIELDS[Idx].MAPPED_FIELD.DATA_TYPE);
                                stream.WriteLine(row);
                            }
                        }
                    }
                }
            }
        }
        public void ExportMappingToExcel(string SelectedTableSchema, string SaveFilePath, bool IncludeUnmapped)
        {
            IXLWorkbook Workbook = new XLWorkbook();
            IXLWorksheet Worksheet = Workbook.Worksheets.Add("Mappings");

            int Row = 1;
            string TableRange;
            foreach (Table table in TableList)
            {
                if ((table.SCHEMA.ToLower() == SelectedTableSchema || SelectedTableSchema == string.Empty) && (IncludeUnmapped || table.MAPPED_ENTITY_NAME != string.Empty))
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
                            Worksheet.Cell("C" + Row).Value = table.FIELDS[Idx].MAPPED_FIELD.COLUMN_NAME;
                            Worksheet.Cell("D" + Row).Value = table.FIELDS[Idx].MAPPED_FIELD.DATA_TYPE;

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
        public void CreateTable(string SolutionName, string PublisherPrefix, string TableDisplayName, string TablePluralName, string TableName, string TableDescription, string FieldLabel, string FieldSchema, int LengthOrPrecision, int Required)
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
        public void CreateTableAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, string DataType, int LengthOrPrecision, int Required)
        {
            switch (DataType)
            {
                // Text
                case "Single Line of Text": CreateSingleLineAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Multiple Lines of Text": CreateMemoAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // Two Option
                case "Two Option": CreateTwoOptionAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Required); break;
                // Number
                case "Whole Number": CreateWholeNumberAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, Required); break;
                case "Decimal": CreateDecimalAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                case "Money": CreateCurrencyAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                // DateTime
                case "Date Only":
                case "Date and Time": CreateDateTimeAttribute(SolutionName, PublisherPrefix, TableName, FieldLabel, FieldSchema, LengthOrPrecision, Required); break;
                default: break;
            }
        }
        private void CreateSingleLineAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
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

            Console.WriteLine($"\nThe [{FieldLabel}] text attribute has been added to the [{TableName}] entity.");
        }
        private void CreateMemoAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Length, int Required)
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

            Console.WriteLine($"\nThe [{FieldLabel}] memo attribute has been added to the [{TableName}] entity.");
        }
        private void CreateWholeNumberAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Required)
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

            Console.WriteLine($"\nThe [{FieldLabel}] whole number attribute has been added to the [{TableName}] entity.");
        }
        private void CreateDecimalAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
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

            Console.WriteLine($"\nThe [{FieldLabel}] decimal number attribute has been added to the [{TableName}] entity.");
        }
        private void CreateCurrencyAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
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

            Console.WriteLine($"\nThe [{FieldLabel}] currency attribute has been added to the [{TableName}] entity.");
        }
        private void CreatePicklistAttribute(string TableName, ref Field field, Dictionary<int, string> Values, string FieldSchema, int Required)
        {
            /*Console.WriteLine("\nCreating picklist attribute...");
            //string FieldLabel = SplitCamelCase(FieldSchema.ToLower().EndsWith("id") ? FieldSchema.Substring(0,FieldSchema.Length-2) : FieldSchema);   

            OptionSetMetadata OptionSetMeta = new OptionSetMetadata
            {
                IsGlobal = false,
                OptionSetType = OptionSetType.Picklist
            };

            foreach (KeyValuePair<int, string> Pair in Values)
            {
                if(Pair.Value != null) OptionSetMeta.Options.Add(new OptionMetadata(new Label(Pair.Value, LanguageCode), Pair.Key));
                else OptionSetMeta.Options.Add(new OptionMetadata(new Label(Pair.Value, LanguageCode), Pair.Key));
            }

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

            Console.WriteLine($"\nThe [{FieldLabel}] picklist attribute has been added to the [{TableName}] entity.");*/
        }
        private void CreateDateTimeAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Precision, int Required)
        {
            DateTimeFormat Format = Precision > 0 ? DateTimeFormat.DateAndTime : DateTimeFormat.DateOnly;
            
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

            Console.WriteLine($"\nThe [{FieldLabel}] date attribute has been added to the [{TableName}] entity.");
        }
        private void CreateTwoOptionAttribute(string SolutionName, string PublisherPrefix, string TableName, string FieldLabel, string FieldSchema, int Required)
        {
            Console.WriteLine("\nCreating two option attribute...");

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

            Console.WriteLine($"\nThe [{FieldLabel}] two option attribute has been added to the [{TableName}] entity.");
        }
        private void CreateLookupAttribute(Key key)
        {
            // if field name is longer than 50 (46 without prefix) char limit, remove table name
            /*string FieldSchema = key.COLUMN_NAME.Length <= 46 || key.COLUMN_NAME.ToLower() == key.TABLE.ToLower() ? key.COLUMN_NAME : key.COLUMN_NAME.Replace(key.TABLE, "");
            FieldSchema = FieldSchema.ToLower().EndsWith("id") ? FieldSchema.Substring(0, FieldSchema.Length - 2) : FieldSchema;

            //string FieldLabel = SplitCamelCase(FieldSchema);

            // find related table name
            string RelatedTable = ExistingTables.Find(x => x.Contains("_" + key.REFERENCED_TABLE.ToLower()) && x.Length <= key.REFERENCED_TABLE.Length+4); // custom tables take precendence over OOB

            if (RelatedTable == null) // check OOB tables
            {
                RelatedTable = ExistingTables.Find(x => x == key.REFERENCED_TABLE.ToLower());
            }
            if (RelatedTable == null)
            {
                RelatedTable = PublisherPrefix + key.REFERENCED_TABLE.ToLower();
            }

            EntityMetadata PrimaryTable = RetrieveEntityMetaData(ExistingTables.Find(x => x == key.D365_TABLE_NAME || x.Contains("_" + key.D365_TABLE_NAME))); // table exists
            EntityMetadata SecondaryTable = RetrieveEntityMetaData(RelatedTable); // related table exists

            if (PrimaryTable != null && SecondaryTable != null) // both tables exist
            {
                string RelatedTableLabel, RelatedTablePluralLabel;
                AttributeMetadata AttributeMetaData = PrimaryTable.Attributes.FirstOrDefault(a => a.LogicalName == PublisherPrefix + FieldSchema.ToLower());

                //RelatedTableLabel = RemovePunctuations(SplitCamelCase(key.TABLE));
                if (AttributeMetaData == null) // lookup does not already exist
                {
                    if (RelatedTableLabel.EndsWith("s"))
                    {
                        RelatedTablePluralLabel = RelatedTableLabel + "es";
                    }
                    else if (Regex.IsMatch(RelatedTableLabel, "\b.*(a|e|i|o|u)y\b"))
                    {
                        RelatedTablePluralLabel = RelatedTableLabel + "s";
                    }
                    else if (RelatedTableLabel.EndsWith("y"))
                    {
                        RelatedTablePluralLabel = RelatedTableLabel.Substring(0, RelatedTableLabel.Length - 1) + "ies";
                    }
                    else
                    {
                        RelatedTablePluralLabel = RelatedTableLabel + "s";
                    }

                    CreateOneToManyRequest CreateAttributeRequest = new CreateOneToManyRequest()
                    {
                        Lookup = new LookupAttributeMetadata()
                        {
                            DisplayName = new Label(FieldLabel, LanguageCode),
                            LogicalName = PublisherPrefix + FieldSchema + "id",
                            SchemaName = PublisherPrefix + FieldSchema + "Id", // for consistency
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None)

                        },
                        OneToManyRelationship = new OneToManyRelationshipMetadata()
                        {
                            AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                            {
                                Behavior = AssociatedMenuBehavior.UseCollectionName,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label(RelatedTablePluralLabel, LanguageCode),
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
                            ReferencedEntity = RelatedTable,
                            ReferencedAttribute = RelatedTable + "id",
                            ReferencingEntity = key.D365_TABLE_NAME,
                            SchemaName = (PublisherPrefix + RelatedTable).Replace(PublisherPrefix+PublisherPrefix, PublisherPrefix) + "_" + key.D365_TABLE_NAME + "_" + FieldSchema.ToLower()
                        },
                        SolutionUniqueName = SolutionName
                    };

                    OrgService.Execute(CreateAttributeRequest);
                    Console.WriteLine($"\nThe [{FieldLabel}] lookup attribute has been added to the [{PrimaryTable.DisplayName.UserLocalizedLabel.Label}] entity.");
                }
            }*/
        }                
    }
}