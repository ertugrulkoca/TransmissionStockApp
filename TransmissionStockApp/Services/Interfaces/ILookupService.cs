using TransmissionStockApp.Models.ViewModels;

namespace TransmissionStockApp.Services.Interfaces
{
    public interface ILookupService
    {
        Task<LookupDataViewModel> GetLookupDataAsync();
    }

}
