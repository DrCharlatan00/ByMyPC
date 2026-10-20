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
        public PropertiesPc()
        {
                
        }

        public PropertiesPc(RDTOCpuSmallModel? cpu, RDTOGPUSmallModel? gpu, RDTOModelMotherboardCard? motherboard, RDTOPSUSmallModel? pSU, IEnumerable<RDTORAMSmallModel> rams, IEnumerable<RDTOHDDCardModel> hdds)
        {
            Cpu = cpu;
            Gpu = gpu;
            Motherboard = motherboard;
            PSU = pSU;
            Rams = rams;
            Hdds = hdds;
        }

        public RDTOCpuSmallModel? Cpu { get; set; }

        public RDTOGPUSmallModel? Gpu { get; set; }

        public RDTOModelMotherboardCard? Motherboard { get; set; }

        public RDTOPSUSmallModel? PSU { get; set; }
        IEnumerable<RDTORAMSmallModel> Rams { get; set; }
        IEnumerable<RDTOHDDCardModel> Hdds { get; set; }
    }


    public record RDTOPCModel(Guid id, string name, PropertiesPc PropertiesPc);


    public class PropertiesPcSmall
    {
        public PropertiesPcSmall()
        {
              
        }
        public PropertiesPcSmall(Guid? cpuId, Guid? gpuId, Guid? motherboardId, Guid? pSUId, IEnumerable<Guid> rams, IEnumerable<Guid> hdds)
        {
            CpuId = cpuId;
            GpuId = gpuId;
            MotherboardId = motherboardId;
            PSUId = pSUId;
            Rams = rams;
            Hdds = hdds;
        }

        public Guid? CpuId { get; set; }
        public Guid? GpuId { get; set; }
        public Guid? MotherboardId { get; set; }
        public Guid? PSUId { get; set; }
        IEnumerable<Guid> Rams { get; set; }
        IEnumerable<Guid> Hdds { get; set; }
    }

    public record RDTOPCSmallModel(Guid id, string name, PropertiesPcSmall PropertiesPc);
}
