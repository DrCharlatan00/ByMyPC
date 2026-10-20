using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Models.RAMModels.RDTO;

namespace ByMyPC.Models.RAMModels
{
    public class MappingRamClass : Profile  
    {
        public MappingRamClass()
        {
            #region RDTO Map

            CreateMap<RamDbModel,RDTORAMModel>()
                .ConstructUsing(x => new RDTORAMModel(
                    x.ID,
                    x.Name,
                    x.Size,
                    x.DDRType,
                    x.Frequency,
                    x.IsLive
                    ));

            CreateMap<RamDbModel, RDTORAMSmallModel>()
    .ConstructUsing(x => new RDTORAMSmallModel(
        x.ID,
        x.Name,
        x.Size,
        x.DDRType
        ));

            CreateMap<RamSmallModel, RDTORAMSmallModel>()
               .ConstructUsing(x => new RDTORAMSmallModel(
                   x.ID,
                   x.Name,
                   x.Size,
                   x.DDRType
                   ));
            #endregion

            #region DTO
            CreateMap<DTORAMCreate, RamCreateModel>()
                .ConstructUsing(
                    x => new RamCreateModel(
                        x.name,
                        x.size,
                        x.Type,
                        x.frequency,
                        x.isLive
                        )
                );

            #endregion
        }
    }
}
