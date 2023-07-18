namespace MigrationAssistant
{
    internal class Key
    {
        public string COLUMN_NAME;
        public int KEY_TYPE;
        public string TABLE;
        public string REFERENCED_TABLE;
        public string REFERENCED_COLUMN;

        public Key(string COLUMN_NAME, int KEY_TYPE, string TABLE)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.KEY_TYPE = KEY_TYPE;
            this.TABLE = TABLE;
        }
        public Key(string COLUMN_NAME, int KEY_TYPE, string TABLE, string REFERENCED_TABLE, string REFERENCED_COLUMN)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.KEY_TYPE = KEY_TYPE;
            this.TABLE = TABLE;
            this.REFERENCED_TABLE = REFERENCED_TABLE;
            this.REFERENCED_COLUMN = REFERENCED_COLUMN;
        }
    }
}
