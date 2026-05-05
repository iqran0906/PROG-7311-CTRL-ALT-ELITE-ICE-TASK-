using MediLink.Dtos;

namespace MediLink.Interfaces
{
    public interface IDrugInfoService
    {
        Task<List<DrugInfoDto>> SearchDrugInfoAsync(string drugName);
    }
}