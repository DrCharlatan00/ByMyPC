using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;

namespace ByMyPC.Models.MotherbordModels
{
    public class MotherboardMappingClass : Profile
    {
        public MotherboardMappingClass()
        {
            CreateMap<MotherboardDbModel, RDTOModelMotherboard>().
                ConstructUsing(x => new RDTOModelMotherboard(
                        id: x.ID,
                        Name: x.Name,
                        Socket: x.Socket,
                        MaxRamSlot: x.MaxRamSlot,
                        MaxRamFrequency: x.MaxRamFrequency,
                        MaxCpuFrequency: x.MaxCpuFrequency,
                        IntegrationGpu: x.IntegrationGpu,
                        IsLive: x.IsLive,
                        VideoSlot: (RDTO.VideoSlots)x.VideoSlot
                    ));
            CreateMap<MotherboardSmallDbModel, RDTOModelMotherboardCard>()
                .ConstructUsing(
                    x => new RDTOModelMotherboardCard(
                        Name: x.Name,
                        Socket: x.Socket,
                        IsLive: x.IsLive
                        )
                );

            CreateMap<DTOMotherboardCreateModel, MotherboardCreateModel>()
                .ConstructUsing(
                x => new MotherboardCreateModel(
                        name: x.Name,
                        socket: x.Socket,
                        maxRamSlot: x.MaxRamSlot,
                        maxRamFrequency: x.MaxRamFrequency,
                        maxCpuFrequency: x.MaxCpuFrequency,
                        integrationGpu: x.IntegrationGpu,
                        isLive: x.IsLive,
                        videoSlot: (ByMyPc.Postgresql.Models.VideoSlots)x.VideoSlot
                    )
                );
            CreateMap<DTOMotherboardUpdateModel, MotherboardUpdateModel>()
                .ConstructUsing(
                x => new MotherboardUpdateModel(
                        iD: x.id,
                        name: x.Name,
                        socket: x.Socket,
                        isLive: x.IsLive
                    )
                );
        }
    }
}
