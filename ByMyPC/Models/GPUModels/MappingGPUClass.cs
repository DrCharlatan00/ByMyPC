using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;

namespace ByMyPC.Models.GPUModels
{
    public class MappingGPUClass : Profile
    {
        public MappingGPUClass()
        {
            #region RDTO
            CreateMap<GpuDbModel, RDTOGPUModel>()
                .ConstructUsing(
                    x => new RDTOGPUModel (
                            id: x.ID,
                            name: x.Name,
                            videoMemorySize: x.VideoMemorySize,
                            VideoSlots: x.VideoSlot,
                            memoryBus: x.MemoryBus,
                            typeConnector: x.TypeConnector,
                            typeMemory: x.TypeMemory
                        )
                );

            CreateMap<GPUSmallModel, RDTOGPUSmallModel>()
                .ConstructUsing(
                    x => new RDTOGPUSmallModel(
                            id: x.ID,
                            name: x.Name,
                            videoMemorySize: x.VideoMemorySize
                        )
                );

            #endregion

            #region DTO
            CreateMap<DTOGPUCreateModel, GPUCreateModel>()
                .ConstructUsing(
                    x => new GPUCreateModel(
                            name: x.name,
                            videoMemorySize: x.videoMemorySize,
                            videoSlot: x.VideoSlots,
                            memoryBus: x.memoryBus,
                            typeConnector: x.typeConnector,
                            typeMemory: x.typeMemory
                        )
                );
            #endregion
        }
    }
}
