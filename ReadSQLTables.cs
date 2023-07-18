using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MigrationAssistant
{
    internal class ReadSQLTables
    {
        public string Database;
        public SqlConnection Connection;
        protected IOrganizationService OrgService;
        protected int LanguageCode = 1033;
        public bool IgnoreEmpty;

        public List<Table> ReadAllSQLTables(int Mode, string SelectedTableSchema)
        {
            List<Table> TableList = new List<Table>();
            List<string[]> Tables = new List<string[]>();

            // Query information_SCHEMA to get all tables
            string Query = String.Format(@"USE [{0}]
                            SELECT TABLE_SCHEMA, TABLE_NAME
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_TYPE = 'BASE TABLE'
                            ORDER BY TABLE_SCHEMA, TABLE_NAME", this.Database);

            Connection.Open();
            using (SqlCommand Command = new SqlCommand(Query, this.Connection))
            {
                using (SqlDataReader Reader = Command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Tables.Add(new string[] { Reader["TABLE_SCHEMA"].ToString(), Reader["TABLE_NAME"].ToString() });
                    }
                }
            }
            Connection.Close();

            Connection.Open();
            int RecCount;
            Table table;

            if (Mode == 0)
            {
                string[] tableArr = Tables.Find(t => t[0].ToLower()+"."+t[1].ToLower() == SelectedTableSchema);

                if (tableArr != null)
                {
                    RecCount = IgnoreEmpty ? GetRecordCount(tableArr) : 0;

                    if (RecCount > 0 || !IgnoreEmpty)
                    {
                        table = RetrieveTableColumns(tableArr);
                        table.RECORD_COUNT = RecCount;
                        TableList.Add(table);
                    }
                }
            }
            else
            {
                foreach (string[] tableArr in Tables)
                {
                    if ((Mode == 1 && tableArr[0].ToLower() == SelectedTableSchema) || Mode == 2)
                    {
                        RecCount = IgnoreEmpty ? GetRecordCount(tableArr) : 0;

                        if (RecCount > 0 || !IgnoreEmpty)
                        {
                            table = RetrieveTableColumns(tableArr);
                            table.RECORD_COUNT = RecCount;
                            TableList.Add(table);
                        }
                    }
                }
            }
            Connection.Close();

            return TableList;
        }
        protected Table RetrieveTableColumns(string[] tbl)
        {
            Table table = new Table(tbl[0]+"."+tbl[1]);
            List<Field> fields = new List<Field>();
            List<Field> fieldsFiltered = new List<Field>();
            List<Key> keys = new List<Key>();

            try
            {
                // retrieve columns
                string Query = String.Format(@"USE [{0}]
                                            SELECT COLUMN_NAME
                                                   ,IS_NULLABLE
                                                   ,DATA_TYPE
                                                   ,CHARACTER_MAXIMUM_LENGTH
                                                   ,NUMERIC_PRECISION
                                                   ,DATETIME_PRECISION
                                            FROM INFORMATION_SCHEMA.COLUMNS
                                            WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{2}'", Database, tbl[1], tbl[0]);

                using (SqlCommand Command = new SqlCommand(Query, this.Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        string NAME, nullable, datatype, maxlength, numberprecision, dateprecision;
                        while (Reader.Read())
                        {
                            NAME = Reader["COLUMN_NAME"].ToString();
                            nullable = Reader["IS_NULLABLE"].ToString();
                            datatype = Reader["DATA_TYPE"].ToString();
                            maxlength = Reader["CHARACTER_MAXIMUM_LENGTH"].ToString();
                            numberprecision = Reader["NUMERIC_PRECISION"].ToString();
                            dateprecision = Reader["DATETIME_PRECISION"].ToString();

                            fields.Add(new Field(NAME, nullable, datatype, maxlength, numberprecision, dateprecision, table.NAME));
                        }
                    }
                }

                // check count of distinct values for each column
                foreach (Field field in fields)
                {
                    field.DISTINCT_VALUE_COUNT = IgnoreEmpty ? GetDistinctValueCount(table, field) : 0;

                    if (field.DISTINCT_VALUE_COUNT > 0 || !IgnoreEmpty) // ignore empty columns
                    {
                        fieldsFiltered.Add(field);
                    }
                }

                // retrieve primary key
                Query = String.Format(@"USE [{0}]
                                    SELECT COLUMN_NAME
                                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1
                                    AND TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{2}'", this.Database, tbl[1], tbl[0]);

                using (SqlCommand Command = new SqlCommand(Query, this.Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        string NAME;
                        while (Reader.Read())
                        {
                            NAME = Reader["COLUMN_NAME"].ToString();

                            if (fieldsFiltered.Find(x => x.COLUMN_NAME == NAME) != null) // is used column
                            {
                                keys.Add(new Key(NAME, 1, table.NAME));
                            }
                        }
                    }
                }

                // retrieve foreign keys
                Query = String.Format(@"USE [{0}]
                                       SELECT t.NAME AS FK_TABLE
                                       ,s.NAME AS FK_TABLE_SCHEMA
                                       ,fk.NAME AS FK_NAME
                                       ,pc.NAME AS COLUMN_NAME
                                       ,rt.NAME AS REFERENCED_TABLE
                                       ,c.NAME AS REFERENCED_COLUMN
                                       ,rs.NAME AS REFERENCED_SCHEMA
                                    FROM sys.foreign_key_columns AS fkc
                                    INNER JOIN sys.foreign_keys AS fk ON fkc.constraint_object_id = fk.object_id
                                    INNER JOIN sys.tables AS t ON fkc.parent_object_id = t.object_id
                                    INNER JOIN sys.tables AS rt ON fkc.referenced_object_id = rt.object_id
                                    INNER JOIN sys.columns AS pc ON fkc.parent_object_id = pc.object_id
                                        AND fkc.parent_column_id = pc.column_id
                                    INNER JOIN sys.columns AS c ON fkc.referenced_object_id = c.object_id
                                        AND fkc.referenced_column_id = c.column_id
                                    INNER JOIN sys.SCHEMAs AS s ON s.SCHEMA_id = t.SCHEMA_id
                                    INNER JOIN sys.SCHEMAs AS rs ON rs.SCHEMA_id = rt.SCHEMA_id
                                    WHERE t.NAME = '{1}' AND s.NAME = '{2}'", this.Database, tbl[1], tbl[0]);

                using (SqlCommand Command = new SqlCommand(Query, this.Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            if (fieldsFiltered.Find(x => x.COLUMN_NAME == Reader["COLUMN_NAME"].ToString()) != null) // is used column
                            {
                                keys.Add(new Key(Reader["COLUMN_NAME"].ToString(), 2, Reader["FK_TABLE_SCHEMA"].ToString()+"."+Reader["FK_TABLE"].ToString(), Reader["REFERENCED_SCHEMA"].ToString()+"."+Reader["REFERENCED_TABLE"].ToString(), Reader["REFERENCED_COLUMN"].ToString()));
                            }
                        }
                    }
                }

                table.FIELDS = fieldsFiltered;
                table.KEYS = keys;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex);
            }

            return table;
        }
        protected int GetRecordCount(string[] tbl)
        {
            int RECORD_COUNT = 0;

            try
            {
                string Query = String.Format(@"USE [{0}]
                                            SELECT COUNT(1) AS REC_COUNT
                                            FROM [{1}].[{2}]", Database, tbl[0], tbl[1]);

                using (SqlCommand Command = new SqlCommand(Query, this.Connection))
                {
                    using (SqlDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            RECORD_COUNT = int.Parse(Reader["REC_COUNT"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex);
            }

            return RECORD_COUNT;
        }
        protected int GetDistinctValueCount(Table table, Field field)
        {
            int DistinctValues = 0;

            if (field.DATA_TYPE.ToLower() == "xml") return DistinctValues;

            string Query = String.Format(@"USE [{0}]
                                    SELECT COUNT(DISTINCT [{1}]) AS CNT
                                    FROM {2}", Database, field.COLUMN_NAME, table.NAME);

            try
            {
                using (SqlCommand Command2 = new SqlCommand(Query, this.Connection))
                {
                    using (SqlDataReader Reader2 = Command2.ExecuteReader())
                    {
                        while (Reader2.Read())
                        {
                            DistinctValues = int.Parse(Reader2["CNT"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Connection.Close();
            }

            return DistinctValues;
        }
        public Dictionary<int, string> GetDistinctValues(string col, Table tbl, Key key, string descCol)
        {
            Connection.Open();

            string Query;
            if (key != null)
            {
                Query = String.Format(@"USE [{0}]
                                        SELECT DISTINCT [{1}].[{2}] AS CODE, [{3}].[{4}] AS DESCRIPTION
                                        FROM [5}
                                        INNER JOIN [{6}] ON [{7}].[{8}] = [{9}].[{10}]
                                        ORDER BY [{11}].[{12}]", this.Database, tbl.NAME, col, key.REFERENCED_TABLE, descCol, tbl.NAME, key.REFERENCED_TABLE, key.REFERENCED_TABLE, key.REFERENCED_COLUMN, tbl.NAME, col, tbl.NAME, col);
            }
            else
            {
                Query = String.Format(@"USE [{0}]
                                        SELECT DISTINCT [{1}] AS DESCRIPTION, '' AS CODE
                                        FROM {2}
                                        ORDER BY [{3}]", this.Database, col, tbl.NAME, col);
            }

            Dictionary<int, string> Values = new Dictionary<int, string>();
            using (SqlCommand Command = new SqlCommand(Query, this.Connection))
            {
                using (SqlDataReader Reader = Command.ExecuteReader())
                {
                    int CodeNumberValue, NewOptionValue = 876060000;
                    bool isNumber;
                    while (Reader.Read())
                    {
                        if (Reader["DESCRIPTION"].ToString() != "")
                        {
                            isNumber = int.TryParse(Reader["CODE"].ToString(), out CodeNumberValue);

                            if (isNumber) Values.Add(CodeNumberValue, Reader["DESCRIPTION"].ToString());
                            else Values.Add(NewOptionValue, Reader["DESCRIPTION"].ToString());

                            NewOptionValue++;
                        }
                    }
                }
            }

            Connection.Close();
            return Values;
        }        
    }
}