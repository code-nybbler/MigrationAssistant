using System.Collections.Generic;

namespace MigrationAssistant
{
    internal class Table
    {
        public string SCHEMA;
        public string NAME;
        public List<Field> FIELDS;
        public List<Key> KEYS;
        public int RECORD_COUNT;
        public string MAPPED_ENTITY_NAME;
        //public int ? OBJECT_TYPE_CODE;

        public Table(string SCHEMA, string NAME)
        {
            this.SCHEMA = SCHEMA;
            this.NAME = NAME;
            this.MAPPED_ENTITY_NAME = string.Empty;
        }
    }
}
