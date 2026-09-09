using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class HDDUpdateModel
    {
        public HDDUpdateModel()
        {
            
        }

        public HDDUpdateModel(Guid id, string? name, int? gbSize, HddConnector? connector)
        {
            this.id = id;
            Name = name;
            GbSize = gbSize;
            this.connector = connector;
        }

        public Guid id { get; set; }
        public string? Name { get; set; } = null;
        public int? GbSize { get; set; } = null;
        public HddConnector? connector { get; set; } = null;
    }

    public class HDDCreateModel
    {
        public HDDCreateModel()
        {
            
        }

        public HDDCreateModel(string name, int gbSize, HddConnector connector)
        {
            Name = name;
            GbSize = gbSize;
            this.connector = connector;
        }

        public string Name { get; set; } = string.Empty;
        public int GbSize { get; set; } = 0;
        public HddConnector connector { get; set; } = HddConnector.UNKNOWN;
    }
}
