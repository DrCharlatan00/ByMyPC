namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IPcRamRepo
    {
        ValueTask<bool> AttachAsync(Guid PcId, Guid RamID);
        Task<bool> DeAttachAsync(Guid PcId, Guid RamID);
    }
}