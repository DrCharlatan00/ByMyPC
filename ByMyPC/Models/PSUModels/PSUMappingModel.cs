using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;

namespace ByMyPC.Models.PSUModels
{
    public class PSUMappingModel : Profile
    {
        public PSUMappingModel()
        {
            #region DTO
            CreateMap<DTOPSUModelCreate, PSUCreateModel>()
                .ConstructUsing(
                x => new PSUCreateModel(name: x.Name,
                                        x.PowerWatt,
                                        x.IsLive,
                                        x.Size,
                                        x.IsModular,
                                        x.IsCertified)
                );
            #endregion

            #region RDTO
            CreateMap<PSUSmallModel, RDTOPSUSmallModel>()
                .ConstructUsing(
                    x => new RDTOPSUSmallModel(
                        x.Id,
                        x.Name,
                        x.PowerWatt,
                        x.IsLive
                        )
                );

            CreateMap<PSUDbModel, RDTOPSUModel>()
                .ConstructUsing(
                    x => new RDTOPSUModel(
                           x.ID,
                           x.Name,
                           x.PowerWatt,
                           x.IsLive,
                           x.Size,
                           x.IsModular,
                           x.IsСertified
                        )
                );
            #endregion
        }
    }
}
