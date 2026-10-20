namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IPcHddRepo
    {
        ValueTask<bool> AttachHddToPc(Guid PcId, Guid HDDId);
        ValueTask<bool> DeAtthachHDD(Guid PcId, Guid HDDId);
    }
}