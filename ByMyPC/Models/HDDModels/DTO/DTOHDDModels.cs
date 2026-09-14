

using ByMyPc.Postgresql.CRUDModel.FiltersModels;

namespace ByMyPC.Models.HDDModels.DTO
{
    public record DTOHDDCreateModel(string name, int GbSize, HddConnectorType ConnectorType);
    public record DTOHDDUpdateModel(Guid id, string? name, int? GbSize, HddConnectorType? ConnectorType);
    public class DTOHDDFilter {

        public DTOHDDFilter()
        {
            
        }

        public DTOHDDFilter(string? name, int? gbSize, HddConnectorType? connector)
        {
            Name = name;
            GbSize = gbSize;
            Connector = connector;
        }

        public string? Name { get; set; } = null;
        public int? GbSize { get; set; } = null;
        public HddConnectorType? Connector { get; set; } = null;

        public HDDFilterModel ConvertToDbModel(DTOHDDFilter filter) {
            return new HDDFilterModel(
                filter.Name,
                filter.GbSize,
                filter.Connector is not null ? (ByMyPc.Postgresql.Models.HddConnector)filter.Connector : null
                );
        }
    }

    public record DTOHddOperations(Guid PcId,Guid HddId);


}
