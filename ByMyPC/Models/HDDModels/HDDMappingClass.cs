using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.HDDModels.RDTO;

namespace ByMyPC.Models.HDDModels
{
    public class HDDMappingClass : Profile
    {
        public HDDMappingClass()
        {
            CreateMap<DTOHDDCreateModel, HDDCreateModel>()
                .ConstructUsing(
                x => new HDDCreateModel
                    (
                        name: x.name,
                        gbSize: x.GbSize,
                        connector: (ByMyPc.Postgresql.Models.HddConnector)x.ConnectorType
                    )
                );


            
            CreateMap<HDDSmallModel, RDTOHDDCardModel>()
                .ConstructUsing(
                    x => new RDTOHDDCardModel(
                         id: x.id,
                         Name: x.Name,
                         GbSize: x.GbSize
                        )
                );


            CreateMap<HDDDbModel, RDTOHDDModel>()
            .ConstructUsing(
                x => new RDTOHDDModel(
                    id: x.ID,
                    Name: x.Name,
                    GbSize: x.GbSize,
                    ConnectorType: (HddConnectorType)x.connector
                 )
              );
        }
    }
}
