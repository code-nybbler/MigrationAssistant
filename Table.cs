using System.Collections.Generic;

namespace MigrationAssistant
{
    internal class Table
    {
        public string NAME;
        public List<Field> FIELDS;
        public List<Key> KEYS;
        public int RECORD_COUNT;
        public string MAPPED_ENTITY_NAME;

        public Table(string NAME)
        {
            this.NAME = NAME;
            this.MAPPED_ENTITY_NAME = string.Empty;
        }
    }
}
