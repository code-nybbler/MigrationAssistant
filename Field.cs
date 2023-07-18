namespace MigrationAssistant
{
    internal class Field
    {
        public string COLUMN_NAME;
        public string IS_NULLABLE;
        public string DATA_TYPE;        
        public string CHARACTER_MAXIMUM_LENGTH;
        public string NUMERIC_PRECISION;
        public string DATETIME_PRECISION;
        public string TABLE_NAME;

        public Field MAPPED_FIELD;
        public Key KEY;

        public int DISTINCT_VALUE_COUNT;

        public Field(string COLUMN_NAME, string DATA_TYPE, string TABLE_NAME)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.DATA_TYPE = DATA_TYPE;
            this.TABLE_NAME = TABLE_NAME;
        }

        public Field(string COLUMN_NAME, string IS_NULLABLE, string DATA_TYPE, string CHARACTER_MAXIMUM_LENGTH, string NUMERIC_PRECISION, string DATETIME_PRECISION, string TABLE_NAME)
        {
            this.COLUMN_NAME = COLUMN_NAME;
            this.IS_NULLABLE = IS_NULLABLE;
            this.DATA_TYPE = DATA_TYPE;
            this.CHARACTER_MAXIMUM_LENGTH = CHARACTER_MAXIMUM_LENGTH;
            this.NUMERIC_PRECISION = NUMERIC_PRECISION;
            this.DATETIME_PRECISION = DATETIME_PRECISION;
            this.TABLE_NAME = TABLE_NAME;
        }
    }
}
