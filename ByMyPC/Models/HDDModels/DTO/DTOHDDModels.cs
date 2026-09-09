

using ByMyPc.Postgresql.CRUDModel.FiltersModels;

namespace ByMyPC.Models.HDDModels.DTO
{
    public record DTOHDDCreateModel(string name, int GbSize, HddConnectorType ConnectorType);
    public record DTOHDDUpdateModel(Guid id, string? name, int? GbSize, HddConnectorType? ConnectorType);
    public class DTOHDDFilter(string? Name, int? GbSize, HddConnectorType? Connector) {
        public string? Name { get; set; } = Name;
        public int? GbSize { get; set; } = GbSize;
        public HddConnectorType? Connector { get; set; } = Connector;

        public HDDFilterModel ConvertToDbModel(DTOHDDFilter filter) {
            return new HDDFilterModel(
                filter.Name,
                filter.GbSize,
                filter.Connector is not null ? (ByMyPc.Postgresql.Models.HddConnector)filter.Connector : null
                );
        }
    }


}
