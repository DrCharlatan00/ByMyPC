using ByMyPc.Postgresql.Models;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.GPUModels.RDTO;
using ByMyPC.Models.HDDModels.RDTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Models.PSUModels.RDTO;
using ByMyPC.Models.RAMModels.RDTO;

namespace ByMyPC.Models.PCModels.RDTO
{
    public class PropertiesPc
    {


        public Guid? CpuId { get; set; }
        public RDTOCpuSmallModel? Cpu { get; set; }

        public Guid? GpuId { get; set; }
        public RDTOGPUSmallModel? Gpu { get; set; }

        public Guid? MotherboardId { get; set; }
        public RDTOModelMotherboardCard? Motherboard { get; set; }

        public Guid? PSUId { get; set; }
        public RDTOPSUSmallModel? PSU { get; set; }
        IEnumerable<RDTORAMSmallModel> Rams { get; set; }
        IEnumerable<RDTOHDDCardModel> Hdds { get; set; }
    }


    public record RDTOPCModel(Guid id, string name, PropertiesPc PropertiesPc);


    public class PropertiesPcSmall
    {
        public Guid? CpuId { get; set; }
        public Guid? GpuId { get; set; }
        public Guid? MotherboardId { get; set; }
        public Guid? PSUId { get; set; }
        IEnumerable<Guid> Rams { get; set; }
        IEnumerable<Guid> Hdds { get; set; }
    }

    public record RDTOPCSmallModel(Guid id, string name, PropertiesPcSmall PropertiesPc);
}
