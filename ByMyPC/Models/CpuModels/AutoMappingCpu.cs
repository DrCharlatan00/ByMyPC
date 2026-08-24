using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;

namespace ByMyPC.Models.CpuModels
{
    public class CpuMappingProfile : Profile
    {
        public CpuMappingProfile()
        {
            #region DTO
            CreateMap<DTOCpuCreateModel, CpuCreateModel>()
                .ConstructUsing(
                    x => new CpuCreateModel(
                        name: x.Name,
                        socket: x.Socket,
                        frequency: x.Frequency,
                        count_Cores: x.Count_Cores,
                        isLive: x.IsLive
                        )
                );
            CreateMap<DTOCpuUpdateModel, CpuUpdateModel>()
                .ConstructUsing(
                x => new CpuUpdateModel(
                        name: x.Name == null ? "skip" : x.Name,
                        socket: x.Socket == null ? "skip" : x.Socket,
                        frequency: x.Frequency,
                        count_Cores: x.Count_Cores,
                        isLive: x.IsLive
                        )
                );
            #endregion


            #region RDTO
            CreateMap<CpuDbModel, RDTOCpuModel>()
                .ConstructUsing(
                    x => new RDTOCpuModel(
                        x.ID,
                        x.Name,
                        x.Socket,
                        x.Frequency,
                        x.Count_Cores,
                        x.IsLive
                        )
                );

            CreateMap<CpuSmallModel, RDTOSmallModel>()
                .ConstructUsing(
                    x => new RDTOSmallModel(
                        x.Name,
                        x.Socket
                        )
                );
            #endregion
        }
    }
}
