using ByMyPc.Postgresql.Models;


namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class RamCreateModel
    {

        public RamCreateModel()
        {
            
        }

        public RamCreateModel(string name, int size, TypeDDR dDRType, int frequency, bool isLive)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
            DDRType = dDRType;
            Frequency = frequency;
            IsLive = isLive;
        }

        public string Name { get; set; } = "N/A";
        public int Size { get; set; } = 8;
        public TypeDDR DDRType { get; set; } = TypeDDR.DDR;
        public int Frequency { get; set; } = 0;
        public bool IsLive { get; set; } = false;
    }
    public class RamUpdateModel
    {
        public RamUpdateModel()
        {
            
        }

        public RamUpdateModel(Guid iD, string? name, int? size, TypeDDR? dDRType, int? frequency, bool? isLive)
        {
            ID = iD;
            Name = name;
            Size = size;
            DDRType = dDRType;
            Frequency = frequency;
            IsLive = isLive;
        }

        public Guid ID { get; set; }
        public string? Name { get; set; } = null;
        public int? Size { get; set; } = null;
        public TypeDDR? DDRType { get; set; } = null;
        public int? Frequency { get; set; } = null;
        public bool? IsLive { get; set; } = null;
    }

}

