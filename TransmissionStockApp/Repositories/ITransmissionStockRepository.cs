using TransmissionStockApp.Models.Entities;

namespace TransmissionStockApp.Repositories
{
    public interface ITransmissionStockRepository : IRepository<TransmissionStock>
    {
        Task<List<TransmissionStock>> GetAllWithRelationsAsync();
        Task<TransmissionStock?> GetByIdWithRelationsAsync(int id);
    }
}
