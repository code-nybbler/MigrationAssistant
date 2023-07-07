namespace MigrationAssistant
{
    internal class Key
    {
        public string COLUMN_NAME;
        public int KEY_TYPE;
        public string TABLE;
        public string TABLE_SCHEMA;
        public string D365_TABLE_NAME;
        public string REFERENCED_SCHEMA;
        public string REFERENCED_TABLE;
        public string REFERENCED_COLUMN;

        public Key(string COLUMN_NAME, int KEY_TYPE, string TABLE)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.KEY_TYPE = KEY_TYPE;
            this.TABLE = TABLE;
        }
        public Key(string COLUMN_NAME, int KEY_TYPE, string TABLE, string TABLE_SCHEMA, string D365_TABLE_NAME, string REFERENCED_SCHEMA, string REFERENCED_TABLE, string REFERENCED_COLUMN)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.KEY_TYPE = KEY_TYPE;
            this.TABLE = TABLE;
            this.TABLE_SCHEMA = TABLE_SCHEMA;
            this.D365_TABLE_NAME = D365_TABLE_NAME;
            this.REFERENCED_SCHEMA = REFERENCED_SCHEMA;
            this.REFERENCED_TABLE = REFERENCED_TABLE;
            this.REFERENCED_COLUMN = REFERENCED_COLUMN;
        }
    }
}
