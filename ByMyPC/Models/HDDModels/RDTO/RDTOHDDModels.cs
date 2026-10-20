namespace ByMyPC.Models.HDDModels.RDTO
{
    public record RDTOHDDCardModel(Guid id,string Name = "n?a", int GbSize = 0);
    public record RDTOHDDModel(Guid id, string Name = "n?a", int GbSize = 0,HddConnectorType ConnectorType = HddConnectorType.UNKNOWN);
}
